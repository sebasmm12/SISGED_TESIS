using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Generics;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Requests.Assistants;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.DossierDocument;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.User;
using SISGED.Shared.Validators;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class UserRequestRegister
    {
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public UserRequestRegisterValidator UserRequestRegisterValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;

        [CascadingParameter] 
        public MudDialogInstance MudDialog { get; set; } = default!;
        [Parameter]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private MudForm? userRequestForm = default!;
        private IEnumerable<DocumentTypeInfoResponse> documentTypes = default!;
        private bool pageLoading = true;
        private readonly UserRequestRegisterDTO userRequest = new();
        private readonly List<MediaRegisterDTO> annexes = new();


        protected override async Task OnInitializedAsync()
        {
            documentTypes = await GetDocumentTypesAsync();
            pageLoading = false;
        }

        private void GetSolicitorResponse(AutocompletedSolicitorResponse AutocompletedSolicitorResponse)
        {
            userRequest.Solicitor = AutocompletedSolicitorResponse;
        }

        private void CancelRegister()
        {
            MudDialog.Cancel();
        }

        private async Task RegisterUserRequestAsync()
        {
            await userRequestForm!.Validate();

            if (!userRequestForm.IsValid) return;

            var documentRegister = GetDocumentRegister();

            var registeredInitialRequest = await ShowLoadingDialogAsync(documentRegister);

            if (registeredInitialRequest is null) return;

            await RegisterAssistantAsync(new(registeredInitialRequest.Dossier.Id, registeredInitialRequest.Dossier.Type));

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar su solicitud de manera satisfactoria");

            MudDialog.Close(DialogResult.Ok(true));
        }

        private async Task<DossierDocumentInitialRequestResponse?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su solicitud, por favor espere...";

            var userToRegister = () => RegisterUserRequestDocumentAsync(documentRegister);

            return await DialogContentRepository.ShowLoadingDialogAsync(userToRegister, dialogTitle);

        }
        private DossierWrapper GetDocumentRegister()
        {
            var initialRequestContent = Mapper.Map<InitialRequestResponseContent>(userRequest);
            var initialRequest = new InitialRequestResponse(initialRequestContent, annexes, 
               SessionAccount.GetClient().Name, SessionAccount.GetClient().LastName,
               SessionAccount.GetDocumentType() ,SessionAccount.GetDocumentNumber(), SessionAccount.GetUser().Id);
            
            var documentRegister = new DossierWrapper(initialRequest);

            return documentRegister;
        }

        private async Task<DossierDocumentInitialRequestResponse?> RegisterUserRequestDocumentAsync(DossierWrapper documentRegister)
        {
            try
            {
                var userRequestResponse = await HttpRepository.PostAsync<DossierWrapper, DossierDocumentInitialRequestResponse >("api/documents/user-requests", documentRegister);

                if (userRequestResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar su solicitud");
                }

                return userRequestResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar su solicitud");
                return null;
            }
        }

        private async Task RegisterAssistantAsync(AssistantCreateRequest assistantRequest)
        {
            try
            {
                var assistantResponse = await HttpRepository.PostAsync("api/assistants", assistantRequest);

                if(assistantResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo completar con el registro de su solicitud");
                }
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo completar con el registro de su solicitud");
            }
        }

        private async Task<IEnumerable<DocumentTypeInfoResponse>> GetDocumentTypesAsync()
        {
            try
            {
                var documentTypesResponse = await HttpRepository.GetAsync<IEnumerable<DocumentTypeInfoResponse>>("api/documentTypes?type=solicitud");

                if(documentTypesResponse.Error)
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

    }
}