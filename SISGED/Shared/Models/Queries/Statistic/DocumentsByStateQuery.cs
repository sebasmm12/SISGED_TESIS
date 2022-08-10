namespace SISGED.Shared.Models.Queries.Statistic
{
    public class DocumentsByStateQuery : DocumentsByMonthAndAreaQuery
    {
        public string? UserId { get; set; } = default!;
    }
}
