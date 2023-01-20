using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Generators;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Client.Services.Repositories
{
    public class SolicitorDossierRequestGeneration : IDocumentRender
    {
        public string DocumentType { get; set; } = "SolicitudExpedienteNotario";

        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(SolicitorDossierRequestGenerator));
            builder.CloseComponent();
        };
    }
}
