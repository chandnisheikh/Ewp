using Microsoft.AspNetCore.Mvc;
using Ewp.Data;
using Ewp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
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

        // ================= LOGIN PAGE =================
        public IActionResult Login()
        {
            return View();
        }

        // ================= LOGIN POST =================
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // ✅ SESSION (FIXED - VERY IMPORTANT)
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email); // ⭐ MAIN FIX

                // ================= LOGIN HISTORY =================
                var emp = _context.Employees
                    .Include(e => e.Department)
                    .FirstOrDefault(e => e.Email == user.Email);

                var history = new LoginHistory
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    LoginTime = DateTime.Now,
                    DepartmentName = emp != null ? emp.Department?.Name : "Admin"
                };

                _context.LoginHistories.Add(history);
                _context.SaveChanges();

                // ================= ROLE REDIRECT =================
                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");

                if (user.Role == "Manager")
                    return RedirectToAction("Dashboard", "Manager");

                return RedirectToAction("Dashboard", "Employee");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

        // ================= REGISTER PAGE =================
        public IActionResult Register()
        {
            return View();
        }

        // ================= REGISTER POST =================
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Error = "Username and Password are required";
                return View(user);
            }

            // Default role if not provided
            if (string.IsNullOrEmpty(user.Role))
                user.Role = "Employee";

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}