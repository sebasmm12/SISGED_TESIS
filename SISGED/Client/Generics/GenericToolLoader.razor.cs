using Microsoft.AspNetCore.Components;

namespace SISGED.Client.Generics
{
    public partial class GenericToolLoader
    {
        [Parameter]
        public string Body { get; set; } = default!;
    }
}