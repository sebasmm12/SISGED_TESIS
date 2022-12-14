using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudExtensions;
using MudExtensions.Utilities;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Solicitor;
using System.Text.Json;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class DictumRegister
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;
        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;

        private bool pageLoading = true;
        private MudForm? dictumForm = default!;
        private readonly DictumRegisterDTO dictum = new();
        private MudStepper? dictumStepper;
        private readonly List<MediaRegisterDTO> annexes = new();
        private string dossierId = default!;

        private async Task RegisterDictumAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredDictum = await ShowLoadingDialogAsync(documentRegister);

            if (registeredDictum is null) return;

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar el dictamen de manera satisfactoria");

            UpdateRegisteredDocument(registeredDictum);
        }

        private void UpdateRegisteredDocument(Dictum dictum)
        {
            var inputItem = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace == "inputs");

            ProcessWorkItemInfo(inputItem!, dictum);

            WorkEnvironment.UpdateRegisteredDocument(inputItem!);

        }

        private void ProcessWorkItemInfo(Item item, Dictum complaintRequest)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(complaintRequest);

            dossierTray.DocumentObjects!.Add(documentResponse);
            dossierTray.Document = documentResponse;
            dossierTray.Type = "Dictamen";

            Mapper.Map(dossierTray, item);
        }

        private bool CheckDictumRegisterAsync()
        {
            if (dictumStepper!.GetActiveIndex() != dictumStepper!.Steps.Count - 1) return false;

            dictumForm!.Validate().GetAwaiter().GetResult();

            if(!dictumForm.IsValid)
            {
                SwalFireRepository.ShowErrorSwalFireAsync("No se puede registrar el dictamen, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }

            RegisterDictumAsync().GetAwaiter();

            return false;
        }

        private DossierWrapper GetDocumentRegister()
        {
            var dictumContent = Mapper.Map<DictumResponseContent>(dictum);
            var complaint = new DictumResponse(dictumContent, annexes);

            var dictumRegister = new DossierWrapper(dossierId, complaint);

            return dictumRegister;
        }

        protected override async Task OnInitializedAsync()
        {
            await GetComplaintRequestInformationAsync();

            pageLoading = false;
        }

        private async Task<Dictum?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su denuncia, por favor espere...";

            var dictumToRegister = () => RegisterDictumAsync(documentRegister);

            return await DialogContentRepository.ShowLoadingDialogAsync(dictumToRegister, dialogTitle);

        }

        private async Task GetComplaintRequestInformationAsync()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            dictum.Client = userTray.Client;

            var dossierTray = userTray.Value as DossierTrayResponse;
            
            var complaintRequest = dossierTray!.DocumentObjects!.First(document => document.Type == "SolicitudDenuncia");

            var complaintContent = JsonSerializer.Deserialize<ComplaintRequestContentDTO>(JsonSerializer.Serialize(complaintRequest.Content));

            dictum.Solicitor = await GetSolicitorAsync(complaintContent!.SolicitorId);
            dossierId = dossierTray.DossierId;
        }

        private async Task<Dictum?> RegisterDictumAsync(DossierWrapper dossierWrapper)
        {
            try
            {
                var dictum = await HttpRepository.PostAsync<DossierWrapper, Dictum>("api/documents/dictums", dossierWrapper);

                if(dictum.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar el dictamen");
                }

                return dictum.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar el dictamen");
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