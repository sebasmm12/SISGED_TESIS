using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Shared.Models.Responses.DossierTray;
using MudBlazor;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using Newtonsoft.Json;
using SISGED.Shared.Validators;
using SISGED.Shared.DTOs;
using AutoMapper;
using SISGED.Client.Helpers;
using MudExtensions;
using MudExtensions.Utilities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class SolicitorDossierRequestRegister
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;
        [Inject]
        public SolicitorDossierRequestRegisterValidator SolicitorDossierRequestRegisterValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository dialogContentRepository { get; set; } = default!;

        private MudForm? requestForm = default!;
        private MudStepper? requestStepper;

        //Variables de sesion
        [CascadingParameter(Name = "WorkPlaceItems")]
        public List<Item> WorkPlaceItems { get; set; } = default!;
        [CascadingParameter(Name = "SessionAccount")] protected SessionAccountResponse SessionAccount { get; set; }


        //Datos del formulario
        private SolicitorDossierRequestRegisterDTO solicitorDossierRequestRegister = default!;
        private List<MediaRegisterDTO> annexes = new();
        private bool pageLoading = true;
        private string dossierId = default!;

        String typeDocument = "SolicitudExpedienteNotario";

        protected override async Task OnInitializedAsync()
        {
            await GetUserRequestInformationAsync();

            pageLoading = false;
        }


        private DossierWrapper GetDocumentRegister()
        {
            var solicitorDossierRequestContent = Mapper.Map<SolicitorDossierRequestResponseContent>(solicitorDossierRequestRegister);
            var solicitorDossier = new SolicitorDossierRequestResponse(solicitorDossierRequestContent, annexes);

            var documentRegister = new DossierWrapper(dossierId, solicitorDossier);

            return documentRegister;
        }

        private async Task<SolicitorDossierRequest?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su denuncia, por favor espere...";

            var toRegister = () => RegisterSolicitorDossierRequestAsync(documentRegister);

            return await dialogContentRepository.ShowLoadingDialogAsync(toRegister, dialogTitle);

        }

        private async Task RegisterSolicitorDossierRequestAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredSolicitorDossier = await ShowLoadingDialogAsync(documentRegister);

            if (registeredSolicitorDossier is null) return;

            await swalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar la solicitud de expediente de notario de manera satisfactoria");
        }

        private async Task<SolicitorDossierRequest?> RegisterSolicitorDossierRequestAsync(DossierWrapper documentRegister)
        {
            try
            {
                var solicitorDossierResponse = await httpRepository.PostAsync<DossierWrapper, SolicitorDossierRequest>("api/documents/solicitor-dossier-request", documentRegister);

                if (solicitorDossierResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la solicitud de expediente de notario");
                }

                return solicitorDossierResponse.Response!;
            }
            catch (Exception)
            {

                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la solicitud de expediente de notario");
                return null;
            }
        }

        private async Task<AutocompletedSolicitorResponse> GetSolicitorAsync(string solicitorId)
        {
            try
            {
                var solicitorResponse = await httpRepository.GetAsync<AutocompletedSolicitorResponse>($"api/solicitors/{solicitorId}");

                if (solicitorResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                return new();
            }
        }

        private async Task GetUserRequestInformationAsync()
        {
            var userTray = WorkPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            solicitorDossierRequestRegister.Client = userTray.Client;

            var dossierTray = userTray.Value as DossierTrayResponse;

            //var documentContent = JsonSerializer.Deserialize<InitialRequestContentDTO>(JsonSerializer.Serialize(dossierTray!.Document!.Content));

            //solicitorDossierRequestRegister.Solicitor = await GetSolicitorAsync(documentContent!.SolicitorId);
            dossierId = dossierTray!.DossierId;
        }

        private void GetSolicitorResponse(AutocompletedSolicitorResponse AutocompletedSolicitorResponse)
        {
            solicitorDossierRequestRegister.Solicitor = AutocompletedSolicitorResponse;
        }

        private static StepperLocalizedStrings GetRegisterLocalizedStrings()
        {
            return new() { Completed = "Completado", Finish = "Registrar", Next = "Siguiente", Previous = "Anterior" };
        }

        private bool CheckRegisterAsync()
        {
            if (requestStepper!.GetActiveIndex() != 1) return false;

            requestForm!.Validate().GetAwaiter().GetResult();

            if (!requestForm!.IsValid)
            {
                swalFireRepository.ShowErrorSwalFireAsync("No se puede registrar la solicitud de expediente de notario, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }

            RegisterSolicitorDossierRequestAsync().GetAwaiter();

            return false;
        }
    }
}