using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Client.Services.Repositories
{
    public class DisciplinaryOpennessDocument : IDocumentRender
    {
        public string DocumentType { get; set; } = "DisciplinaryOpenness";
        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(DisciplinaryOpennessRegister));
            builder.CloseComponent();
        };
    }
}
