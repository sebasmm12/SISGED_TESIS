using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MudBlazor;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Requests.Account;
using SISGED.Shared.Validators;
using System.Security.Cryptography;

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
        private bool pageLoading = true;

        private AccountLoginRequest GetFormMapped()
        {

            var userCredentials = Mapper.Map<AccountLoginRequest>(new UserLoginDTO { Username = userLogin.Username, Password = EncryptPassword(userLogin.Password).Password });

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

                await loginRepository.Login(loggedUser.Token);

                await swalFireRepository.ShowSuccessfulSwalFireAsync($"Se ha iniciado sesión.");

                navigationManager.NavigateTo("/");
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
                    await swalFireRepository.ShowErrorSwalFireAsync($"{httpResponse.HttpResponseMessage}");
                }
                return httpResponse.Response!;
            }
            catch (Exception)
            {
                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo verificar las credenciales.");
                return null;
            }
        }

        private static EncryptedPasswordDTO EncryptPassword(string password)
        {
            var salt = new byte[16];

            using var random = RandomNumberGenerator.Create();
            random.GetBytes(salt);

            return EncryptPassword(password, salt);
        }

        private static EncryptedPasswordDTO EncryptPassword(string password, byte[] salt)
        {
            var derivatedKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 1000, 32);

            var newPassword = Convert.ToBase64String(derivatedKey);

            return new(newPassword, Convert.ToBase64String(salt));
        }

    }
}