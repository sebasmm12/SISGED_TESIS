using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.User;
using SISGED.Shared.Validators;
using System.Text.Json;

namespace SISGED.Client.Components.Documents
{
    public partial class DocumentEvaluation
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public DocumentEvaluationValidator DocumentEvaluationValidator { get; set; } = default!;

        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;
        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private bool pageLoading = true;
        private DossierTrayResponse dossierTray = default!;
        private Role? userRole;
        private UserInfoResponse? user; 
        private DocumentContentDTO documentContent = default!;
        private bool isApproved = true;
        private MudForm? documentEvaluationForm = default!;
        private DocumentEvaluationDTO documentEvaluation = new();

        protected async override Task OnInitializedAsync()
        {
            await GetDocumentEvaluationInfoAsync();

            await WorkEnvironment.UpdateAssistantMessageAsync(new(dossierTray.Type!, dossierTray.Document!.Type, 0));

            pageLoading = false;
        }

        private async Task AprroveDocumentAsync()
        {
            isApproved = true;
            documentEvaluation.EvaluatorComment = default!;

            await EvaluateDocumentAsync();
        }

        private void RejectDocumentAsync()
        {
            isApproved = false;
        }

        private async Task ConfirmDocumentEvaluationAsync()
        {
            await documentEvaluationForm!.Validate();

            if (!documentEvaluationForm.IsValid) return;

            await EvaluateDocumentAsync();
        }
        
        private async Task EvaluateDocumentAsync()
        {
            var swalFireInfo = GetSwalFireInfo(isApproved);
            bool updatedDocument = await SwalFireRepository.ShowLockSwalFireAsync(swalFireInfo);

            if (!updatedDocument) return;

            var evaluatedDocument = await ShowLoadingDialogAsync(new(isApproved, documentEvaluation.EvaluatorComment, dossierTray.Document!.Id));

            if (evaluatedDocument is null) return;

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo evaluar el documento de manera satisfactoria");

            await UpdateEvaluatedDocumentAsync(evaluatedDocument);
        }

        private async Task UpdateEvaluatedDocumentAsync(DocumentResponse documentResponse)
        {
            var item = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace != "tools");

            ProcessWorkItemInfo(item!, documentResponse);

            await WorkEnvironment.EvaluateDocumentAsync(item!, isApproved);
        }

        private void ProcessWorkItemInfo(Item item, DocumentResponse documentResponse)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            dossierTray.Document = documentResponse;
            dossierTray.DocumentObjects!.RemoveAt(dossierTray.DocumentObjects.Count - 1);
            dossierTray.DocumentObjects.Add(dossierTray.Document);

            Mapper.Map(dossierTray, item);
        }

        private static SwalFireInfo GetSwalFireInfo(bool isApproved)
        {
            string action = isApproved ? "aprobar" : "rechazar";
            string actionTitle = isApproved ? "Aprobación" : "Rechazo";

            string title = $"{actionTitle} del documento";
            string htmlContent = $"¿Está seguro que desea {action} el documento?";

            return new SwalFireInfo(title, htmlContent, SwalFireIcons.Warning, action);
        }

        private async Task<DocumentResponse?> ShowLoadingDialogAsync(DocumentEvaluationRequest documentEvaluationRequest)
        {
            string dialogTitle = $"Evaluando el documento, por favor espere...";

            var functionToExecute = () => EvaluateDocumentAsync(documentEvaluationRequest);

            return await DialogContentRepository.ShowLoadingDialogAsync(functionToExecute, dialogTitle);
        }

        private async Task GetDocumentEvaluationInfoAsync()
        {
            dossierTray = GetDossierTray();

            documentContent = JsonSerializer.Deserialize<DocumentContentDTO>(JsonSerializer.Serialize(dossierTray.Document!.Content))!;

            var lastProcess = dossierTray.Document!.ProcessesHistory.Last();

            await GetEvaluatorInfoAsync(lastProcess);

        }

        private async Task GetEvaluatorInfoAsync(Process process)
        {
            var userTask = GetUserAsync(process.SenderId);
            var userRoleTask = GetRoleAsync(process.Area!);

            await Task.WhenAll(userTask, userRoleTask);

            user = await userTask;
            userRole = await userRoleTask;
        }

        private DossierTrayResponse GetDossierTray()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            var dossierTray = userTray.Value as DossierTrayResponse;

            return dossierTray!;
        }

        private async Task<DocumentResponse?> EvaluateDocumentAsync(DocumentEvaluationRequest documentEvaluationRequest)
        {
            try
            {
                var documentResponse = await HttpRepository.PutAsync<DocumentEvaluationRequest, DocumentResponse?>("api/documents/evaluation", documentEvaluationRequest);
                
                if(documentResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo evaluar el documento");
                }

                return documentResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo evaluar el documento");
                return null;
            }
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

        private async Task<UserInfoResponse?> GetUserAsync(string userId)
        {
            try
            {
                var roleResponse = await HttpRepository.GetAsync<UserInfoResponse>($"api/users/{userId}");

                if (roleResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo obtener información sobre el usuario creado del documento");
                }

                return roleResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo obtener información sobre el usuario creado del documento");
                return null;
            }
        }

    }
}