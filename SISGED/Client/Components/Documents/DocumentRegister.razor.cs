using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Account;

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
        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        // Fields
        private Roles userRole;
        private IEnumerable<DocumentOption> documentOptions = default!;
        private RenderFragment? childContent;

        protected override void OnInitialized()
        {
            userRole = Enum.Parse<Roles>(SessionAccount.Role);

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