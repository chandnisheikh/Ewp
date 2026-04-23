using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ewp.Data;
using Ewp.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;

namespace Ewp.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= ROLE CHECK =================
        private bool IsManager()
        {
            return HttpContext.Session.GetString("Role") == "Manager";
        }

        private IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            if (!IsManager()) return RedirectToLogin();

            ViewBag.TotalEmployees = _context.Employees.Count();

            ViewBag.TotalTasks = _context.Tasks.Count();
            ViewBag.CompletedTasks = _context.Tasks.Count(t => t.Status == "Completed");
            ViewBag.PendingTasks = _context.Tasks.Count(t => t.Status == "Pending");

            return View();
        }

        // ================= EMPLOYEES =================
        public IActionResult Employees()
        {
            if (!IsManager()) return RedirectToLogin();

            var data = _context.Employees
                .Include(e => e.Department)
                .ToList();

            return View(data);
        }

        // ================= CREATE EMPLOYEE =================
        public IActionResult CreateEmployee()
        {
            if (!IsManager()) return RedirectToLogin();

            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CreateEmployee(Employee emp)
        {
            if (!IsManager()) return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(emp);
            }

            _context.Employees.Add(emp);
            _context.SaveChanges();

            return RedirectToAction("Employees");
        }

        // ================= EDIT EMPLOYEE =================
        public IActionResult EditEmployee(int id)
        {
            if (!IsManager()) return RedirectToLogin();

            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();

            ViewBag.Departments = _context.Departments.ToList();
            return View(emp);
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee emp)
        {
            if (!IsManager()) return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(emp);
            }

            _context.Employees.Update(emp);
            _context.SaveChanges();

            return RedirectToAction("Employees");
        }

        // ================= DELETE EMPLOYEE =================
        public IActionResult DeleteEmployee(int id)
        {
            if (!IsManager()) return RedirectToLogin();

            var emp = _context.Employees.Find(id);

            if (emp != null)
            {
                _context.Employees.Remove(emp);
                _context.SaveChanges();
            }

            return RedirectToAction("Employees");
        }

        // ================= TASK STATUS =================
        public IActionResult TaskStatus()
        {
            if (!IsManager()) return RedirectToLogin();

            var data = _context.Tasks
                .Include(t => t.Employee)
                .ToList();

            return View(data);
        }

        // ================= ASSIGN TASK =================
        public IActionResult AssignTask()
        {
            if (!IsManager()) return RedirectToLogin();

            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult AssignTask(TaskItem task)
        {
            if (!IsManager()) return RedirectToLogin();

            ViewBag.Employees = _context.Employees.ToList();

            if (!ModelState.IsValid)
                return View(task);

            if (task.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select employee");
                return View(task);
            }

            task.Title ??= "No Title";
            task.Description ??= "No Description";
            task.Status = "Pending";
            task.CreatedAt = DateTime.Now;

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return RedirectToAction("TaskStatus");
        }

        // ================= EXPENSES =================
        public IActionResult Expenses()
        {
            if (!IsManager()) return RedirectToLogin();

            var data = _context.Expenses
                .Include(e => e.Employee)
                .ToList();

            return View(data);
        }

        // ================= APPROVE EXPENSE =================
        public IActionResult ApproveExpense(int id)
        {
            if (!IsManager()) return RedirectToLogin();

            var exp = _context.Expenses.FirstOrDefault(x => x.Id == id);

            if (exp != null)
            {
                exp.Status = "Approved";
                _context.SaveChanges();
            }

            return RedirectToAction("Expenses");
        }

        // ================= REJECT EXPENSE =================
        public IActionResult RejectExpense(int id)
        {
            if (!IsManager()) return RedirectToLogin();

            var exp = _context.Expenses.FirstOrDefault(x => x.Id == id);

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
            if (!IsManager()) return RedirectToLogin();

            var data = _context.Queries
                .Include(q => q.Employee)
                .OrderByDescending(q => q.CreatedAt)
                .ToList();

            return View(data);
        }
    }
}