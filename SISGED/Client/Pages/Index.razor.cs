using Microsoft.AspNetCore.Components;
using SISGED.Shared.Models.Responses.Account;

namespace SISGED.Client.Pages
{
    public partial class Index
    {
        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;
    }
}