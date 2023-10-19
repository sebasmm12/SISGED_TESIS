using AutoMapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions;
using MudExtensions.Utilities;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Validators;
using System.Text.Json;
using SISGED.Client.Helpers;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class SessionResolutionRegister
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; }
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;
        [Inject]
        public SessionResolutionValidator SessionResolutionValidator { get; set; }

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;
        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;

        private bool pageLoading = true;
        private MudForm? sessionResolutionForm = default!;
        private readonly SessionResolutionRegisterDTO sessionResolution = new();
        private MudStepper? sessionResolutionStepper;
        private readonly List<MediaRegisterDTO> annexes = new();
        private string dossierId = default!;
        private string dossierType = "Denuncia";
        private string previousDocumentId = default!;

        protected override async Task OnInitializedAsync()
        {
            await GetSessionResolutionInformationAsync();

            pageLoading = false;
        }

        private bool CheckSessionResolutionRegisterAsync()
        {
            if (sessionResolutionStepper!.GetActiveIndex() != sessionResolutionStepper!.Steps.Count - 1) 
                return false;

            sessionResolutionForm!.Validate().GetAwaiter().GetResult();

            if (!sessionResolutionForm.IsValid)
            {
                SwalFireRepository.ShowErrorSwalFireAsync("No se puede registrar la resolución de la sesión, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }

            RegisterSessionResolutionAsync().GetAwaiter();

            return false;

        }

        private async Task RegisterSessionResolutionAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredSessionResolution = await ShowLoadingDialogAsync(documentRegister);

            if (registeredSessionResolution is null)
                return;

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar la resolución de sesión de manera satisfactoria");

            await UpdateRegisteredDocumentAsync(registeredSessionResolution);
        }

        private async Task UpdateRegisteredDocumentAsync(SessionResolution sessionResolution)
        {
            var inputItem = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace != "tools");

            ProcessWorkItemInfo(inputItem!, sessionResolution);

            await WorkEnvironment.UpdateRegisteredDocumentAsync(inputItem!);

        }

        private void ProcessWorkItemInfo(Item item, SessionResolution sessionResolution)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(sessionResolution);

            dossierTray.DocumentObjects!.Add(documentResponse);
            dossierTray.Document = documentResponse;
            dossierTray.Type = dossierType;

            Mapper.Map(dossierTray, item);
        }

        private DossierWrapper GetDocumentRegister()
        {
            var sessionResolutionContent = Mapper.Map<SessionResolutionResponseContent>(sessionResolution);
            var complaint = new SessionResolutionResponse(sessionResolutionContent, annexes);

            var sessionResolutionRegister = new DossierWrapper(dossierId, complaint, previousDocumentId);

            return sessionResolutionRegister;
        }

        private async Task<SessionResolution?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su resolución de sesión, por favor espere...";

            var sessionResolutionToRegister = () => RegisterSessionResolutionAsync(documentRegister);

            return await DialogContentRepository.ShowLoadingDialogAsync(sessionResolutionToRegister, dialogTitle);
        }

        private async Task GetSessionResolutionInformationAsync()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            sessionResolution.Client = userTray.Client;

            var dossierTray = userTray.Value as DossierTrayResponse;

            var complaintRequest = dossierTray!.DocumentObjects!.First(document => document.Type == "SolicitudDenuncia");

            var complaintContent = JsonSerializer.Deserialize<SolicitorDossierRequestContentDTO>(JsonSerializer.Serialize(complaintRequest.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            sessionResolution.Solicitor = await GetSolicitorAsync(complaintContent!.SolicitorId);
            dossierId = dossierTray.DossierId;

            sessionResolution.DocumentContent = JsonSerializer.Deserialize<DocumentContentDTO>(JsonSerializer.Serialize(dossierTray.Document!.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            sessionResolution.PreviousDocumentId = dossierTray.Document!.Id;
            previousDocumentId = sessionResolution.PreviousDocumentId;
        }

        private async Task<SessionResolution?> RegisterSessionResolutionAsync(DossierWrapper dossierWrapper)
        {
            try
            {
                var sessionResolution = await HttpRepository.PostAsync<DossierWrapper, SessionResolution>("api/documents/session-resolutions", dossierWrapper);

                if (sessionResolution.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la resolución de la sesión");
                }

                return sessionResolution.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la resolución de la sesión");
                return null;
            }
        }

        private async Task<AutocompletedSolicitorResponse> GetSolicitorAsync(string solicitorId)
        {
            try
            {
                var solicitorResponse = await HttpRepository.GetAsync<AutocompletedSolicitorResponse>($"api/solicitors/{solicitorId}");

                if (solicitorResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                return new();
            }
        }

        private static StepperLocalizedStrings GetRegisterLocalizedStrings()
        {
            return new() { Completed = "Completado", Finish = "Registrar", Next = "Siguiente", Previous = "Anterior" };
        }
    }
}
