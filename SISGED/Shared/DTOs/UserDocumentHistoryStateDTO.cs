using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class UserDocumentHistoryStateDTO
    {
        public string State { get; set; } = default!;
        public string Date { get; set; } = default!;

    }
}
