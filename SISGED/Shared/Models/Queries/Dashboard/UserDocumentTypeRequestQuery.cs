using SISGED.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Queries.Dashboard
{
    public class UserDocumentTypeRequestQuery
    {
        public string UserId { get; set; } = default!;
        public DateFilterDTO DateFilter { get; set; } = default!;
    }
}
