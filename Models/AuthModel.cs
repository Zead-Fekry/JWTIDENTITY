﻿using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace JWT.Models
{
    public class AuthModel
    {
       public String Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public List <String> Roles { get; set; }
        public String Token { get; set; }
        public DateTime Expires { get; set; }

    }
}
