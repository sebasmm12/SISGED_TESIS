using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Requests.Step
{
    public class StepsRequest
    {
        public String Id { get; set; } = default!;
        public String DossierName { get; set; } = default!;
        public List<DocumentStepRequest> Documents { get; set; } = default!;
    }
    public class DocumentStepRequest
    {
        public string Uid { get; set; } = default!;
        public Int32 Index { get; set; } = default!;
        public String Type { get; set; } = default!;
        public List<StepRequest> Steps { get; set; } = default!;
    }

    public class StepRequest
    {
        public string Uid { get; set; }
        public Int32 Index { get; set; }
        public String Name { get; set; } = default!;
        public String Description { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public Int32 Days { get; set; }
        public List<Substep> Substep { get; set; } = default!;

    }
}
