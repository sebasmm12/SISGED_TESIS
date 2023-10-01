namespace SISGED.Shared.Models.Responses.Document
{
    public class BPNResultResponse : Entities.Document
    {
        public BPNResultResponseContent Content { get; set; } = new BPNResultResponseContent();
    }

    public class BPNResultResponseContent
    {
        public Int32 Cost { get; set; }
        public Int32 PageQuantity { get; set; } = 2;
        public string State { get; set; }
        public Entities.PublicDeed PublicDeedId { get; set; }
        public List<string> URLAnnex { get; set; } = new();
    }
}
