namespace SISGED.Shared.Models.Responses.Document
{
    public class SignExpeditionRegisterResponse : Entities.Document
    {
        public string ClientName { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public SignExpeditionRegisterResponseContent Content { get; set; } = new SignExpeditionRegisterResponseContent();
    }

    public class SignExpeditionRegisterResponseContent
    {
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Client { get; set; } = default!;
        public DateTime RealizationDate { get; set; } = default!;
        public string Data { get; set; } = default!;
        public List<string> URLAnnex { get; set; } = new List<string>();

    }
}
