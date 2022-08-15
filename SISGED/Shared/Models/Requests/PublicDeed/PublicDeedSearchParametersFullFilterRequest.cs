using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Requests.PublicDeed
{
    public class PublicDeedSearchParametersFullFilterRequest
    {
        public int Page { get; set; } = 1;
        public int RecordsQuantity { get; set; } = 1;

        public Pagination Pagination
        {
            get { return new Pagination() { Page = Page, QuantityPerPage = RecordsQuantity }; }
        }

        public string NotarialOfficeDirection { get; set; } = default!;
        public string NotaryName { get; set; } = default!;
        public string LegalAct { get; set; } = default!;
        public List<string> GrantersName { get; set; } = default!
    }
}
