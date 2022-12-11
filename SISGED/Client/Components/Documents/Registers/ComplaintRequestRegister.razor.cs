using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudExtensions;
using MudExtensions.Utilities;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using Entities = SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Validators;
using System.Text.Json;
using SISGED.Shared.Models.Requests.Documents;
using AutoMapper;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Client.Generics;
using SISGED.Client.Services.Repositories;
using SISGED.Client.Components.WorkEnvironments;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class ComplaintRequestRegister
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public ComplaintRequestValidator ComplaintRequestValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;


        private bool pageLoading = true;
        private MudStepper? complaintRequestStepper;
        private IEnumerable<DocumentTypeInfoResponse> documentTypes = default!;
        private MudForm? complaintRequestForm = default!;
        private readonly ComplaintRequestRegisterDTO complaintRequest = new();
        private readonly List<MediaRegisterDTO> annexes = new();
        private string dossierId = default!;

        protected override async Task OnInitializedAsync()
        {
            documentTypes = await GetDocumentTypesAsync();

            await GetUserRequestInformationAsync();

            pageLoading = false;
        }

        private void GetSolicitorResponse(AutocompletedSolicitorResponse AutocompletedSolicitorResponse)
        {
            complaintRequest.Solicitor = AutocompletedSolicitorResponse;
        }

        private async Task RegisterComplaintRequestAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredComplaint = await ShowLoadingDialogAsync(documentRegister);

            if (registeredComplaint is null) return;

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar la denuncia de manera satisfactoria");

            UpdateRegisteredDocument(registeredComplaint);
        }

        private void UpdateRegisteredDocument(ComplaintRequest complaintRequest)
        {
            var inputItem = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace == "inputs");

            ProcessWorkItemInfo(inputItem!, complaintRequest);

            WorkEnvironment.UpdateRegisteredDocument(inputItem!);

        }

        private void ProcessWorkItemInfo(Item item, ComplaintRequest complaintRequest)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(complaintRequest);

            dossierTray.DocumentObjects!.Add(documentResponse);
            dossierTray.Document = documentResponse;
            dossierTray.Type = "Denuncia";

            Mapper.Map(dossierTray, item);
        }

        private bool CheckComplaintRegisterAsync()
        {
            if (complaintRequestStepper!.GetActiveIndex() != 2) return false;

            complaintRequestForm!.Validate().GetAwaiter().GetResult();

            if (!complaintRequestForm!.IsValid)
            {
                SwalFireRepository.ShowErrorSwalFireAsync("No se puede registrar la solicitud de Denuncia, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }

            RegisterComplaintRequestAsync().GetAwaiter();

            return false;
        }

        private async Task GetUserRequestInformationAsync()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            complaintRequest.Client = userTray.Client;

            var dossierTray = userTray.Value as DossierTrayResponse;

            var documentContent = JsonSerializer.Deserialize<InitialRequestContentDTO>(JsonSerializer.Serialize(dossierTray!.Document!.Content));

            complaintRequest.Solicitor = await GetSolicitorAsync(documentContent!.SolicitorId);
            complaintRequest.ComplaintType = GetComplaintType(complaintRequest.Client);
            dossierId = dossierTray.DossierId;
        }

        private DocumentTypeInfoResponse GetComplaintType(Entities.Client client)
        {
            if (client.DocumentType == "DNI") return documentTypes.First(document => document.Name == "Denuncia");

            return new();
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

        private async Task<IEnumerable<DocumentTypeInfoResponse>> GetDocumentTypesAsync()
        {
            try
            {
                var documentTypesResponse = await HttpRepository.GetAsync<IEnumerable<DocumentTypeInfoResponse>>("api/documentTypes/denuncia");

                if (documentTypesResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                }

                return documentTypesResponse.Response!;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                return new List<DocumentTypeInfoResponse>();
            }
        }

        private DossierWrapper GetDocumentRegister()
        {
            var complaintRequestContent = Mapper.Map<ComplaintRequestResponseContent>(complaintRequest);
            var complaint = new ComplaintRequestResponse(complaintRequestContent, annexes);

            var documentRegister = new DossierWrapper(dossierId, complaint);

            return documentRegister;
        }

        private async Task<ComplaintRequest?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su denuncia, por favor espere...";

            var complaintToRegister = () => RegisterComplaintRequestAsync(documentRegister);

            return await DialogContentRepository.ShowLoadingDialogAsync(complaintToRegister, dialogTitle);

        }

        private async Task<ComplaintRequest?> RegisterComplaintRequestAsync(DossierWrapper documentRegister)
        {
            try
            {
                var complaintResponse = await HttpRepository.PostAsync<DossierWrapper, ComplaintRequest>("api/documents/complaint-requests", documentRegister);
                
                if (complaintResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la solicitud de denuncia");
                }

                return complaintResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la solicitud de denuncia");
                return null;
            }
        }


        private static StepperLocalizedStrings GetRegisterLocalizedStrings()
        {
            return new() { Completed = "Completado", Finish = "Registrar", Next = "Siguiente", Previous = "Anterior" };
        }

    }
}