using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ewp.Data;
using Ewp.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Ewp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= ROLE CHECK =================
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        private IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToLogin();

            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalEmployees = _context.Employees.Count();
            ViewBag.TotalTasks = _context.Tasks.Count();
            ViewBag.TotalExpenses = _context.Expenses.Count();

            return View();
        }

        // ================= SYSTEM REPORT (✅ FIX ADDED) =================
        public IActionResult SystemReport()
        {
            if (!IsAdmin()) return RedirectToLogin();

            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalEmployees = _context.Employees.Count();
            ViewBag.TotalTasks = _context.Tasks.Count();
            ViewBag.TotalExpenses = _context.Expenses.Count();
            ViewBag.TotalSalary = _context.Salaries.Sum(s => (decimal?)s.BaseSalary) ?? 0;

            return View();
        }

        // ================= USERS =================
        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var data = _context.LoginHistories
                .OrderByDescending(l => l.LoginTime)
                .ToList();

            return View(data);
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if (!IsAdmin()) return RedirectToLogin();

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Error = "Username & Password required";
                return View(user);
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Users");
        }

        // ================= EMPLOYEES =================
        public IActionResult Employees()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var data = _context.Employees
                .Include(e => e.Department)
                .ToList();

            return View(data);
        }

        public IActionResult CreateEmployee()
        {
            if (!IsAdmin()) return RedirectToLogin();

            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CreateEmployee(Employee emp, string Username, string Password)
        {
            if (!IsAdmin()) return RedirectToLogin();

            if (string.IsNullOrEmpty(emp.Name) || string.IsNullOrEmpty(emp.Email))
            {
                ViewBag.Error = "Name & Email required";
                return View(emp);
            }

            emp.Role = "Employee";

            _context.Employees.Add(emp);
            _context.SaveChanges();

            var user = new User
            {
                Username = Username,
                Password = Password,
                Email = emp.Email,
                Role = "Employee"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Employees");
        }

        public IActionResult EditEmployee(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();

            ViewBag.Departments = _context.Departments.ToList();
            return View(emp);
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee emp)
        {
            if (!IsAdmin()) return RedirectToLogin();

            _context.Employees.Update(emp);
            _context.SaveChanges();

            return RedirectToAction("Employees");
        }

        public IActionResult DeleteEmployee(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var emp = _context.Employees.Find(id);
            if (emp != null)
            {
                _context.Employees.Remove(emp);
                _context.SaveChanges();
            }

            return RedirectToAction("Employees");
        }

        // ================= TASKS =================
        public IActionResult Tasks()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var data = _context.Tasks
                .Include(t => t.Employee)
                .ToList();

            return View(data);
        }

        // ================= SALARY =================
        public IActionResult SalaryList()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var data = _context.Salaries
                .Include(s => s.Employee)
                .ToList();

            return View(data);
        }

        public IActionResult GenerateSalaryAll()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var employees = _context.Employees.ToList();

            foreach (var emp in employees)
            {
                bool exists = _context.Salaries.Any(s =>
                    s.EmployeeId == emp.Id &&
                    s.Date.Month == DateTime.Now.Month &&
                    s.Date.Year == DateTime.Now.Year);

                if (!exists)
                {
                    _context.Salaries.Add(new Salary
                    {
                        EmployeeId = emp.Id,
                        BaseSalary = emp.Salary,
                        Bonus = 1000,
                        Deduction = 500,
                        Date = DateTime.Now
                    });
                }
            }

            _context.SaveChanges();

            return RedirectToAction("SalaryList");
        }

        // ================= EXPENSE =================
        public IActionResult Expenses()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var data = _context.Expenses
                .Include(e => e.Employee)
                .ToList();

            return View(data);
        }

        public IActionResult ApproveExpense(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var exp = _context.Expenses.Find(id);

            if (exp != null)
            {
                exp.Status = "Approved";
                _context.SaveChanges();
            }

            return RedirectToAction("Expenses");
        }

        public IActionResult RejectExpense(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var exp = _context.Expenses.Find(id);

            if (exp != null)
            {
                exp.Status = "Rejected";
                _context.SaveChanges();
            }

            return RedirectToAction("Expenses");
        }

        // ================= QUERIES =================
        public IActionResult Queries()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var data = _context.Queries
                .Include(q => q.Employee)
                .OrderByDescending(q => q.CreatedAt)
                .ToList();

            return View(data);
        }

        public IActionResult ReplyQuery(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var q = _context.Queries
                .Include(x => x.Employee)
                .FirstOrDefault(x => x.Id == id);

            if (q == null) return NotFound();

            return View(q);
        }

        [HttpPost]
        public IActionResult ReplyQuery(int id, string reply)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var query = _context.Queries.Find(id);

            if (query != null)
            {
                query.Reply = reply;
                query.Status = "Resolved";
                _context.SaveChanges();
            }

            return RedirectToAction("Queries");
        }
    }
}