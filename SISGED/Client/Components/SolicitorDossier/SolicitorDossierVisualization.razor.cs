using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.SolicitorDossier;

namespace SISGED.Client.Components.SolicitorDossier
{
    public partial class SolicitorDossierVisualization
    {
        [Inject]
        public IBadgeFactory BadgeFactory { get; set; } = default!;

        [Parameter]
        public AnnexPreview SolicitorDossierVisalization { get; set; } = default!;

        [Parameter]
        public SolicitorDossierResponse SolicitorDossier { get; set; } = default!; 

        [Parameter]
        public EventCallback<SolicitorDossierActionDTO> AddOrDeleteSolicitorDossier  { get; set; } = default!;

        [Parameter]
        public bool Selected { get; set; } = false;

        private BadgePreview badgePreview = default!;

        protected override void OnInitialized()
        {
            ChangeBadgePreview();
        }

        private async Task AddOrDeleteSolicitorDossierAsync()
        {
            Selected = !Selected;

            var solicitorDossierAction = new SolicitorDossierActionDTO(SolicitorDossier.Id, Selected);

            ChangeBadgePreview();

            await AddOrDeleteSolicitorDossier.InvokeAsync(solicitorDossierAction);
        }

        private void ChangeBadgePreview()
        {
            badgePreview = BadgeFactory.GetBadgePreview(Selected);
        }
    }
}