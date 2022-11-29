using Microsoft.AspNetCore.Components;

namespace SISGED.Client.Generics
{
    public partial class GenericListHeader
    {
        [Parameter]
        public string HeaderIcon { get; set; } = default!;
        [Parameter]
        public string HeaderTitle { get; set; } = default!;
        [Parameter]
        public string HeaderDescription { get; set; } = default!;
        [Parameter]
        public string CreationButtonDescription { get; set; } = default!;
        [Parameter]
        public EventCallback OnCreationButton { get; set; }

        private async Task CreateEntityAsync()
        {
            await OnCreationButton.InvokeAsync();
        }


    }
}