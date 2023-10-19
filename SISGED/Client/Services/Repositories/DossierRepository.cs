using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DossierRepository : IDossierRepository
    {
        public IEnumerable<SelectOption> GetDossierTypes()
        {
            return new List<SelectOption>()
            {
                new("Denuncia", "Denuncia"),
                new("Expedicion de Firma", "Expedicion de Firma"),
                new("Solicitud", "Solicitud"),
                new("Busqueda de Protocolo Notarial", "Busqueda Protocolo Notarial")
            };
        }
    }
}
