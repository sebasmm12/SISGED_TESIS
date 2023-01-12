using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SISGED.Shared.DTOs
{
    public class UserLoginDTO
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
