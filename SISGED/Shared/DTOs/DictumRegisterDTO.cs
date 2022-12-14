using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class DictumRegisterDTO
    {
        public string Title { get; set; } = default!;
        public string Conclusion { get; set; } = default!;
        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public List<TextFieldDTO> Observations { get; set; } = new() { new(string.Empty, 0) };
        public List<TextFieldDTO> Recommendations { get; set; } = new() { new(string.Empty, 0) };
    }
}
