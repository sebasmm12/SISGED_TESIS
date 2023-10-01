namespace SISGED.Shared.Models.Responses.Document.InitialRequest
{
    public class InitialRequestInfoResponse: DocumentInfoResponse
    {
        public InitialRequestContentInfo Content { get; set; } = default!;
    }

    public class InitialRequestContentInfo
    {
        public string Description { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}
