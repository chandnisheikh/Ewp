using System;

namespace Ewp.Models
{
    public class Salary
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        // ✅ AUTO CALCULATE
        public decimal TotalSalary => BaseSalary + Bonus - Deduction;
    }
}