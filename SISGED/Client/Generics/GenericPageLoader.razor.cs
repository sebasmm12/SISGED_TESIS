using Microsoft.AspNetCore.Components;

namespace SISGED.Client.Generics
{
    public partial class GenericPageLoader
    {
        [Parameter]
        public string Body { get; set; } = default!;
    }
}