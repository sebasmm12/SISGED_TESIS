using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies
{
    public class UserHistoryDocumentStateMonthlyStrategy : IUserHistoryDocumentStateStrategy
    {
        private readonly IDocumentService _documentService;

        public UserHistoryDocumentStateMonthlyStrategy(IDocumentService documentService)
        {
            _documentService = documentService;
        }
        public DateFilterDTO DateFilter => DateFilterDTO.Monthly;

        public async Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId)
        {
            var result = await _documentService.GetUserHistoryStateMonthlyAsync(userId);
            ReemplazarMeses(result);
            return result;
        }

        private static void ReemplazarMeses(IEnumerable<UserDocumentHistoryStateDTO> lista)
        {
            // Array con nombres de meses en español (índice 0 = Enero, 11 = Diciembre)
            string[] meses = {
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    };

            foreach (var item in lista)
            {
                // Convertir el string a entero (maneja formatos "1", "01", "10", etc.)
                if (int.TryParse(item.Date, out int mes) && mes >= 1 && mes <= 12)
                {
                    item.Date = meses[mes - 1];  // Acceder al índice del array
                }
                else
                {
                    // Manejar valores inválidos (opcional)
                    item.Date = "Inválido";
                }
            }
        }
    }
}
