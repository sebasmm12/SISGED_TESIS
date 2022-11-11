using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;

namespace SISGED.Client.Components.Documents
{
    public partial class DocumentRegister
    {
        // Dependencies Injection
        [Inject]
        private IDocumentRepository DocumentRepository { get; set; } = default!;
        [Inject]
        private DocumentStrategy DocumentStrategy { get; set; } = default!;

        // Parameters

        // Fields
        private Roles userRole;
        private IEnumerable<DocumentOption> documentOptions = default!;
        private RenderFragment? childContent;

        protected override void OnInitialized()
        {
            // TODO: Implement logic to get the userRoles based on the authenticated user
            userRole = Roles.MesaPartes;

            documentOptions = DocumentRepository.GetDocumentTypesWithDossier().Where(document => document.Rol == userRole);
        }


        private void GetDocument(DocumentOption documentOption)
        {
            if (documentOption is null)
            {
                childContent = null;
                return;
            }

            var documentRender = DocumentStrategy.GetDocument(documentOption.Value);

            childContent = documentRender.RenderFragment;
        }
    }
}