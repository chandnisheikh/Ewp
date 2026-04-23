using System;
using System.ComponentModel.DataAnnotations;

namespace Ewp.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";

        public string? ResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}