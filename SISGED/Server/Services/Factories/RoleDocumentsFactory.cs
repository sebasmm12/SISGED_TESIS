using SISGED.Server.Services.Factories.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Server.Services.Factories
{
    public class RoleDocumentsFactory : IRoleDocumentsFactory
    {
        private readonly IEnumerable<IRoleDocumentsStrategy> _roleDocumentsStrategies;

        private readonly IEnumerable<string> documentStates = new List<string>
        {
            "creado",
            "modificado",
            "generado",
            "evaluado",
            "derivado"
        };

        public RoleDocumentsFactory(IEnumerable<IRoleDocumentsStrategy> roleDocumentsStrategies)
        {
            _roleDocumentsStrategies = roleDocumentsStrategies;
        }

        public async Task<IEnumerable<RoleDocumentsResponse>> GetRolesDocumentsAsync(DateFilterDTO dateFilter)
        {
            var roleDocumentStrategy =  _roleDocumentsStrategies.FirstOrDefault(x => x.DateFilter == dateFilter);

            if(roleDocumentStrategy is null) throw new ArgumentException("Filtro de fechas inválido para la obtención de los documentos por roles");

            var documents = await roleDocumentStrategy.GetDocumentsAsync();

            var groupedDocuments = documents.GroupBy(document => document.Role);

            var rolesDocuments = groupedDocuments.Select(GetRoleDocumentsByState);

            return rolesDocuments;
        }

        private RoleDocumentsResponse GetRoleDocumentsByState(IGrouping<string, RoleDocumentDTO> groupedDocuments)
        {
            var documents = groupedDocuments.ToList();

            var dueDocuments = documents
                                .Where(IsDocumentExpired)
                                .ToList();

            var approvedDocuments = documents
                .Where(document => document.IsApproved.HasValue && document.IsApproved.Value)
                .ToList();

            var rejectedDocuments = documents
                .Where(document => document.IsApproved.HasValue && !document.IsApproved.Value)
                .ToList();

            var nonDueDocuments = documents
                .Except(dueDocuments)
                .Except(approvedDocuments)
                .Except(rejectedDocuments);

            var documentsByState = documentStates
                                    .Select(state => GetDocumentsState(state, nonDueDocuments))
                                    .ToDictionary(documentState => documentState.State, documentState => documentState.Count);

            SetAdditionalDocumentStates(
                documentsByState, 
                dueDocuments.Count, 
                approvedDocuments.Count, 
                rejectedDocuments.Count);

            return new(groupedDocuments.Key, documentsByState);
        }

        private static DocumentsState GetDocumentsState(string state, IEnumerable<RoleDocumentDTO> roleDocuments)
        {
            var totalDocuments = roleDocuments.Count(document => document.State == state);

            return new(state, totalDocuments);
        }

        private static bool IsDocumentExpired(RoleDocumentDTO document)
        {
            var currentDate = DateTime.UtcNow.AddHours(-5);

            return !document.EndDate.HasValue && document.DueDate.Date < currentDate.Date;
        }

        private static void SetAdditionalDocumentStates(
            Dictionary<string, int> documentsByState, 
            int dueDocuments, 
            int approvedDocuments, 
            int rejectedDocuments)
        {
            documentsByState.Add("retrasado", dueDocuments);
            documentsByState.Add("aprobado", approvedDocuments);
            documentsByState.Add("rechazado", rejectedDocuments);
        }
    }
}
