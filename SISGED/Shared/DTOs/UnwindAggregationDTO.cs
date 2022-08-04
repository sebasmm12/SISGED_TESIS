using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class UnwindAggregationDTO
    {
        public UnwindAggregationDTO(string path, string? includeArrayIndex = null, bool? preserveNullAndEmptyArrays = null)
        {
            Path = path;
            IncludeArrayIndex = includeArrayIndex;
            PreserveNullAndEmptyArrays = preserveNullAndEmptyArrays;
        }

        public string Path { get; set; } = default!;
        public string? IncludeArrayIndex { get; set; }
        public bool? PreserveNullAndEmptyArrays { get; set; }
    }
}
