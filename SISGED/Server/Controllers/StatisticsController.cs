using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("documents")]
        public async Task<ActionResult<IEnumerable<DocumentsByMonthAndAreaResponse>>> GetDocumentsByMonthAndAreaAsync([FromQuery] DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery)
        {
            try
            {
                IEnumerable<DocumentsByMonthAndAreaResponse> documents;

                if(string.IsNullOrEmpty(documentsByMonthAndAreaQuery.Area)) documents = await _statisticService.GetDocumentsByMonthAsync(documentsByMonthAndAreaQuery);
                else documents = await _statisticService.GetDocumentsByMonthAndAreaAsync(documentsByMonthAndAreaQuery);

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("documents/expired")]
        public async Task<ActionResult<IEnumerable<ExpiredDocumentsResponse>>> GetExpiredDocumentsByMonthAsync([FromQuery] DocumentsByMonthQuery documentsByMonthQuery)
        {
            try
            {
                var documents = await _statisticService.GetExpiredDocumentsByMonthAsync(documentsByMonthQuery);

                return Ok(documents);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("documents/state")]
        public async Task<ActionResult<IEnumerable<DocumentByStateResponse>>> GetDocumentsByStateAsync([FromQuery] DocumentsByStateQuery documentsByStateQuery)
        {
            try
            {
                var documents = await _statisticService.GetDocumentsByStateAsync(documentsByStateQuery);

                return Ok(documents);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
