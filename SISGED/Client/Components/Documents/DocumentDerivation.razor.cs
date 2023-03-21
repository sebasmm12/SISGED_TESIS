using AutoMapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Dossier;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Tray;
using SISGED.Shared.Validators;

namespace SISGED.Client.Components.Documents
{
    public partial class DocumentDerivation
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;
        [Inject]
        public DocumentDerivationValidator DocumentDerivationValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;

        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;
        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private bool pageLoading = true;
        private Role? userRole = default!;
        private readonly string currentDate = DateTime.UtcNow.AddHours(-5).ToString("dd/MM/yyyy");
        // TODO: Get the roleId based on the derivation step from the helper
        private string roleId = "5eeaf91a8ca4ff53a0b791eb";
        private DocumentDerivationDTO documentDerivation = new();
        private MudForm? documentDerivationForm = default!;


        protected override async Task OnInitializedAsync()
        {
            await GetUserInformationAsync();

            pageLoading = false;
        }

        private void UpdateUserTray(UserTrayResponse userTrayResponse)
        {
            documentDerivation.UserTray = userTrayResponse;
        }

        private async Task GetUserInformationAsync()
        {
            var userRoleTask = GetRoleAsync(SessionAccount.GetUser().Rol);
            var receiverUserRoleTask = GetRoleAsync(roleId);

            await Task.WhenAll(userRoleTask, receiverUserRoleTask);

            userRole = await userRoleTask;
            documentDerivation.ReceiverUserRole = await receiverUserRoleTask;
        }


        private async Task<Role?> GetRoleAsync(string roleId)
        {
            try
            {
                var roleResponse = await HttpRepository.GetAsync<Role>($"api/accounts/roles/{roleId}");

                if (roleResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo obtener información sobre su rol");
                }

                return roleResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo obtener información sobre su rol");
                return null;
            }
        }

        private async Task SendDocumentAsync()
        {
            await documentDerivationForm!.Validate();

            if (!documentDerivationForm.IsValid) return;

            var documentDerivationRequest = GetDossierLasDocument();

            var dossierDocument = await ShowLoadingDialogAsync(documentDerivationRequest, documentDerivation.UserTray.UserId);

            if (dossierDocument is null) return;

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo derivar el documento de manera satisfactoria");

            await UpdateSentDocumentAsync(dossierDocument);
        }

        private async Task UpdateSentDocumentAsync(DossierLastDocumentResponse dossierDocument)
        {
            var item = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace != "tools");

            ProcessWorkItemInfo(item!, dossierDocument);

            await WorkEnvironment.SendDocumentAsync(item!);
        }

        private void ProcessWorkItemInfo(Helpers.Item item, DossierLastDocumentResponse dossierDocument)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(dossierDocument.LastDocument);

            dossierTray.Document = documentResponse;
            dossierTray.DocumentObjects!.RemoveAt(dossierTray.DocumentObjects.Count - 1);
            dossierTray.DocumentObjects.Add(dossierTray.Document);

            Mapper.Map(dossierTray, item);
        }


        private DossierLastDocumentRequest GetDossierLasDocument()
        {
            var dossierTray = GetDossierTray();

            var derivation = new Derivation(userRole!.Id, documentDerivation.ReceiverUserRole!.Id,
                                            SessionAccount.User.Id, "derivado", dossierTray.Document!.Type);

            return new(dossierTray.DossierId, dossierTray.Document!.Id, derivation);
        }

        private DossierTrayResponse GetDossierTray()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            var dossierTray = userTray.Value as DossierTrayResponse;

            return dossierTray!;
        }

        private async Task<DossierLastDocumentResponse?> ShowLoadingDialogAsync(DossierLastDocumentRequest dossierLastDocumentRequest, string userId)
        {
            string dialogTitle = $"Realizando la derivación del documento, por favor espere...";

            var complaintToRegister = () => RegisterDerivationAsync(dossierLastDocumentRequest, userId);

            return await DialogContentRepository.ShowLoadingDialogAsync(complaintToRegister, dialogTitle);

        }

        private async Task<DossierLastDocumentResponse?> RegisterDerivationAsync(DossierLastDocumentRequest dossierLastDocumentRequest, string userId)
        {
            try
            {
                var documentDerivationResponse = await HttpRepository.PostAsync<DossierLastDocumentRequest, DossierLastDocumentResponse>($"api/dossiers/derivations/{userId}", dossierLastDocumentRequest);

                if (documentDerivationResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo derivar el documento");
                }

                return documentDerivationResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo derivar el documento");
                return null;
            }
        }


    }
}