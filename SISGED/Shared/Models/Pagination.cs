using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models
{
    public class Pagination
    {
        public int Page { get; set; } = 1;
        public int QuantityPerPage { get; set; } = 3;
    }
}
