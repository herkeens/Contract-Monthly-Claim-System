using CMCS_Auto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace CMCS_Auto.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitClaim(Claim model)
        {
            if (ModelState.IsValid)
            {
                int lecturerID = HttpContext.Session.GetInt32("UserID") ?? 0;

                if (lecturerID == 0)
                {
                    // If LecturerID is 0, redirect to login because session might be missing
                    return RedirectToAction("Login", "Account");
                }

                model.LecturerID = lecturerID;
                model.SubmissionDate = DateTime.Now; // Set submission date to current time
                model.FinalPayment = model.HoursWorked * model.HourlyRate;
                model.Status =  "Pending"; // Set to "Pending" if not set already

                _context.Claims.Add(model);
                _context.SaveChanges();

                return RedirectToAction("ViewClaims");
            }
            else
            {
                // Log or display validation errors to understand why ModelState is invalid
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Use logging in production
                }
            }

            return View(model); // If model is invalid, return the same view with the model data
        }



        [HttpGet]
        public IActionResult ViewClaims()
        {
            int? lecturerID = HttpContext.Session.GetInt32("UserID");
            var claims = _context.Claims.Where(c => c.LecturerID == lecturerID).ToList();
            return View(claims);
        }
    }
}
