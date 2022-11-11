using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class SignExpeditionRequestDocument : IDocumentRender
    {
        public string DocumentType { get; set; } = "SignExpeditionRequest";
        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(SignExpeditionRequestRegister));
            builder.CloseComponent();
        };
    }
}
