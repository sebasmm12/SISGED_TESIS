using Microsoft.AspNetCore.Components;

namespace SISGED.Client.Generics
{
    public partial class GenericFormHeader
    {
        [Parameter]
        public string HeaderIcon { get; set; } = default!;
        [Parameter]
        public string HeaderTitle { get; set; } = default!;
        [Parameter]
        public string HeaderDescription { get; set; } = default!;
    }
}