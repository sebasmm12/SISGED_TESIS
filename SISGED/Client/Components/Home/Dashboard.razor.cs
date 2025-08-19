using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Shared.Models.Responses.Account;

namespace SISGED.Client.Components.Home;

public partial class Dashboard
{
    [CascadingParameter(Name = "SessionAccount")]
    public SessionAccountResponse SessionAccount { get; set; } = default!;

    private IEnumerable<Roles> managerRoles = new List<Roles>
    {
        Roles.SecretariaGeneral,
        Roles.JuntaDirectiva,
    };

    private Roles userRole;

    protected override void OnInitialized()
    {
        userRole = Enum.Parse<Roles>(SessionAccount.Role);
    }
}