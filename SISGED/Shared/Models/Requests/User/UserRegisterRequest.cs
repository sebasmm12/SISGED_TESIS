using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Requests.User
{
    public class UserRegisterRequest
    {
        public string Type { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public UserData Data { get; set; } = new UserData();
        public string State { get; set; } = default!;
        public string Rol { get; set; } = default!;
    }
}
