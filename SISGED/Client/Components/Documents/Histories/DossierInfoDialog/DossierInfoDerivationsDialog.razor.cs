using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Components.Documents.Histories.DossierInfoDialog
{
    public partial class DossierInfoDerivationsDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; } = default!;

        [Parameter]
        public List<UserDossierDerivationDTO> Derivations { get; set; } = new List<UserDossierDerivationDTO>();
        private void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}