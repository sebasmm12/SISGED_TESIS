using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Client.Components.WorkEnvironments
{
    public partial class WorkEnvironment
    {
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        public InputOutputTrayResponse UserTray { get; set; } = default!;
        public DossierTrayResponse SelectedTray { get; set; } = default!;
        private List<Item> Items { get; set; } = new();
        private bool isRendered = false;
        public readonly List<Item> workPlaceItems = new();

        private bool CanReorder => workPlaceItems.Count > 0;

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
                Value = permission.Name,
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

        private void UpdateItem(MudItemDropInfo<Item> item)
        {
            if (item.Item.CurrentPlace == "workplace" && item.DropzoneIdentifier != "workplace") workPlaceItems.Remove(item.Item);

            item.Item.CurrentPlace = item.DropzoneIdentifier;

            if (item.DropzoneIdentifier == "workplace") workPlaceItems.Add(item.Item);
        }

        public void UpdateRegisteredDocument(Item item)
        {
            ChangeToolPlace(new("inputs", "outputs", item));
            UpdateUsedTool();
        }

        public void UpdateGeneratedDocument(Item item)
        {
            ChangeToolPlace(new("outputs", "outputs", item));

            UpdateUsedTool();
        }

        private void UpdateUsedTool()
        {
            ChangeToolPlace(new("tools", "tools"));

            workPlaceItems.Clear();

            StateHasChanged();
        }

        public void SendDocument(Item item)
        {
            // TODO: Implement the logic the send the document into the receiver user tray by the signalR Hub
            Items.Remove(item);

            UpdateUsedTool();
        }

        public void EvaluateDocument(Item item, bool isApproved)
        {
            if (isApproved) {
                ChangeToolPlace(new("inputs", "inputs", item));
                return;
            }

            // TODO: Implement the logic the send the document into the receiver user tray by the signalR Hub
            Items.Remove(item);

            UpdateUsedTool();
        }

        private void ChangeToolPlace(WorkToolPlace workToolPlace)
        {
            var item = workToolPlace.Item;

            if (workToolPlace.Item is null) item = workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace == workToolPlace.CurrentPlace);

            var itemDropInfo = new MudItemDropInfo<Item>(item!, workToolPlace.NewPlace, 0);

            UpdateItem(itemDropInfo);
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
            bool canDrop = true;

            if (workPlaceItems.Any(workPlaceItem => workPlaceItem.OriginPlace == item.OriginPlace)) canDrop = false;

            return canDrop;
        };
    }
}