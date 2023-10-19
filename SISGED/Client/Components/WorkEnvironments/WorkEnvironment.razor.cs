using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Components.VirtualHelpers;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Assistants;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Client.Components.WorkEnvironments
{
    public partial class WorkEnvironment
    {
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        public InputOutputTrayResponse UserTray { get; set; } = default!;
        public DossierTrayResponse SelectedTray { get; set; } = default!;
        private List<Item> Items { get; set; } = new();
        private bool isRendered = false;
        public readonly List<Item> workPlaceItems = new();

        private bool CanReorder => workPlaceItems.Count > 0;
        private string assistantMessage = "Seleccione un expediente de las bandejas al área de trabajo para procesarlo";
        private Assistant assistant = default!;
        private VirtualHelper? virtualHelper;

        protected override async Task OnInitializedAsync()
        {
            UserTray = await GetUserTrayAsync(SessionAccount.User.Id);
            GetItems();

            isRendered = true;
        }

        private void GetItems()
        {
            Items.AddRange(GetTools(SessionAccount.ToolPermissions));
            Items.AddRange(GetTrays(UserTray.InputDossier, "inputs"));
            Items.AddRange(GetTrays(UserTray.OutputDossier, "outputs"));
        }

        private static List<Item> GetTools(List<Permission> permissions)
        {
            return permissions.Select(permission => new Item()
            {
                Name = permission.Label,
                Value = permission.Id,
                Icon = permission.Icon,
                Description = string.Empty,
                CurrentPlace = "tools",
                OriginPlace = "tools"
            }).ToList();
        }

        private static List<Item> GetTrays(List<DossierTrayResponse> trays, string place)
        {
            return trays.Select(inputTray => new Item()
            {
                Name = inputTray.Type!,
                Value = inputTray,
                Icon = "fas fa-file-lines",
                CurrentPlace = place,
                OriginPlace = place,
                Client = inputTray.Client!,
                Description = inputTray.Document!.Type

            }).ToList();
        }

        private async Task UpdateItemAsync(MudItemDropInfo<Item> item)
        {
            if (item.Item.CurrentPlace == "workplace" && item.DropzoneIdentifier != "workplace") workPlaceItems.Remove(item.Item);

            item.Item.CurrentPlace = item.DropzoneIdentifier;

            if (item.DropzoneIdentifier != "workplace") return;

            workPlaceItems.Add(item.Item);

            if (item.Item.OriginPlace != "tools")
            {
                await GetDossierAssistantAsync(item.Item);

                return;
            }

            await UpdateAssistantStepAsync();
        }

        private async Task UpdateAssistantStepAsync()
        {
            if (assistant is null) return;

            var currentDocumentStep = assistant.GetCurrentDocumentStep();

            if (currentDocumentStep.StartDate.HasValue) return;

            var updatedAssistantRequest = new AssistantStepStartDateUpdateRequest(assistant.Id);

            assistant = await UpdateAssistantStepStartDateAsync(updatedAssistantRequest);
        }

        public DocumentStep GetCurrentStep()
        {
            return assistant.GetCurrentDocumentStep();
        }

        private async Task GetDossierAssistantAsync(Item item)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            assistant = await GetAsistantByDossierAsync(dossierTray.DossierId);

            string assistantMessage = assistant.GetCurrentMessage();

            ChangeAssistantMessage(assistantMessage);
        }

        private void ChangeAssistantMessage(string message)
        {
            virtualHelper!.ChangeMessage(message);
        }

        public bool IsAssistantLastStep() => assistant.IsLastStep();

        public async Task UpdateAssistantMessageAsync(AssistantMessageUpdate assistantMessageUpdate)
        {
            string assistantMessage;

            if (assistant.Substep == assistantMessageUpdate.SubStep) return;

            assistant.Substep = assistantMessageUpdate.SubStep;

            if (!assistant.IsLastSubStep()) assistantMessage = assistant.GetCurrentMessage();
            else
            {
                assistant = await UpdateAssistantStepAsync(new(assistant.Id, assistantMessageUpdate.DossierType, 
                                                            assistantMessageUpdate.DocumentType, assistantMessageUpdate.Document));

                assistantMessage = this.assistantMessage;
            }
            
            ChangeAssistantMessage(assistantMessage);
        }

        private async Task<Assistant> UpdateAssistantStepAsync(AssistantUpdateRequest assistantUpdateRequest)
        {
            try
            {
                var updatedAssistantResponse = await HttpRepository.PutAsync<AssistantUpdateRequest, Assistant>("api/assistants", assistantUpdateRequest);

                if (updatedAssistantResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo actualizar el flujo del expediente para el asistente");
                }

                return updatedAssistantResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo actualizar el flujo del expediente para el asistente");
                return new();
            }
        }

        private async Task<Assistant> UpdateAssistantStepStartDateAsync(AssistantStepStartDateUpdateRequest updatedAssistantRequest)
        {
            try
            {
                var updatedAssistantResponse = await HttpRepository.PutAsync<AssistantStepStartDateUpdateRequest, Assistant>("api/assistants/steps/start-date", updatedAssistantRequest);

                if (updatedAssistantResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo actualizar la información de la herramienta seleccionada para el asistente");
                }

                return updatedAssistantResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo actualizar la información de la herramienta seleccionada para el asistente");
                return new();
            }
        }

        private async Task<Assistant> GetAsistantByDossierAsync(string dossierId)
        {
            try
            {
                var assitantResponse = await HttpRepository.GetAsync<Assistant>($"api/assistants/{dossierId}");

                if (assitantResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener la información del expediente para el asistente");
                }

                return assitantResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener la información del expediente para el asistente");
                return new();
            }
        }


        public async Task UpdateRegisteredDocumentAsync(Item item)
        {
            await ChangeToolPlaceAsync(new("inputs", "outputs", item));

            await Task.WhenAny(UpdateUsedToolAsync(), UpdateAssistantMessageFromItemAsync(item, 2));
        }

        public async Task UpdateGeneratedDocumentAsync(Item item)
        {
            await ChangeToolPlaceAsync(new("outputs", "outputs", item));

            await Task.WhenAny(UpdateUsedToolAsync(), UpdateAssistantMessageFromItemAsync(item, 3));
        }

        private async Task UpdateUsedToolAsync()
        {
            await ChangeToolPlaceAsync(new("tools", "tools"));

            workPlaceItems.Clear();

            StateHasChanged();
        }

        public async Task SendDocumentAsync(Item item)
        {
            // TODO: Implement the logic the send the document into the receiver user tray by the signalR Hub
            Items.Remove(item);

            await Task.WhenAny(UpdateUsedToolAsync(), UpdateAssistantMessageFromItemAsync(item, 3));
        }

        public async Task EvaluateDocumentAsync(Item item, bool isApproved)
        {
            if (isApproved) await ChangeToolPlaceAsync(new("inputs", "inputs", item));
            else Items.Remove(item);

            // TODO: Implement the logic the send the document into the receiver user tray by the signalR Hub
            await Task.WhenAny(UpdateUsedToolAsync(), UpdateAssistantMessageFromItemAsync(item, 2));
        }

        private async Task UpdateAssistantMessageFromItemAsync(Item item, int assistantSubStep)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var assistantSubStepUpdate = new AssistantMessageUpdate(dossierTray.Type!, dossierTray.Document!.Type, 
                                                                    assistantSubStep, new(dossierTray.Document.Id, dossierTray.Document.CreationDate));

            await UpdateAssistantMessageAsync(assistantSubStepUpdate);
        }


        private async Task ChangeToolPlaceAsync(WorkToolPlace workToolPlace)
        {
            var item = workToolPlace.Item;

            if (workToolPlace.Item is null) item = workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace == workToolPlace.CurrentPlace);

            var itemDropInfo = new MudItemDropInfo<Item>(item!, workToolPlace.NewPlace, 0);

            await UpdateItemAsync(itemDropInfo);
        }

        private async Task<InputOutputTrayResponse> GetUserTrayAsync(string userId)
        {
            try
            {

                var userTrayResponse = await HttpRepository.GetAsync<InputOutputTrayResponse>($"api/tray/{userId}");

                if (userTrayResponse.Error)
                {
                    return new();
                }

                return userTrayResponse.Response!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private readonly Func<Item, string, bool> VerifySelector = (Item item, string dropzone) =>
        {
            return item.CurrentPlace == dropzone;
        };

        private readonly Func<Item, IEnumerable<Item>, bool> CanDropToWorkZone = (item, workPlaceItems) =>
        {
            bool canDrop = workPlaceItems.All(workPlaceItem => workPlaceItem.OriginPlace != item.OriginPlace);

            return canDrop;
        };
    }
}