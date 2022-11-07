using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;
using SISGED.Shared.Models.Responses.Step;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class StepsController : ControllerBase
    {
        private readonly IStepService _stepService;
        public StepsController(IStepService stepService)
        {
            _stepService = stepService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Step>>> GetStepsAsync()
        {
            try
            {
                var steps = await _stepService.GetStepsAsync();

                return Ok(steps);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("{stepId}")]
        public async Task<ActionResult<Step>> GetStepByIdAsync([FromRoute] string stepId)
        {
            try
            {
                var step = await _stepService.GetStepByIdAsync(stepId);

                return Ok(step);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpGet("dossiers")]
        public async Task<ActionResult<List<DossierStepsResponse>>> GetStepRequestAsync()
        {
            try
            {

                var dossiersSteps = await _stepService.GetStepRequestAsync();

                return Ok(dossiersSteps);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public async Task<ActionResult> RegisterStepAsync(StepRegisterRequest stepsRequest)
        {
            try
            {
                await _stepService.RegisterStepAsync(stepsRequest);

                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpPut]
        public async Task<ActionResult> UpdateStepAsync(StepUpdateRequest stepUpdateRequest)
        {
            try
            {
                await _stepService.UpdateStepAsync(stepUpdateRequest);

                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}
