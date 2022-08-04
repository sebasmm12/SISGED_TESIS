using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class HttpResponseWriterDTO
    {
        public HttpResponseWriterDTO(Stream originalStream, HttpContext context, MemoryStream httpMemoryStream)
        {
            OriginalStream = originalStream;
            Context = context;
            HttpMemoryStream = httpMemoryStream;
        }

        public Stream OriginalStream { get; set; } = default!;
        public HttpContext Context { get; set; } = default!;
        public MemoryStream HttpMemoryStream { get; set; } = default!;

        
    }
}
