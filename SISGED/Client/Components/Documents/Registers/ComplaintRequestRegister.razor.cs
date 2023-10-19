using AutoMapper;
using Microsoft.AspNetCore.Components;
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
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Validators;
using System.Text.Json;
using Entities = SISGED.Shared.Entities;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class ComplaintRequestRegister
    {
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
        private string previousDocumentId = default!;

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

            await UpdateRegisteredDocumentAsync(registeredComplaint);
        }

        private async Task UpdateRegisteredDocumentAsync(ComplaintRequest complaintRequest)
        {
            var inputItem = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace != "tools");

            ProcessWorkItemInfo(inputItem!, complaintRequest);

            await WorkEnvironment.UpdateRegisteredDocumentAsync(inputItem!);
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
            if (complaintRequestStepper!.GetActiveIndex() != complaintRequestStepper!.Steps.Count - 1) return false;

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

            var dossierTray = (DossierTrayResponse)userTray.Value;

            var documentContent = JsonSerializer.Deserialize<InitialRequestContentDTO>(JsonSerializer.Serialize(dossierTray!.Document!.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            complaintRequest.Solicitor = string.IsNullOrEmpty(documentContent!.SolicitorId) ? new() : await GetSolicitorAsync(documentContent!.SolicitorId);
            complaintRequest.ComplaintType = GetComplaintType(complaintRequest.Client);
            dossierId = dossierTray.DossierId;
            previousDocumentId = dossierTray.Document!.Id;
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
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener al notario registrado en la solicitud");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener al notario registrado en la solicitud");
                return new();
            }
        }

        private async Task<IEnumerable<DocumentTypeInfoResponse>> GetDocumentTypesAsync()
        {
            try
            {
                var documentTypesResponse = await HttpRepository.GetAsync<IEnumerable<DocumentTypeInfoResponse>>("api/documentTypes?type=denuncia");

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

            var documentRegister = new DossierWrapper(dossierId, complaint, previousDocumentId);

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