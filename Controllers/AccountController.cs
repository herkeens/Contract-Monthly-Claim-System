using CMCS_Auto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace CMCS_Auto.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Users.Add(model);
                    _context.SaveChanges();
                    Console.WriteLine("User registered successfully and redirected to login.");
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving user: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while registering the user.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetInt32("UserID", user.UserID);

                return user.Role switch
                {
                    "Lecturer" => RedirectToAction("SubmitClaim", "Claim"),
                    "Coordinator" => RedirectToAction("ViewClaims", "ClaimApproval"),
                    "AcademicManager" => RedirectToAction("ViewClaims", "ClaimApproval"),
                    "HR" => RedirectToAction("ApprovedClaimsReport", "HR"),
                    _ => RedirectToAction("Dashboard", "Home")
                };
            }
            ViewBag.ErrorMessage = "Invalid login credentials.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
