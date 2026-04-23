using System;

namespace Ewp.Models
{
    public class LoginHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; } = "";

        public string Role { get; set; } = "";

        public DateTime LoginTime { get; set; } = DateTime.Now;

        // ✅ FIXED
        public string? DepartmentName { get; set; }
    }
}