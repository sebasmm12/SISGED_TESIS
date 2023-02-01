using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class SolicitorRepository: IFilterRepository<SolicitorFilter>
    {
        public Dictionary<string, object> ConvertToFilters(SolicitorFilter solicitorFilter)
        {
            var filters = new Dictionary<string, object>();

            var solicitorConditions = GetSolicitorConditions();

            solicitorConditions.ForEach(solicitorCondition =>
            {
                if (solicitorCondition.Condition(solicitorFilter)) filters = solicitorCondition.Result(filters, solicitorFilter);
            });

            return filters;
        }

        private static List<FilterCondition<SolicitorFilter, Dictionary<string, object>>> GetSolicitorConditions()
        {
            var solicitorConditions = new List<FilterCondition<SolicitorFilter, Dictionary<string, object>>>()
            {
                new()
                {
                    Condition = (solicitorFilter) => true,
                    Result = (Dictionary<string, object> filters, SolicitorFilter solicitorFilter) =>
                    {
                        string solicitorName = string.IsNullOrEmpty(solicitorFilter.SolicitorName) ? "" : 
                            solicitorFilter.SolicitorName.ToLower().Trim();

                        filters.Add("solicitorName", solicitorName);

                        return filters;
                    }
                },
                new()
                {
                    Condition = (solicitorFilter) => solicitorFilter.ExSolicitor.HasValue,
                    Result = (Dictionary<string, object> filters, SolicitorFilter solicitorFilter) =>
                    {
                        filters.Add("exSolicitor", solicitorFilter.ExSolicitor!.Value);

                        return filters;
                    }
                }
            };

            return solicitorConditions;
        }
    }
}
