using CMCS_Auto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CMCS_Auto.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // This action serves as the main dashboard for lecturers
        public IActionResult Dashboard()
        {
            // Redirects directly to the SubmitClaim page for lecturers
            return RedirectToAction("SubmitClaim", "Claim");
        }
    }
}
