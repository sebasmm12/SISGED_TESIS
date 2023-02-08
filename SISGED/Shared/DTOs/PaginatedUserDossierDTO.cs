using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class PaginatedUserDossierDTO
    {
        public PaginatedUserDossierDTO() { }

        public PaginatedUserDossierDTO(IEnumerable<UserDossierDTO> userDossiers, int total)
        {
            UserDossiers = userDossiers;
            Total = total;
        }

        public IEnumerable<UserDossierDTO> UserDossiers { get; set; } = default!;
        public int Total { get; set; }
    }
}
