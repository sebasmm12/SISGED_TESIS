using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Helpers;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.PublicDeed;
using SISGED.Shared.Models.Responses.PublicDeed;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PublicDeedsController : ControllerBase
    {
        private readonly IPublicDeedsService _publicdeedsService;

        public PublicDeedsController(IPublicDeedsService publicdeedsService)
        {
            _publicdeedsService = publicdeedsService;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<PublicDeed>>> autocompletefilter([FromQuery] string term)
        {
            try
            {
                var deedList = await _publicdeedsService.Filter(term);
                return Ok(deedList);
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("filterespecial")]
        public async Task<ActionResult<IEnumerable<PublicDeedFilterResponse>>> autocompleteFilterCompleto([FromQuery] PublicDeedSearchParametersFullFilterRequest searchparameters)
        {
            try
            {
                var filteredPublicDeedsList = await _publicdeedsService.SpecialFilter(searchparameters);
                await HttpContext.InsertPagedParameterOnResponse(filteredPublicDeedsList.AsQueryable(), searchparameters.RecordsQuantity);
                var paginaredFilteredPublicDeedsList =
                    filteredPublicDeedsList.AsQueryable().Paginate(searchparameters.Pagination).ToList();
                return Ok(paginaredFilteredPublicDeedsList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("id")]
        public async Task<ActionResult<PublicDeed>> GetById([FromQuery] string id)
        {
            try
            {
                var escrituraPublica = await _publicdeedsService.GetById(id);
                return Ok(escrituraPublica);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
