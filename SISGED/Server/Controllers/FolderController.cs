using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Folder>>> Get()
        {
            try
            {
                var folders = await _folderService.GetAsync();
 
                return Ok(folders);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
