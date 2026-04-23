using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ewp.Data;
using Ewp.Models;
using System.Linq;

namespace Ewp.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public IActionResult Index()
        {
            var data = _context.Salaries
                .Include(s => s.Employee)
                .ToList();

            return View(data);
        }

        // CREATE PAGE
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }

        // CREATE POST (🔥 FIXED)
        [HttpPost]
        public IActionResult Create(Salary s)
        {
            ViewBag.Employees = _context.Employees.ToList();

            if (!ModelState.IsValid)
                return View(s);

            if (s.EmployeeId == 0)
            {
                ViewBag.Error = "Please select employee";
                return View(s);
            }

            _context.Salaries.Add(s);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}