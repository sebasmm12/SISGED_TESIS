using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class UserDossierDTO
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public Client Client { get; set; } = default!;
        public List<Derivation> Derivations { get; set; } = default!;
        public List<DossierDocument> Documents { get; set; } = default!;
        public string State { get; set; } = default!;
        public DateTime StartDate { get; set; } = default!;
        public DateTime? EndDate { get; set; } = default!;
    }
}
