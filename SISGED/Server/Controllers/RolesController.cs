using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Responses.Role;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RolesController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleInfoResponse>>> GetRolesAsync()
        {
            try
            {
                var roles = await _roleService.GetRolesAsync();

                var rolesInfoResponse = _mapper.Map<IEnumerable<RoleInfoResponse>>(roles);

                return Ok(rolesInfoResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<RoleInfoResponse>> GetRoleByAsync([FromRoute] string roleId)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(roleId);

                var roleInfoResponse = _mapper.Map<RoleInfoResponse>(role);

                return Ok(roleInfoResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
