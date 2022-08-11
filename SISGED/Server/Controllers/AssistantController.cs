using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Assistant;
using SISGED.Shared.Models.Requests.Step;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AssistantController : ControllerBase
    {
        private readonly IAssistantService _assistantService;
        private readonly IMapper _mapper;

        public AssistantController(IAssistantService assistantService, IMapper mapper)
        {
            _assistantService = assistantService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Assistant>> Create(AssistantCreateRequest assistantCreateRequest)
        {
            try
            {
                var assistant = new Assistant()
                {
                    DossierId = assistantCreateRequest.DossierId,
                    Steps = new AssistantStep() { DossierName = assistantCreateRequest.DossierName }
                };

                assistant = await _assistantService.CreateAsync(assistant);

                return Ok(assistant);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPut("Initial")]
        public async Task<ActionResult<Assistant>> UpdateInitialRequestAsync(Assistant assistant, [FromQuery] String dossierName)
        {
            try
            {
                assistant = await _assistantService.UpdateInitialRequestAsync(assistant, dossierName);

                return Ok(assistant);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPut]
        public async Task<ActionResult<Assistant>> Update([FromBody] StepsUpdateRequest stepUpdateRequest)
        {
            try
            {
                var assistant = await _assistantService.UpdateAsync(stepUpdateRequest);
                return Ok(assistant);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{dossierId}")]
        public async Task<ActionResult<Assistant>> GetByDossierId([FromQuery] String dossierId)
        {
            try
            {
                var assistant = await _assistantService.GetAssistantAsync(dossierId);

                return assistant;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("Normal")]
        public async Task<ActionResult<Assistant>> UpdateNormalAsync([FromBody] StepsUpdateRequest stepUpdateRequest)
        {
            try
            {
                var assistant = await _assistantService.UpdateNormalAsync(stepUpdateRequest);
                return Ok(assistant);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("Finally")]
        public async Task<ActionResult<Assistant>> UpdateFinally([FromBody] StepsUpdateRequest stepUpdateRequest)
        {
            try
            {
                var assistant = await _assistantService.UpdateNormalAsync(stepUpdateRequest);
                return Ok(assistant);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
