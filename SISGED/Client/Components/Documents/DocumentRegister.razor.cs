using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.DossierTray;

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
        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;

        // Fields
        private Roles userRole;
        private IEnumerable<DocumentOption> documentOptions = default!;
        private RenderFragment? childContent;
        private DossierTrayResponse dossierTray = default!;
        private IEnumerable<string> registeredDocuments = default!;

        protected override async Task OnInitializedAsync()
        {
            dossierTray = GetDossierTray();

            userRole = Enum.Parse<Roles>(SessionAccount.Role);

            registeredDocuments = GetDocumentsFromAssistant();

            documentOptions = DocumentRepository.GetDocumentTypesWithDossier().Where(document => document.Rol == userRole 
                                                                                    && registeredDocuments.Contains(document.Value));

            await WorkEnvironment.UpdateAssistantMessageAsync(new(dossierTray.Type!, dossierTray.Document!.Type, 1));
        }

        private IEnumerable<string> GetDocumentsFromAssistant()
        {
            var documents = WorkEnvironment.GetCurrentStep().RegisteredDocuments;

            if (documents is null) return new List<string>();

            return documents;
        }

        private DossierTrayResponse GetDossierTray()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            var dossierTray = userTray.Value as DossierTrayResponse;

            return dossierTray!;
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