namespace SISGED.Shared.Models.Queries.Document
{
    public class UserRequestPaginationQuery : PaginationQuery
    {
        public string ClientId { get; set; } = default!;
    }
}
