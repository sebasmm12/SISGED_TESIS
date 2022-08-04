using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.User;
using SISGED.Shared.Models.Responses.User;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITrayService _trayService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IFileService fileService, ITrayService trayService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
            _fileService = fileService;
            _trayService = trayService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfoResponse>>> GetUsersAsync()
        {
            try
            {
                var users = await _userService.GetUsersAsync();

                var usersInfoResponse = _mapper.Map<IEnumerable<UserInfoResponse>>(users);

                return Ok(usersInfoResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult> CreateUserAsync(UserRegisterRequest userRegisterRequest)
        {
            try
            {
                var user = _mapper.Map<User>(userRegisterRequest);

                var fileUrl = await _fileService.SaveFileAsync(new(user.GetProfile(), ".jpg", "users"));

                user.SetProfile(fileUrl);

                await _userService.CreateUserAsync(user);

                if (user.Type != "cliente") await _trayService.RegisterUserTrayAsync(user.Type, user.Id);

                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserInfoResponse>> GetUserByIdAsync([FromRoute] string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                return Ok(user);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("name/{userName}")]
        public async Task<ActionResult<UserInfoResponse>> GetUserByNameAsync([FromRoute] string userName)
        {
            try
            {
                var user = await _userService.GetUserByNameAsync(userName);

                return Ok(user);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUserAsync([FromBody] UserUpdateRequest userUpdateRequest)
        {
            try
            {
                var newUser = _mapper.Map<User>(userUpdateRequest);

                var currentUser = await _userService.GetUserByIdAsync(userUpdateRequest.Id);

                var fileUrl = await _fileService.UpdateImageAsync(new(newUser.GetProfile(), currentUser.GetProfile(), ".jpg", "users"));

                newUser.SetProfile(fileUrl);

                await _userService.UpdateUserAsync(newUser);

                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> UpdateUserStateAsync([FromRoute] string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                await _userService.UpdateUserStateAsync(userId, user.State);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("password")]
        public async Task<ActionResult> UpdatUserPasswordAsync([FromBody] UserPasswordUpdateRequest userPasswordUpdateRequest)
        {
            try
            {
                if (!await _userService.VerifyUserExistsAsync(userPasswordUpdateRequest.UserId)) return NotFound($"No se pudo encontrar el usuario con el identificador {userPasswordUpdateRequest.UserId}");

                await _userService.UpdateUserPasswordAsync(userPasswordUpdateRequest.UserId, userPasswordUpdateRequest.NewPassword);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("state/{userState}")]
        public async Task<ActionResult<IEnumerable<UserInfoResponse>>> GetUsersByStateAsync([FromRoute] string userState)
        {
            try
            {
                var users = await _userService.GetUsersByStateAsync(userState);

                var usersInfoResponse = _mapper.Map<IEnumerable<UserInfoResponse>>(users);

                return Ok(usersInfoResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("prosecutors")]
        public async Task<ActionResult<IEnumerable<ProsecutorUserInfoResponse>>> GetProsecutorUsersAsync()
        {
            try
            {
                var prosecutorUsers = await _userService.GetProsecutorUsersAsync();

                return Ok(prosecutorUsers);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
