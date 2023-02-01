using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using SISGED.Client;
using SISGED.Client.Generics;
using SISGED.Client.Shared;
using SISGED.Client.Helpers;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Components.SolicitorDossier;
using SISGED.Client.Components.Trays;
using SISGED.Shared.Models.Responses.Tray;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.PublicDeed;
using SISGED.Shared.Models.Responses.User;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.DTOs;
using MudBlazor;
using MudExtensions;
using MudExtensions.Enums;
using AutoMapper;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Validators;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Requests.Account;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.User;

namespace SISGED.Client.Pages.Auth
{
    public partial class Register
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;
        [Inject]
        public UserSelfRegisterValidator userSelfRegisterValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository dialogContentRepository { get; set; } = default!;
        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;
        [Inject]
        private ILoginRepository loginRepository { get; set; } = default!;

        private MudForm? requestForm = default!;
        private UserSelfRegisterDTO userSelfRegister = new UserSelfRegisterDTO();
        private IEnumerable<DocumentTypeInfoResponse> documentTypes = Enumerable.Empty<DocumentTypeInfoResponse>();
        private bool pageLoading = false;

        protected override async Task OnInitializedAsync()
        {
            documentTypes = await GetDocumentTypesAsync();

            userSelfRegister.DocumentType = documentTypes.FirstOrDefault();

            pageLoading = false;
        }

        private async Task<IEnumerable<DocumentTypeInfoResponse>> GetDocumentTypesAsync()
        {
            try
            {
                var documentTypesResponse = await httpRepository.GetAsync<IEnumerable<DocumentTypeInfoResponse>>("api/documentTypes?type=identidad");

                if (documentTypesResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de documentos del sistema");
                }

                return documentTypesResponse.Response!;
            }
            catch (Exception)
            {
                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de documentos del sistema");
                return new List<DocumentTypeInfoResponse>();
            }
        }

        private UserRegisterRequest GetFormMapped()
        {

            var userData = Mapper.Map<UserRegisterRequest>(userSelfRegister);

            userData.Type = "Cliente";
            userData.Rol = "5f1a4c95bf2a0a3a0c510f7c";
            userData.State = "activo";

            return userData;
        }

        private async Task RegisterAsync()
        {
            requestForm!.Validate().GetAwaiter().GetResult();

            if (requestForm!.IsValid)
            {
                var accountRegister = GetFormMapped();

                var registeredUser = await ShowLoadingDialogAsync(accountRegister);

                if (registeredUser is null) return;

                await loginRepository.Login(registeredUser);

                await swalFireRepository.ShowSuccessfulSwalFireAsync($"Se ha registrado como usuario.");

                navigationManager.NavigateTo("/", true);
            }
        }

        private async Task<UserToken?> ShowLoadingDialogAsync(UserRegisterRequest accountRegister)
        {
            string dialogTitle = $"Realizando el registro de sus datos, por favor espere...";

            var toRegister = () => RegisterAsync(accountRegister);

            return await dialogContentRepository.ShowLoadingDialogAsync(toRegister, dialogTitle);

        }

        private async Task<UserToken?> RegisterAsync(UserRegisterRequest userRegisterRequest)
        {
            try
            {
                var httpResponse = await httpRepository.PostAsync<UserRegisterRequest, UserToken>("api/accounts", userRegisterRequest);

                if (httpResponse.Error)
                {
                    var msg = await httpResponse.GetBodyAsync();
                    await swalFireRepository.ShowErrorSwalFireAsync($"{msg}");
                }
                return httpResponse.Response!;
            }
            catch (Exception)
            {
                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registrar sus datos en el sistema.");
                return null;
            }
        }
    }
}