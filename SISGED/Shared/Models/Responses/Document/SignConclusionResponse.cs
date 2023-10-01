namespace SISGED.Shared.Models.Responses.Document
{
    public class SignConclusionResponse : Entities.Document
    {
        public SignConclusionResponseContent Content { get; set; } = new();
    }
    public class SignConclusionResponseContent
    {
        public Entities.PublicDeed PublicDeedId { get; set; } = new();
        public Entities.Solicitor SolicitorId { get; set; } = new();
        public Entities.User ClientId { get; set; } = new();
        public Int32 PageQuantity { get; set; } = 2;
        public double Price { get; set; } = 0;
        public List<string> URLAnnex { get; set; } = new();

    }
}
