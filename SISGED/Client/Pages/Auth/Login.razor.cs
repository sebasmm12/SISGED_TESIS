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
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Requests.Account;
using SISGED.Client.Components.Documents.Registers;
using static MudBlazor.CategoryTypes;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text.Json;

namespace SISGED.Client.Pages.Auth
{
    public partial class Login
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;
        [Inject]
        public UserLoginValidator userLoginValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository dialogContentRepository { get; set; } = default!;
        [Inject]
        private ILoginRepository loginRepository { get; set; } = default!;
        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;

        private MudForm? requestForm = default!;
        private UserLoginDTO userLogin = new UserLoginDTO();
        private bool pageLoading = false;

        private AccountLoginRequest GetFormMapped()
        {

            var userCredentials = Mapper.Map<AccountLoginRequest>(userLogin);

            return userCredentials;
        }

        private async Task LoginAsync()
        {
            requestForm!.Validate().GetAwaiter().GetResult();

            if (requestForm!.IsValid)
            {
                var accountLogin = GetFormMapped();

                var loggedUser = await ShowLoadingDialogAsync(accountLogin);

                if (loggedUser is null) return;

                await loginRepository.Login(loggedUser);

                await swalFireRepository.ShowSuccessfulSwalFireAsync($"Se ha iniciado sesión.");

                navigationManager.NavigateTo("/",true);
            }
        }

        private async Task<UserToken?> ShowLoadingDialogAsync(AccountLoginRequest userInfoResponse)
        {
            string dialogTitle = $"Realizando la validación de las credenciales, por favor espere...";

            var toLogin = () => LoginAsync(userInfoResponse);

            return await dialogContentRepository.ShowLoadingDialogAsync(toLogin, dialogTitle);

        }

        private async Task<UserToken?> LoginAsync(AccountLoginRequest accountLoginRequest)
        {
            try
            {
                var httpResponse = await httpRepository.PostAsync<AccountLoginRequest, UserToken>("api/accounts/login", accountLoginRequest);

                if (httpResponse.Error)
                {
                    var msg = await httpResponse.GetBodyAsync();
                    await swalFireRepository.ShowErrorSwalFireAsync($"{msg}");
                }
                return httpResponse.Response!;
            }
            catch (Exception)
            {
                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo verificar las credenciales.");
                return null;
            }
        }
    }
}