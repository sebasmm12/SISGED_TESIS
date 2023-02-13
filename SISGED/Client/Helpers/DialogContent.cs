using MudBlazor;

namespace SISGED.Client.Helpers
{
    public class DialogContent
    {
        public DialogContent(Type component, string title, DialogParameters dialogParameters, DialogOptions dialogOptions)
        {
            Component = component;
            Title = title;
            DialogParameters = dialogParameters;
            DialogOptions = dialogOptions;
        }

        public Type Component { get; set; } = default!;
        public string Title { get; set; } = default!;
        public DialogParameters DialogParameters { get; set; } = default!;
        public DialogOptions DialogOptions { get; set; } = default!;
    }
}
