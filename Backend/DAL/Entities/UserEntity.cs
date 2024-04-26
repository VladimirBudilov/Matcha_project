using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class UserEntity
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public string? ResetToken { get; set; }
        public bool? IsVerified { get; set; }
        
        public ProfileEntity Profile { get; set; }
    }
}