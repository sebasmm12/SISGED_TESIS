namespace SISGED.Shared.Models.Queries.Document
{
    public class UserRequestPaginationQuery : PaginationQuery
    {
        public string DocumentNumber { get; set; } = default!;
    }
}
