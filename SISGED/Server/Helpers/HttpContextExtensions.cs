namespace SISGED.Server.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPagedParameterOnResponse<T>(
            this HttpContext context,
            IQueryable<T> queryable, int quantityPerPage)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            double count = Convert.ToDouble(queryable.Count());
            double totalPaginas = Math.Ceiling(count / quantityPerPage);
            context.Response.Headers.Add("conteo", count.ToString());
            context.Response.Headers.Add("totalPaginas", totalPaginas.ToString());
        }
    }
}
