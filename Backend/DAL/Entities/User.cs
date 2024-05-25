﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        public string? EmailResetToken { get; set; }
        public string? JwtResetToken { get; set; }
        public bool IsVerified { get; set; } = false;
        
        public Profile? Profile { get; set; }
        
        
        public bool HasLike { get; set; } = false;
    }
}