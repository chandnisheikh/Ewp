namespace Ewp.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = "Pending";

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}