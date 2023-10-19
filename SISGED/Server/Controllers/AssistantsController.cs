using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Generics.Step;
using SISGED.Shared.Models.Requests.Assistants;
using SISGED.Shared.Models.Requests.Step;
using DocumentStep = SISGED.Shared.Entities.DocumentStep;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AssistantsController : ControllerBase
    {
        private readonly IAssistantService _assistantService;
        private readonly IStepService _stepService;
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;
        private readonly IPermissionService _permissionService;

        public AssistantsController(
            IAssistantService assistantService, 
            IStepService stepService, 
            IMapper mapper, 
            IDocumentService documentService, 
            IPermissionService permissionService)
        {
            _assistantService = assistantService;
            _stepService = stepService;
            _mapper = mapper;
            _documentService = documentService;
            _permissionService = permissionService;
        }

        [HttpPost]
        public async Task<ActionResult<Assistant>> CreateAssistantAsync(AssistantCreateRequest assistantCreateRequest)
        {
            try
            {
                var assistant = new Assistant(assistantCreateRequest.DossierId, assistantCreateRequest.DossierName, new() { new(assistantCreateRequest.DossierName) });

                await CreateAssistantAsync(assistant);

                await UpdateDocumentDueDateAsync(assistant, assistantCreateRequest.Document);

                return Ok(assistant);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPut("steps/start-date")]
        public async Task<ActionResult<Assistant>> UpdateAssistantStepStartDateRequestAsync([FromBody] AssistantStepStartDateUpdateRequest assistantStepStartDateRequest)
        {
            try
            {
                var assistant = await _assistantService.GetAssistantAsync(assistantStepStartDateRequest.AssistantId);

                var stepStartDate = DateTime.UtcNow.AddHours(-5);
                var stepLimitDate = stepStartDate.AddDays(assistant.GetCurrentDocumentStep().Days);

                var assitantStepDto = new AssistantStepDTO(assistant.Step, assistant.DocumentType, assistant.DossierType);

                var assistantResponse = await _assistantService.UpdateAssistantStepStartDateAsync(new(assistant.Id, assitantStepDto, stepStartDate, stepLimitDate));

                return Ok(assistantResponse);

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
        public async Task<ActionResult<Assistant>> UpdateAssistantAsync([FromBody] AssistantUpdateRequest assistantUpdateRequest)
        {
            try
            {
                var assistant = await _assistantService.GetAssistantAsync(assistantUpdateRequest.Id);

                var updatedAssistant = await UpdateAssistantStepAsync(assistant, assistantUpdateRequest);

                return Ok(assistant);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{dossierId}")]
        public async Task<ActionResult<Assistant>> GetByDossierId([FromRoute] string dossierId)
        {
            try
            {
                var assistant = await _assistantService.GetAssistantByDossierAsync(dossierId);

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
        public async Task<ActionResult<Assistant>> UpdateFinallyAsync([FromBody] StepsUpdateRequest stepUpdateRequest)
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

        #region private methods

        private async Task UpdateDocumentDueDateAsync(Assistant assistant, RegisteredDocumentDTO document)
        {
            var documentSteps = assistant.GetCurrentDocumentSteps();

            var lastDocumentStep = documentSteps.Last(); 

            var isDocumentRegisterLastStepAction = await IsDocumentRegisterLastStepAction(lastDocumentStep);

            var totalDocumentProcessDays = documentSteps.Sum(documentStep => documentStep.Days);

            if(isDocumentRegisterLastStepAction)
                totalDocumentProcessDays -= lastDocumentStep.Days;

            document.SetDueDate(totalDocumentProcessDays);

            await _documentService.UpdateDocumentDueDateAsync(document);
        }

        private async Task<bool> IsDocumentRegisterLastStepAction(DocumentStep documentStep)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(documentStep.ActionId);

            return permission.Name.ToLower() == "registrardocumento";
        }

        private async Task CreateAssistantAsync(Assistant assistant)
        {
            var steps = await _stepService.GetStepByDossierTypeAsync(assistant.DossierType);

            assistant.Steps = new() { new(steps.DossierName, steps.Documents) };
            assistant.DocumentType = assistant.GetFirstDocument();

            await _assistantService.CreateAsync(assistant);
        }

        private async Task<Assistant> UpdateAssistantStepAsync(Assistant assistant, AssistantUpdateRequest assistantUpdateRequest)
        {
            var assistantStepUpdateDTO = new AssistantStepUpdateDTO(DateTime.UtcNow.AddHours(-5), new(assistant.Step, assistant.DocumentType, assistant.DossierType));

            if (assistant.DossierType != assistantUpdateRequest.DossierType)
            {
                var updatedAssistant = await UpdateAssistantDossierAsync(new(assistant, assistantUpdateRequest.DossierType,
                                                                             assistantUpdateRequest.DocumentType, assistantStepUpdateDTO.EndDate));

                await UpdateDocumentDueDateAsync(updatedAssistant, assistantUpdateRequest.Document!);

                return updatedAssistant;
            }
               
            await VerifyAssistantsStepsAndDocuments(assistant, assistantUpdateRequest, assistantStepUpdateDTO);

            _mapper.Map(assistant, assistantStepUpdateDTO);

            return await _assistantService.UpdateAssistantStepAsync(assistantStepUpdateDTO);
        }

        private async Task VerifyAssistantsStepsAndDocuments(Assistant assistant, AssistantUpdateRequest assistantUpdateRequest, AssistantStepUpdateDTO assistantStepUpdateDTO)
        {
            if (!assistant.IsLastDocumentStep())
            {
                assistant.UpdateNextStep();

                return;
            }

            if (assistant.IsLastDocument())
                return;
            

            await _assistantService.UpdateAssistantDocumentLastStepAsync(assistant.Id, assistantStepUpdateDTO.LastAssistantStep);

            assistant.UpdateNextDocument(assistantUpdateRequest.DocumentType);

            await MoveAssistantStepStartDatesAsync(assistant, assistantStepUpdateDTO.LastAssistantStep);

            assistantStepUpdateDTO.LastAssistantStep = new(assistant.Step, assistant.DocumentType, assistant.DossierType);

            assistant.UpdateNextDocumentStep();

            await UpdateDocumentDueDateAsync(assistant, assistantUpdateRequest.Document!);
        }

        private async Task MoveAssistantStepStartDatesAsync(Assistant assistant, AssistantStepDTO currentAssistantStep)
        {
            var assistantStep = new AssistantStepDTO(assistant.Step, assistant.DocumentType, assistant.DossierType);
            var assistantLastStep = assistant.GetDocumentStep(currentAssistantStep.DocumentType);

            await _assistantService.UpdateAssistantStepStartDateAsync(new(assistant.Id, assistantStep, assistantLastStep.StartDate!.Value, assistantLastStep.DueDate!.Value));
        }

        private async Task<Assistant> UpdateAssistantDossierAsync(AssistantDossierStepUpdateDTO assistantDossierStepUpdate)
        {
            var assistantDossierUpdateDTO = await FillAssistantDossierUpdateAsync(assistantDossierStepUpdate);

            await _assistantService.UpdateAssistantDossierAsync(assistantDossierStepUpdate.Assistant, assistantDossierUpdateDTO);

            var assitantStepDto = new AssistantStepDTO(assistantDossierStepUpdate.Assistant.Step, 
                                            assistantDossierStepUpdate.Assistant.DocumentType, assistantDossierStepUpdate.Assistant.DossierType);

            return await _assistantService.UpdateAssistantDocumentLastStepAsync(assistantDossierStepUpdate.Assistant.Id, assitantStepDto);
        }

        private async Task<AssistantDossierUpdateDTO> FillAssistantDossierUpdateAsync(AssistantDossierStepUpdateDTO assistantDossierStepUpdate)
        {
            var steps = await _stepService.GetStepByDossierTypeAsync(assistantDossierStepUpdate.DossierType);

            var firstStepDocument = steps.Documents
                                         .First();

            var firstStep = firstStepDocument
                                 .Steps
                                 .First();


            var currentStep = assistantDossierStepUpdate.Assistant.GetCurrentDocumentStep();

            FillStepDates(currentStep, firstStep);

            firstStep.EndDate = assistantDossierStepUpdate.StepEndDate;

            return new AssistantDossierUpdateDTO(assistantDossierStepUpdate.Assistant.Id, assistantDossierStepUpdate.DossierType
                                                                         , steps.GetFirstDocument().Type, new(assistantDossierStepUpdate.DossierType, steps.Documents),
                                                                         firstStepDocument.GetNextStepIndex(firstStep.Index));
        }

        private static void FillStepDates(DocumentStep currentStep, DocumentStep newStep)
        {
            newStep.StartDate = currentStep.StartDate;
            newStep.DueDate = currentStep.DueDate;
        }

        #endregion
    }
}
