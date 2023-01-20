using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.User;
using MudBlazor;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using System.Text.Json;
using SISGED.Shared.Validators;
using SISGED.Shared.DTOs;
using AutoMapper;
using SISGED.Shared.Models.Responses.Solicitor;
using MudExtensions;
using MudExtensions.Utilities;
using SISGED.Client.Components.WorkEnvironments;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class DisciplinaryOpennessRegister
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;
        [Inject]
        public DisciplinaryOpennessRegisterValidator disciplinaryOpennessRegisterValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository dialogContentRepository { get; set; } = default!;

        private MudForm? requestForm = default!;
        private MudStepper? requestStepper;

        //Variables de sesion
        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;
        [CascadingParameter(Name = "SessionAccount")] protected SessionAccountResponse SessionAccount { get; set; }
        
        //Datos del formulario
        private DisciplinaryOpennessRegisterDTO disciplinaryOpennessRegister = new DisciplinaryOpennessRegisterDTO();

        private bool pageLoading = false;
        String typeDocument = "AperturamientoDisciplinario";
        private List<MediaRegisterDTO> annexes = new();
        private string dossierId = default!;

        List<ProsecutorUserInfoResponse> prosecutors { get; set; } = new List<ProsecutorUserInfoResponse>();

        protected override async Task OnInitializedAsync()
        {
            await GetUserRequestInformationAsync();

           prosecutors = await GetProsecutorInformationAsync();

            pageLoading = false;

        }

        private DossierWrapper GetDocumentRegister()
        {
            var disciplinaryOpennessContent = Mapper.Map<DisciplinaryOpennessResponseContent>(disciplinaryOpennessRegister);
            var disciplinaryOpenness = new DisciplinaryOpennessResponse(disciplinaryOpennessContent, annexes);

            var documentRegister = new DossierWrapper(dossierId, disciplinaryOpenness);

            return documentRegister;
        }

        private async Task<DisciplinaryOpenness?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su aperturamiento disciplinario, por favor espere...";

            var toRegister = () => RegisterDisciplinaryOpennessRequestAsync(documentRegister);

            return await dialogContentRepository.ShowLoadingDialogAsync(toRegister, dialogTitle);

        }

        private async Task RegisterDisciplinaryOpennessRequestAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredDisciplinary = await ShowLoadingDialogAsync(documentRegister);

            if (registeredDisciplinary is null) return;

            await swalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar el aperturamiento disciplinario de manera satisfactoria");
            UpdateRegisteredDocument(registeredDisciplinary);
        }

        private void UpdateRegisteredDocument(DisciplinaryOpenness disciplinaryOpenness)
        {
            var inputItem = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace == "inputs");

            ProcessWorkItemInfo(inputItem!, disciplinaryOpenness);

            WorkEnvironment.UpdateRegisteredDocument(inputItem!);

        }

        private void ProcessWorkItemInfo(Item item, DisciplinaryOpenness disciplinaryOpenness)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(disciplinaryOpenness);

            dossierTray.DocumentObjects!.Add(documentResponse);
            dossierTray.Document = documentResponse;
            dossierTray.Type = typeDocument;

            Mapper.Map(dossierTray, item);
        }

        private async Task<DisciplinaryOpenness?> RegisterDisciplinaryOpennessRequestAsync(DossierWrapper documentRegister)
        {
            try
            {
                var disciplinaryResponse = await httpRepository.PostAsync<DossierWrapper, DisciplinaryOpenness>("api/documents/disciplinary-openness", documentRegister);

                if (disciplinaryResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar el aperturamiento disciplinario");
                }

                return disciplinaryResponse.Response!;
            }
            catch (Exception)
            {

                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar el aperturamiento disciplinario");
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
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            disciplinaryOpennessRegister.Client = userTray.Client;

            var dossierTray = userTray.Value as DossierTrayResponse;

            var documentContent = JsonSerializer.Deserialize<InitialRequestContentDTO>(JsonSerializer.Serialize(dossierTray!.Document!.Content)); //ComplaintRequestContentDTO

            disciplinaryOpennessRegister.Solicitor = await GetSolicitorAsync(documentContent!.SolicitorId);
            dossierId = dossierTray!.DossierId;
        }

        private async Task<List<ProsecutorUserInfoResponse>> GetProsecutorInformationAsync()
        {
            try
            {
                var solicitorResponse = await httpRepository.GetAsync<List<ProsecutorUserInfoResponse>>($"api/users/prosecutors");

                if (solicitorResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los fiscales del sistema");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los fiscales del sistema");
                return new();
            }
        }

        private void GetSolicitorResponse(AutocompletedSolicitorResponse AutocompletedSolicitorResponse)
        {
            disciplinaryOpennessRegister.Solicitor = AutocompletedSolicitorResponse;
        }

        private static StepperLocalizedStrings GetRegisterLocalizedStrings()
        {
            return new() { Completed = "Completado", Finish = "Registrar", Next = "Siguiente", Previous = "Anterior" };
        }

        private bool CheckRegisterAsync()
        {
            if (requestStepper!.GetActiveIndex() != requestStepper!.Steps.Count - 1) return false;

            requestForm!.Validate().GetAwaiter().GetResult();

            if (!requestForm!.IsValid)
            {
                swalFireRepository.ShowErrorSwalFireAsync("No se puede registrar el aperturamiento disciplinario, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }

            RegisterDisciplinaryOpennessRequestAsync().GetAwaiter();

            return false;
        }
    }
}