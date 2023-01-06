using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class SolicitorDossierRequestDocument : IDocumentRender
    {
        public string DocumentType { get; set; } = "SolicitorDossierRequest";
        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(SolicitorDossierRequestRegister));
            builder.CloseComponent();
        };
    }
}
