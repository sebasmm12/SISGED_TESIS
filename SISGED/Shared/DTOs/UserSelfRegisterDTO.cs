using SISGED.Shared.Models.Responses.DocumentType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class UserSelfRegisterDTO
    {
        public string Username { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email  { get; set; } = default!;
        public string DocumentNumber  { get; set; } = default!;
        public string Address  { get; set; } = default!;
        public DateTime? BornDate  { get; set; } = default!;
        public DocumentTypeInfoResponse DocumentType  { get; set; } = default!;
    }
}
