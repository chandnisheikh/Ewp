using Microsoft.AspNetCore.Mvc;
using Ewp.Data;

namespace Ewp.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Manager")
                return RedirectToAction("Login", "Account");

            var employees = _context.Employees.ToList();
            return View(employees);
        }
    }
}