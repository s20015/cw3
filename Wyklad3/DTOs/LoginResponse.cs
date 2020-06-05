using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wyklad3.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
