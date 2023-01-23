using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Account;
using SISGED.Shared.Models.Requests.User;
using SISGED.Shared.Models.Responses.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountsController(IUserService userService, IRoleService roleService, IPermissionService permissionService, IMapper mapper, IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _permissionService = permissionService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<UserToken>> CreateUserAsync(UserRegisterRequest userRegisterRequest)
        {
            try
            {
                var user = _mapper.Map<User>(userRegisterRequest);

                var userValidation = await _userService.ValidateUserRegisterAsync(user);

                if (userValidation.Result) return BadRequest(userValidation.ErrorMessage);

                var encryptedPassword = EncryptPassword(user.Password);

                SetUserPassword(user, encryptedPassword);

                await _userService.CreateUserAsync(user);

                var role = await _roleService.GetRoleByIdAsync(user.Rol);

                var usertoken = BuildToken(user, role.Name);

                return Ok(usertoken);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo registrar el usuario en el sistema");
            }
        }

        [HttpGet("name/{username}")]
        public async Task<ActionResult<SessionAccountResponse>> GetUserDataAsync([FromRoute] string username)
        {
            try
            {

                var user = await _userService.GetUserByNameAsync(username);
                var userrole = await _roleService.GetRoleByIdAsync(user.Rol);

                var permissionInterfacesTasks = userrole.Interfaces.Select(async (permissionId) =>
                {
                    var permission = await _permissionService.GetPermissionByIdAsync(permissionId);
                    return permission;
                });

                var permissionToolsTasks = userrole.Tools.Select(async (permissionId) =>
                {
                    var permission = await _permissionService.GetPermissionByIdAsync(permissionId);
                    return permission;
                });

                var permissionInterfaces = await Task.WhenAll(permissionInterfacesTasks);
                var permissionTools = await Task.WhenAll(permissionToolsTasks);

                var sessionAccount = new SessionAccountResponse(permissionTools.ToList(), permissionInterfaces.ToList(), userrole.Name, user);

                return Ok(sessionAccount);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("renewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> RenewToken()
        {
            var user = await _userService.VerifyUserLoginAsync(HttpContext.User.Identity!.Name!);
            var role = await _roleService.GetRoleByIdAsync(user.Rol);

            var userToken = BuildToken(user, role.Name);

            return Ok(userToken);
        }

        [HttpGet("roles/{roleId}")]
        public async Task<ActionResult<Role>> GetRoleByIdAsync([FromRoute] string roleId)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(roleId);

                return Ok(role);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login(AccountLoginRequest userInfo)
        {
            try
            {

                var user = await _userService.VerifyUserLoginAsync(userInfo.Username);

                var encryptedPassword = EncryptPassword(userInfo.Password, Convert.FromBase64String(user.Salt));

                if (user.Password != encryptedPassword.Password) return BadRequest("Inicio de sesión inválido");
                
                var role = await _roleService.GetRoleByIdAsync(user.Rol);
                var userToken = BuildToken(user, role.Name);
                
                return Ok(userToken);
                
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Inicio de sesión inválido");
            }
        }


        #region private methods

        private static void SetUserPassword(User user, EncryptedPasswordDTO encryptedPasswordDTO)
        {
            user.Salt = encryptedPasswordDTO.Salt;
            user.Password = encryptedPasswordDTO.Password;
        }

        private static EncryptedPasswordDTO EncryptPassword(string password)
        {
            var salt = new byte[16];

            using var random = RandomNumberGenerator.Create();
            random.GetBytes(salt);

            return EncryptPassword(password, salt);
        }

        private static EncryptedPasswordDTO EncryptPassword(string password, byte[] salt)
        {
            var derivatedKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 1000, 32);

            var newPassword = Convert.ToBase64String(derivatedKey);

            return new(newPassword, Convert.ToBase64String(salt));
        }

        private UserToken BuildToken(User user, string roleName)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("userId", user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(8);

            var token = new JwtSecurityToken(
               issuer: _configuration.GetValue<string>("JWT:issuer"),
               audience: _configuration.GetValue<string>("JWT:audience"),
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken(new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }
        #endregion


    }
}
