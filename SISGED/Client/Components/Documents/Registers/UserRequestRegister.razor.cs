using AutoMapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.DossierDocument;
using SISGED.Shared.Models.Responses.Solicitor;
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

        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

        
        private MudForm? userRequestForm = default!;
        private IEnumerable<DocumentTypeInfoResponse> documentTypes = default!;
        private bool pageLoading = true;
        private UserRequestRegisterDTO userRequest = new();
        private List<MediaRegisterDTO> annexes = new();


        protected override async Task OnInitializedAsync()
        {
            documentTypes = await GetDocumentTypesAsync();
            pageLoading = false;
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

            await RegisterUserRequestDocumentAsync(documentRegister);
        }
        
        private DossierWrapper GetDocumentRegister()
        {
            var initialRequestContent = Mapper.Map<InitialRequestResponseContent>(userRequest);
            var initialRequest = new InitialRequestResponse(initialRequestContent, annexes);

            var documentRegister = new DossierWrapper(initialRequest);

            return documentRegister;
        }

        private async Task RegisterUserRequestDocumentAsync(DossierWrapper documentRegister)
        {
            try
            {
                var userRequestResponse = await HttpRepository.PostAsync("api/dossierdocument/user-requests", documentRegister);

                if (userRequestResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar su solicitud");
                }
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo registar su solicitud");
            }
        }

        private async Task<IEnumerable<DocumentTypeInfoResponse>> GetDocumentTypesAsync()
        {
            try
            {
                var documentTypesResponse = await HttpRepository.GetAsync<IEnumerable<DocumentTypeInfoResponse>>("api/documentTypes/solicitud");

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