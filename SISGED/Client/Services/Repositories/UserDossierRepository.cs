using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Services.Repositories
{
    public class UserDossierRepository : IFilterRepository<UserDossierFilterDTO>
    {
        public Dictionary<string, object> ConvertToFilters(UserDossierFilterDTO userDossierFilter)
        {
            var filters = new Dictionary<string, object>();

            var conditions = GetUserDossierConditions();

            conditions.ForEach(condition =>
            {
                if (condition.Condition(userDossierFilter)) filters = condition.Result(filters, userDossierFilter);
            });

            return filters;
        }

        private static List<FilterCondition<UserDossierFilterDTO, Dictionary<string, object>>> GetUserDossierConditions()
        {
            var conditions = new List<FilterCondition<UserDossierFilterDTO, Dictionary<string, object>>>()
            {
                new()
                {
                    Condition = (userDossierQuery) => !string.IsNullOrEmpty(userDossierQuery.Code),
                    Result = (Dictionary<string, object> filters, UserDossierFilterDTO userDossierQuery) =>
                    {
                        string code = userDossierQuery.Code!.ToLower().Trim();

                        filters.Add("code", code);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDossierQuery) => userDossierQuery.StartDate.HasValue,
                    Result = (Dictionary<string, object> filters, UserDossierFilterDTO userDossierQuery) =>
                    {
                        filters.Add("startDate", userDossierQuery.StartDate!.Value.ToString("MM/dd/yyyy"));

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDossierQuery) => userDossierQuery.EndDate.HasValue,
                    Result = (Dictionary<string, object> filters, UserDossierFilterDTO userDossierQuery) =>
                    {
                        filters.Add("endDate", userDossierQuery.EndDate!.Value.ToString("MM/dd/yyyy"));

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDossierQuery) => !string.IsNullOrEmpty(userDossierQuery.ClientName),
                    Result = (Dictionary<string, object> filters, UserDossierFilterDTO userDossierQuery) =>
                    {
                        string clientName = userDossierQuery.ClientName!.ToLower().Trim();

                        filters.Add("clientName", clientName);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDossierQuery) => !string.IsNullOrEmpty(userDossierQuery.Type),
                    Result = (Dictionary<string, object> filters, UserDossierFilterDTO userDossierQuery) =>
                    {
                        string dossierType = userDossierQuery.Type!.ToLower().Trim();

                        filters.Add("type", dossierType);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (userDossierQuery) => !string.IsNullOrEmpty(userDossierQuery.State),
                    Result = (Dictionary<string, object> filters, UserDossierFilterDTO userDossierQuery) =>
                    {
                        string state = userDossierQuery.State!.ToLower().Trim();

                        filters.Add("state", state);

                        return filters;
                    }
                }

            };

            return conditions;
        }
    }
}