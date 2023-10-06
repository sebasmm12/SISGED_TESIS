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
using Entities  = SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Validators;
using System.Text.Json;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class SolicitorDossierShipment
    {
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;
        [Inject]
        public SolicitorDossierShipmentValidator SolicitorDossierShipmentValidator { get; set; } = default!;

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;
        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;

        private bool pageLoading = true;
        private MudForm? solicitorDossierShipmentForm = default!;
        private SolicitorDossierShipmentRegisterDTO solicitorDossierShipment = new();
        private MudStepper? solicitorDossierShipmentStepper;
        private readonly List<MediaRegisterDTO> annexes = new();
        private IEnumerable<int> years = default!;
        private string dossierId = default!;
        private readonly string typeDocument = "Denuncia";

        protected override async Task OnInitializedAsync()
        {
            await GetComplaintRequestInformationAsync();
            await GetSolicitorDossierInformationAsync(solicitorDossierShipment.Solicitor.Id);

            pageLoading = false;
        }

        private async Task RegisterSolicitorDossierShipmentAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredSolicitorDossierShipment = await ShowLoadingDialogAsync(documentRegister);

            if (registeredSolicitorDossierShipment is null) return;

            await SwalFireRepository.ShowSuccessfulSwalFireAsync("Se pudo registrar el envío de expediente de manera exitosa");

            await UpdateRegisteredDocumentAsync(registeredSolicitorDossierShipment);
        }

        private async Task UpdateRegisteredDocumentAsync(Entities.SolicitorDossierShipment solicitorDossierShipment)
        {
            var inputItem = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace == "inputs");

            await ProcessWorkItemInfo(inputItem!, solicitorDossierShipment);

            await WorkEnvironment.UpdateRegisteredDocumentAsync(inputItem!);

        }

        private async Task ProcessWorkItemInfo(Item item, Entities.SolicitorDossierShipment solicitorDossierShipment)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(solicitorDossierShipment);

            dossierTray.DocumentObjects!.Add(documentResponse);
            dossierTray.Document = documentResponse;
            dossierTray.Type = typeDocument;

            Mapper.Map(dossierTray, item);
        }

        private DossierWrapper GetDocumentRegister()
        {
            var solicitorDossierShipmentContent = Mapper.Map<SolicitorDossierShipmentResponseContent>(solicitorDossierShipment);
            var solicitorDossierShipmentResponse = new SolicitorDossierShipmentResponse(solicitorDossierShipmentContent, annexes);

            var solicitorDossierShipmentRegister = new DossierWrapper(dossierId, solicitorDossierShipmentResponse);

            return solicitorDossierShipmentRegister;
        }

        private async Task<Entities.SolicitorDossierShipment?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de la entrega de expediente del notario, por favor espere...";

            var solicitorDossierShipmentRegister = () => RegisterSolicitorDossierShipmentAsync(documentRegister);

            return await DialogContentRepository.ShowLoadingDialogAsync(solicitorDossierShipmentRegister, dialogTitle);

        }

        private bool CheckSolicitorDossierShipmentRegisterAsync()
        {
            if (solicitorDossierShipmentStepper!.GetActiveIndex() != solicitorDossierShipmentStepper!.Steps.Count - 1) return false;

            solicitorDossierShipmentForm!.Validate().GetAwaiter().GetResult();

            if (!solicitorDossierShipmentForm.IsValid)
            {
                SwalFireRepository.ShowErrorSwalFireAsync("No se puede registrar la entrega de expediente del notario, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }
            
            RegisterSolicitorDossierShipmentAsync().GetAwaiter();

            return false;
        }

        private async Task GetComplaintRequestInformationAsync()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            var dossierTray = userTray.Value as DossierTrayResponse;

            var complaintRequest = dossierTray!.DocumentObjects!.First(document => document.Type == "SolicitudDenuncia");

            var complaintContent = JsonSerializer.Deserialize<SolicitorDossierRequestContentDTO>(JsonSerializer.Serialize(complaintRequest.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            solicitorDossierShipment.Solicitor = await GetSolicitorAsync(complaintContent!.SolicitorId);
            dossierId = dossierTray.DossierId;
        }

        private async Task GetSolicitorDossierInformationAsync(string solicitorId)
        {
            years = await GetSolicitorDossierYearsAsync(solicitorId);
        }

        private async Task<IEnumerable<int>> GetSolicitorDossierYearsAsync(string solicitorId)
        {
            try
            {
                var solicitorDossierYearsResponse = await HttpRepository.GetAsync<IEnumerable<int>>($"api/solicitorsDossiers/{ solicitorId }/years");

                if(solicitorDossierYearsResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los años de los expedientes del notario");
                }

                return solicitorDossierYearsResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                return new List<int>();
            }
        }

        
        private async Task<Entities.SolicitorDossierShipment?> RegisterSolicitorDossierShipmentAsync(DossierWrapper dossierWrapper)
        {
            try
            {
                var solicitorDossierShipment = await HttpRepository.PostAsync<DossierWrapper, Entities.SolicitorDossierShipment>("api/documents/solicitor-dossier-shipments", dossierWrapper);
                
                if(solicitorDossierShipment.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la entrega del expediente del notario");
                }

                return solicitorDossierShipment.Response!;

            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la entrega del expediente del notario");
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