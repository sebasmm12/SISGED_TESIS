using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Queries.Document
{
    public class UserRequestPaginationQuery : PaginationQuery
    {
        public string DocumentNumber { get; set; } = default!;
    }
}
