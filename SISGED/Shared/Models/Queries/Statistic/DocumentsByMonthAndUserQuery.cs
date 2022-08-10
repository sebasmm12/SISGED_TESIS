namespace SISGED.Shared.Models.Queries.Statistic
{
    public class DocumentsByMonthAndUserQuery : DocumentsByMonthQuery
    {
        public string UserId { get; set; } = default!;
    }
}
