namespace SISGED.Shared.Models.Responses.Solicitor
{
    public class SolicitorInfoResponse
    {
        public string Name { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime BornDate { get; set; }
        public string DNI { get; set; } = default!;
    }
}
