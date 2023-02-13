using Microsoft.AspNetCore.Components;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Responses.DocumentProcess;

namespace SISGED.Client.Components.Documents.Histories
{
    public partial class DocumentProcess
    {
        [Inject]
        public IDocumentStateRepository DocumentStateRepository { get; set; } = default!;

        [Parameter]
        public DocumentProcessInfo Process { get; set; } = default!;


        private string GetDocumentStateName(string documentState)
        {
            return DocumentStateRepository.GetDocumentState(documentState).ProcessName;
        }
    }
}