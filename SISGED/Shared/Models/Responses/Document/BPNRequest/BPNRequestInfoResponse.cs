
namespace SISGED.Shared.Models.Responses.Document.BPNRequest
{
    public class BPNRequestInfoResponse: DocumentInfoResponse
    {
        public BPNRequestContentInfo Content { get; set; } = default!;
    }

    public class BPNRequestContentInfo
    {
        public string JuridicalAct { get; set; } = default!;
        public string ProtocolType { get; set; } = default!;
        public DateTime RealizationDate { get; set; }
    }
}
