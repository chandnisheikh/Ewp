using Microsoft.AspNetCore.Mvc;
using Ewp.Data;
using Ewp.Models;
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

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var employees = _context.Employees.ToList();
            return View(employees);
        }

        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee emp)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(emp);

            _context.Employees.Add(emp);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var emp = _context.Employees.Find(id);
            return View(emp);
        }

        [HttpPost]
        public IActionResult Edit(Employee emp)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            _context.Employees.Update(emp);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ✅ SAFE DELETE
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var emp = _context.Employees.Find(id);
            _context.Employees.Remove(emp);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.TotalEmployees = _context.Employees.Count();
            ViewBag.TotalAdmins = _context.Users.Count(u => u.Role == "Admin");
            ViewBag.TotalManagers = _context.Users.Count(u => u.Role == "Manager");
            ViewBag.TotalUsers = _context.Users.Count(u => u.Role == "Employee");

            return View();
        }
    }
}