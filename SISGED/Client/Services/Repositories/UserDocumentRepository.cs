using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Services.Repositories
{
    public class UserDocumentRepository : IFilterRepository<UserDocumentFilterDTO>
    {
        public Dictionary<string, object> ConvertToFilters(UserDocumentFilterDTO userDocumentFilter)
        {
            var filters = new Dictionary<string, object>();

            var solicitorConditions = GetUserDocumentConditions();

            solicitorConditions.ForEach(solicitorCondition =>
            {
                if (solicitorCondition.Condition(userDocumentFilter)) filters = solicitorCondition.Result(filters, userDocumentFilter);
            });

            return filters;
        }

        private static List<FilterCondition<UserDocumentFilterDTO, Dictionary<string, object>>> GetUserDocumentConditions()
        {
            var solicitorConditions = new List<FilterCondition<UserDocumentFilterDTO, Dictionary<string, object>>>()
            {
                new()
                {
                    Condition = (userDocumentQuery) => !string.IsNullOrEmpty(userDocumentQuery.Code),
                    Result = (Dictionary<string, object> filters, UserDocumentFilterDTO userDocumentQuery) =>
                    {
                        string code = userDocumentQuery.Code!.ToLower().Trim();

                        filters.Add("code", code);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDocumentQuery) => userDocumentQuery.StartDate.HasValue,
                    Result = (Dictionary<string, object> filters, UserDocumentFilterDTO userDocumentQuery) =>
                    {
                        filters.Add("startDate", userDocumentQuery.StartDate!.Value);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDocumentQuery) => userDocumentQuery.EndDate.HasValue,
                    Result = (Dictionary<string, object> filters, UserDocumentFilterDTO userDocumentQuery) =>
                    {
                        filters.Add("endDate", userDocumentQuery.EndDate!.Value);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDocumentQuery) => !string.IsNullOrEmpty(userDocumentQuery.ClientName),
                    Result = (Dictionary<string, object> filters, UserDocumentFilterDTO userDocumentQuery) =>
                    {
                        string clientName = userDocumentQuery.ClientName!.ToLower().Trim();

                        filters.Add("clientName", clientName);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDocumentQuery) => !string.IsNullOrEmpty(userDocumentQuery.DossierType),
                    Result = (Dictionary<string, object> filters, UserDocumentFilterDTO userDocumentQuery) =>
                    {
                        string dossierType = userDocumentQuery.DossierType!.ToLower().Trim();

                        filters.Add("dossierType", dossierType);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDocumentQuery) => !string.IsNullOrEmpty(userDocumentQuery.State),
                    Result = (Dictionary<string, object> filters, UserDocumentFilterDTO userDocumentQuery) =>
                    {
                        string state = userDocumentQuery.State!.ToLower().Trim();

                        filters.Add("state", state);

                        return filters;
                    }
                }

            };

            return solicitorConditions;
        }
    }
}
