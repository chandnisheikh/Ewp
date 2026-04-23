using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ewp.Data;
using Ewp.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;

namespace Ewp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= ROLE CHECK =================
        private bool IsEmployee()
        {
            return HttpContext.Session.GetString("Role") == "Employee";
        }

        private IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            if (!IsEmployee()) return RedirectToLogin();

            // ✅ GET EMAIL FROM SESSION (FIX)
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
                return RedirectToLogin();

            // ✅ FETCH EMPLOYEE USING EMAIL
            var employee = _context.Employees
                .Include(e => e.Department)
                .FirstOrDefault(e => e.Email == email);

            if (employee == null)
            {
                ViewBag.Error = "Employee not found!";
                return View();
            }

            // ✅ TASKS
            var tasks = _context.Tasks
                .Where(t => t.EmployeeId == employee.Id)
                .ToList();

            // ✅ EXPENSES
            var expenses = _context.Expenses
                .Where(e => e.EmployeeId == employee.Id)
                .ToList();

            ViewBag.Employee = employee;
            ViewBag.Tasks = tasks;
            ViewBag.Expenses = expenses;

            return View();
        }

        // ================= UPDATE TASK STATUS =================
        public IActionResult UpdateStatus(int id)
        {
            if (!IsEmployee()) return RedirectToLogin();

            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task != null)
            {
                task.Status = "Completed";
                _context.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }

        // ================= MY EXPENSES =================
        public IActionResult MyExpenses()
        {
            if (!IsEmployee()) return RedirectToLogin();

            var email = HttpContext.Session.GetString("UserEmail");

            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null) return RedirectToLogin();

            var data = _context.Expenses
                .Where(e => e.EmployeeId == employee.Id)
                .ToList();

            return View(data);
        }

        // ================= CREATE EXPENSE =================
        public IActionResult CreateExpense()
        {
            if (!IsEmployee()) return RedirectToLogin();
            return View();
        }

        [HttpPost]
        public IActionResult CreateExpense(Expense exp)
        {
            if (!IsEmployee()) return RedirectToLogin();

            var email = HttpContext.Session.GetString("UserEmail");

            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null) return RedirectToLogin();

            exp.EmployeeId = employee.Id;
            exp.Status = "Pending";
            exp.Date = DateTime.Now;

            _context.Expenses.Add(exp);
            _context.SaveChanges();

            return RedirectToAction("MyExpenses");
        }

        // ================= CREATE QUERY =================
        public IActionResult CreateQuery()
        {
            if (!IsEmployee()) return RedirectToLogin();
            return View();
        }

        [HttpPost]
        public IActionResult CreateQuery(Query q)
        {
            if (!IsEmployee()) return RedirectToLogin();

            var email = HttpContext.Session.GetString("UserEmail");

            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null) return RedirectToLogin();

            q.EmployeeId = employee.Id;
            q.CreatedAt = DateTime.Now;
            q.Status = "Pending";

            _context.Queries.Add(q);
            _context.SaveChanges();

            return RedirectToAction("MyQueries");
        }

        // ================= MY QUERIES =================
        public IActionResult MyQueries()
        {
            if (!IsEmployee()) return RedirectToLogin();

            var email = HttpContext.Session.GetString("UserEmail");

            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null) return RedirectToLogin();

            var data = _context.Queries
                .Where(q => q.EmployeeId == employee.Id)
                .OrderByDescending(q => q.CreatedAt)
                .ToList();

            return View(data);
        }
    }
}