using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Requests.User
{
    public class UserPasswordUpdateRequest
    {
        public string UserId { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
