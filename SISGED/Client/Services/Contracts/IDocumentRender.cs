using Microsoft.AspNetCore.Components;

namespace SISGED.Client.Services.Contracts
{
    public interface IDocumentRender
    {
        public string DocumentType { get; set; }
        public RenderFragment RenderFragment { get; set; }
    }
}
