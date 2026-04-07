using Microsoft.EntityFrameworkCore;
using Ewp.Models;
namespace Ewp.Data   // ✅ THIS IS VERY IMPORTANT
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
