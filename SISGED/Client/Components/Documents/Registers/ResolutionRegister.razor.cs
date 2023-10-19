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

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class ResolutionRegister
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;
        [Inject]
        public ResolutionRegisterValidator ResolutionRegisterValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository dialogContentRepository { get; set; } = default!;

        private MudForm? requestForm = default!;
        private MudStepper? requestStepper;

        //Variables de sesion
        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;

        //Datos del formulario
        private ResolutionRegisterDTO resolutionRegister = new();
        private IEnumerable<DocumentTypeInfoResponse> documentTypes = default!;

        private List<MediaRegisterDTO> annexes = new();
        private bool pageLoading = true;
        private string dossierId = default!;
        private readonly string typeDocument = "Denuncia";
        private string previousDocumentId = default!;

        protected override async Task OnInitializedAsync()
        {
            documentTypes = await GetDocumentTypesAsync();

            await GetUserRequestInformationAsync();

            pageLoading = false;
        }

        private DossierWrapper GetDocumentRegister()
        {
            var resolutionContent = Mapper.Map<ResolutionResponseContent>(resolutionRegister);
            var resolution = new ResolutionResponse(resolutionContent, annexes);

            var documentRegister = new DossierWrapper(dossierId, resolution, previousDocumentId);

            return documentRegister;
        }

        private async Task<Resolution?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su resolución, por favor espere...";

            var toRegister = () => RegisterResolutionAsync(documentRegister);

            return await dialogContentRepository.ShowLoadingDialogAsync(toRegister, dialogTitle);

        }

        private async Task RegisterResolutionAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredResolution = await ShowLoadingDialogAsync(documentRegister);

            if (registeredResolution is null) return;

            await swalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar la resolución de manera satisfactoria");

            await UpdateRegisteredDocumentAsync(registeredResolution);
        }

        private async Task UpdateRegisteredDocumentAsync(Resolution resolution)
        {
            var inputItem = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace != "tools");

            ProcessWorkItemInfo(inputItem!, resolution);

            await WorkEnvironment.UpdateRegisteredDocumentAsync(inputItem!);

        }

        private void ProcessWorkItemInfo(Item item, Resolution resolution)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(resolution);

            dossierTray.DocumentObjects!.Add(documentResponse);
            dossierTray.Document = documentResponse;
            dossierTray.Type = typeDocument;

            Mapper.Map(dossierTray, item);
        }

        private async Task<Resolution?> RegisterResolutionAsync(DossierWrapper documentRegister)
        {
            try
            {
                var resolutionResponse = await httpRepository.PostAsync<DossierWrapper, Resolution>("api/documents/resolutions", documentRegister);

                if (resolutionResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la resolución");
                }

                return resolutionResponse.Response!;
            }
            catch (Exception)
            {

                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la resolución");
                return null;
            }
        }

        private async Task<IEnumerable<DocumentTypeInfoResponse>> GetDocumentTypesAsync()
        {
            try
            {
                var documentTypesResponse = await httpRepository.GetAsync<IEnumerable<DocumentTypeInfoResponse>>("api/documentTypes?type=sancion");

                if (documentTypesResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de sanciones del sistema");
                }

                return documentTypesResponse.Response!;
            }
            catch (Exception)
            {
                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de sanciones del sistema");
                return new List<DocumentTypeInfoResponse>();
            }
        }

        private async Task GetUserRequestInformationAsync()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            resolutionRegister.Client = userTray.Client;

            var dossierTray = userTray.Value as DossierTrayResponse;

            var documentContent = JsonSerializer.Deserialize<DictumContentDTO>(JsonSerializer.Serialize(dossierTray!.Document!.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            resolutionRegister.Solicitor = await GetSolicitorAsync(documentContent!.SolicitorId);
            dossierId = dossierTray!.DossierId;
            previousDocumentId = dossierTray.Document!.Id;
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
                swalFireRepository.ShowErrorSwalFireAsync("No se puede registrar la resolución, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }

            RegisterResolutionAsync().GetAwaiter();

            return false;
        }
    }
}