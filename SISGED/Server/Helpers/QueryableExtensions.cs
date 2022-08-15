using SISGED.Shared.Models;

namespace SISGED.Server.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(
           this IQueryable<T> queryable,
           Pagination pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.QuantityPerPage)
                .Take(pagination.QuantityPerPage);
        }
    }
}

