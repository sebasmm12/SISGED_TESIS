using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Generics
{
    public partial class GenericMultipleText
    {
        [Parameter]
        public TextFieldDTO TextField { get; set; } = default!;
        [Parameter]
        public string TextFieldIcon { get; set; } = default!;
        [Parameter]
        public string TextLabel { get; set; } = default!;
        [Parameter]
        public MultipleTextIcon TextIcon { get; set; } = default!;
        [Parameter]
        public EventCallback<TextFieldDTO> ExecuteAction { get; set; } = default!;

        public async Task ExecuteAsync()
        {
            await ExecuteAction.InvokeAsync(TextField);
        }

    }
}