using Microsoft.AspNetCore.Mvc;
using Ewp.Data;

namespace Ewp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Employee")
                return RedirectToAction("Login", "Account");

            var employees = _context.Employees.ToList();
            return View(employees);
        }
    }
}