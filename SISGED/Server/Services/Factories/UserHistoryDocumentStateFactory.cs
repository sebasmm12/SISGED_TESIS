using SISGED.Server.Services.Factories.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Server.Services.Factories
{
    public class UserHistoryDocumentStateFactory : IUserHistoryDocumentStateFactory
    {
        private readonly IEnumerable<IUserHistoryDocumentStateStrategy> _userHistoryDocumentStateStrategies;

        public UserHistoryDocumentStateFactory(IEnumerable<IUserHistoryDocumentStateStrategy> userHistoryDocumentStateStrategies)
        {
            _userHistoryDocumentStateStrategies = userHistoryDocumentStateStrategies;
        }

        public async Task<IEnumerable<UserDocumentHistoryStateResponse>> GetUserHistoryDocumentStateAsync(string userId, DateFilterDTO dateFilter)
        {
            var userHistoryDocumentStateStrategy =  _userHistoryDocumentStateStrategies.FirstOrDefault(x => x.DateFilter == dateFilter);

            if (userHistoryDocumentStateStrategy is null) throw new ArgumentException("Filtro de fechas inválido para la obtención de los documentos del usuaio por estado");

            var documents = await userHistoryDocumentStateStrategy.GetDocumentsAsync(userId);

            var groupedDocuments = documents.GroupBy(document => document.Date)
                                                                            .Select(g => new UserDocumentHistoryStateResponse
                                                                            {
                                                                                Date = g.Key,
                                                                                Documents = g.GroupBy(item => item.State)
                                                                                            .ToDictionary(group => group.Key, group => group.Count())
                                                                            }).ToList();
            return groupedDocuments;
        }
    }
}
