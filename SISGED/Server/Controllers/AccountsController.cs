using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models;
using SISGED.Shared.Models.Requests.Account;
using SISGED.Shared.Models.Requests.User;
using SISGED.Shared.Models.Responses.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

                await _userService.CreateUserAsync(user);

                var usertoken = BuildToken(user, "");

                return Ok(usertoken);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("name/{username}")]
        public async Task<ActionResult<SessionAccountResponse>> GetUserDataAsync([FromRoute] string username)
        {
            try
            {
                SessionAccountResponse sessionAccount = new SessionAccountResponse();
                List<Permission> permisosInterfaces = new List<Permission>();
                List<Permission> toolsPermissions = new List<Permission>();

                User user = new User();
                user = await _userService.GetUserByNameAsync(username);

                Role userrole = new Role();
                userrole = await _roleService.GetRoleByIdAsync(user.Rol);

                foreach (string permissionId in userrole.Interfaces)
                {
                    //Obtener el objeto permiso y añadirlo
                    Permission permission = new Permission();
                    permission = await _permissionService.GetPermissionByIdAsync(permissionId);
                    permisosInterfaces.Add(permission);
                }

                foreach (string permissionId in userrole.Tools)
                {
                    //Obtener el bjeto permiso y añadirlo
                    Permission permission = new Permission();
                    permission = await _permissionService.GetPermissionByIdAsync(permissionId);
                    toolsPermissions.Add(permission);
                }

                sessionAccount.User = user;
                sessionAccount.Role = userrole.Name;
                sessionAccount.ToolPermissions = toolsPermissions;
                sessionAccount.InterfacePermissions = permisosInterfaces;

                return Ok(sessionAccount);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("role/{roleId}")]
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
                var result = await _userService.VerifyUserLoginAsync(userInfo.Username, userInfo.Password);
                if (result)
                {
                    User user = await _userService.GetUserByNameAsync(userInfo.Username);
                    Role role = await _roleService.GetRoleByIdAsync(user.Rol);
                    var userToken = BuildToken(user, role.Name);
                    return Ok(userToken);
                }
                else
                {
                    return BadRequest("Intento de inicio de sesión inválido.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        private UserToken BuildToken(User user, string roleName)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("miValor", "Lo que yo quiera"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(-5).AddYears(1);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
