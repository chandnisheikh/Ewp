using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ewp.Data;
using Ewp.Models;
using System.Linq;

namespace Ewp.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public IActionResult Index()
        {
            var tasks = _context.Tasks
                .Include(t => t.Employee)
                .ToList();

            return View(tasks);
        }

        // CREATE
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(TaskItem t)
        {
            ViewBag.Employees = _context.Employees.ToList();

            if (!ModelState.IsValid)
                return View(t);

            t.Status = "Pending";

            _context.Tasks.Add(t);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ✅ ONLY ONE METHOD (FINAL FIX)
        [HttpGet]
        public IActionResult UpdateStatus(int id, string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest();

            var task = _context.Tasks.Find(id);

            if (task == null)
                return NotFound();

            task.Status = status;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var task = _context.Tasks.Find(id);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}