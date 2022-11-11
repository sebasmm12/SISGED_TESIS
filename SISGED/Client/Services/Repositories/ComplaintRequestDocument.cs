using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Client.Services.Repositories
{
    public class ComplaintRequestDocument : IDocumentRender
    {
        public string DocumentType { get; set; } = "ComplaintRequest";
        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(ComplaintRequestRegister));
            builder.CloseComponent();
        };
    }
}
