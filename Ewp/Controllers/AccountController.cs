using Microsoft.AspNetCore.Mvc;
using Ewp.Data;
using Ewp.Models;
using System.Linq;

namespace Ewp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Login()
        {
            return View();
        }

        // POST
        [HttpPost]
        public IActionResult Login(User user)
        {
            var u = _context.Users
                .FirstOrDefault(x => x.Username == user.Username && x.Password == user.Password);

            if (u != null)
            {
                // ✅ Store Session
                HttpContext.Session.SetString("Username", u.Username);
                HttpContext.Session.SetString("Role", u.Role);
                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Admin");

                else if (user.Role == "Manager")
                    return RedirectToAction("Index", "Manager");

                else
                    return RedirectToAction("Index", "Employee");

            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

        // ✅ Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}