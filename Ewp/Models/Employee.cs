using System;
using System.ComponentModel.DataAnnotations;

namespace Ewp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        // BASIC
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";

        // ✅ ADD THESE (FIX ERROR)
        public string Phone { get; set; } = "";
        public string Designation { get; set; } = "";

        // SALARY
        public decimal Salary { get; set; }
        public string? Role { get; set; }   // ✅ IMPORTANT FIX

        // RELATION
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}