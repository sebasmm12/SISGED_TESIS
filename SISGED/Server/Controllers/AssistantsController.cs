﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Assistants;
using SISGED.Shared.Models.Requests.Step;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AssistantsController : ControllerBase
    {
        private readonly IAssistantService _assistantService;
        private readonly IStepService _stepService;
        private readonly IMapper _mapper;

        public AssistantsController(IAssistantService assistantService, IStepService stepService, IMapper mapper)
        {
            _assistantService = assistantService;
            _stepService = stepService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Assistant>> CreateAssistantAsync(AssistantCreateRequest assistantCreateRequest)
        {
            try
            {
                var assistant = new Assistant(assistantCreateRequest.DossierId, assistantCreateRequest.DossierName, new() { new(assistantCreateRequest.DossierName) });

                await CreateAssistantAsync(assistant);

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

                var assistantResponse = await _assistantService.UpdateAssistantStepStartDateAsync(new(assistant, stepStartDate, stepLimitDate));

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
        private async Task CreateAssistantAsync(Assistant assistant)
        {
            var steps = await _stepService.GetStepByDossierTypeAsync(assistant.DossierType);

            assistant.Steps = new() { new(steps.DossierName, steps.Documents) };
            assistant.DocumentType = assistant.GetFirstDocument();

            await _assistantService.CreateAsync(assistant);
        }

        private async Task<Assistant> UpdateAssistantStepAsync(Assistant assistant, AssistantUpdateRequest assistantUpdateRequest)
        {
            var assistantStepUpdateDTO = new AssistantStepUpdateDTO(DateTime.UtcNow.AddHours(-5), new(assistant.Step, assistant.DocumentType));

            if (assistant.DossierType != assistantUpdateRequest.DossierType) return await UpdateAssistantDossierAsync(new(assistant, assistantUpdateRequest.DossierType,
                                                                                                                           assistantUpdateRequest.DocumentType, assistantStepUpdateDTO.EndDate));
            VerifyAssistantsStepsAndDocuments(assistant, assistantUpdateRequest);

            _mapper.Map(assistant, assistantStepUpdateDTO);

            return await _assistantService.UpdateAssistantStepAsync(assistantStepUpdateDTO);
        }

        private static void VerifyAssistantsStepsAndDocuments(Assistant assistant, AssistantUpdateRequest assistantUpdateRequest)
        {
            if (!assistant.IsLastStep())
            {
                assistant.UpdateNextStep();

                return;
            }

            if(!assistant.IsLastDocument()) assistant.UpdateNextDocument(assistantUpdateRequest.DocumentType);

        }

        private async Task<Assistant> UpdateAssistantDossierAsync(AssistantDossierStepUpdateDTO assistantDossierStepUpdate)
        {
            var assistantDossierUpdateDTO = await FillAssistantDossierUpdateAsync(assistantDossierStepUpdate);

            await _assistantService.UpdateAssistantDossierAsync(assistantDossierStepUpdate.Assistant, assistantDossierUpdateDTO);

            return await _assistantService.UpdateAssistantLastDossierStepAsync(assistantDossierStepUpdate.Assistant);
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