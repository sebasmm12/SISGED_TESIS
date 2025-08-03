using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Dashboards
{
    public record UserDocumentSnapshotResponse (int registrados, int derivados);
}
// This code defines a record type `UserDocumentSnapshotResponse` with two properties: `registrados` and `derivados`.