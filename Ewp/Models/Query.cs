using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ewp.Models
{
    [Table("Queries")]
    public class Query
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public string? Subject { get; set; }
        public string? Message { get; set; }

        public string? Reply { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}