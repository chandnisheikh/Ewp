using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ewp.Data;
using Ewp.Models;
using System.Linq;

namespace Ewp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= LIST =================
        public IActionResult Index()
        {
            var expenses = _context.Expenses
                .Include(e => e.Employee) // 🔥 join employee
                .ToList();

            return View(expenses);
        }

        // ================= CREATE (GET) =================
        public IActionResult Create()
        {
            // send employee list to dropdown
            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }

        // ================= CREATE (POST) =================
      
        [HttpPost]
        public IActionResult Create(Expense exp)
        {
            var username = HttpContext.Session.GetString("Username");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return Content("User not found");

            var emp = _context.Employees.FirstOrDefault(e => e.Email == user.Email);
            if (emp == null)
                return Content("Employee not found");

            // ✅ VERY IMPORTANT
            exp.EmployeeId = emp.Id;
            exp.Status = "Pending";

            _context.Expenses.Add(exp);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Employee");
        }

        // ================= DELETE =================
        public IActionResult Delete(int id)
        {
            var exp = _context.Expenses.Find(id);

            if (exp != null)
            {
                _context.Expenses.Remove(exp);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}