using CMCS_Auto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CMCS_Auto.Controllers
{
    public class ClaimApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ViewClaims()
        {
            // Retrieve pending claims
            var pendingClaims = _context.Claims.Where(c => c.Status == "Pending").ToList();

            // Retrieve historical claims (Approved or Rejected)
            var historicalClaims = _context.Claims.Where(c => c.Status == "Approved" || c.Status == "Rejected").ToList();

            // Pass both lists to the view using ViewData or a view model
            ViewData["PendingClaims"] = pendingClaims;
            ViewData["HistoricalClaims"] = historicalClaims;

            // Retrieve criteria from TempData and parse them to appropriate types
            ViewData["MinHoursWorked"] = TempData["MinHoursWorked"] != null ? int.Parse((string)TempData["MinHoursWorked"]) : (int?)null;
            ViewData["MinHourlyRate"] = TempData["MinHourlyRate"] != null ? decimal.Parse((string)TempData["MinHourlyRate"]) : (decimal?)null;
            TempData.Keep(); // Keep TempData across multiple requests

            return View();
        }


        [HttpPost]
        public IActionResult SetApprovalCriteria(int minHoursWorked, decimal minHourlyRate)
        {
            // Store criteria in TempData as strings
            TempData["MinHoursWorked"] = minHoursWorked.ToString();
            TempData["MinHourlyRate"] = minHourlyRate.ToString();

            return RedirectToAction("ViewClaims");
        }


        [HttpPost]
        public IActionResult ApproveAllMatchingClaims()
        {
            // Retrieve criteria from TempData and parse to appropriate types
            int minHoursWorked = TempData["MinHoursWorked"] != null ? int.Parse((string)TempData["MinHoursWorked"]) : 0;
            decimal minHourlyRate = TempData["MinHourlyRate"] != null ? decimal.Parse((string)TempData["MinHourlyRate"]) : 0m;

            // Find pending claims that meet the criteria
            var matchingClaims = _context.Claims
                .Where(c => c.Status == "Pending" &&
                            c.HoursWorked >= minHoursWorked &&
                            c.HourlyRate >= minHourlyRate)
                .ToList();

            // Update status to "Approved" for matching claims
            foreach (var claim in matchingClaims)
            {
                claim.Status = "Approved";
            }

            _context.SaveChanges();

            return RedirectToAction("ViewClaims");
        }


        [HttpPost]
        public IActionResult ApproveClaim(int claimID)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimID == claimID);
            if (claim != null)
            {
                claim.Status = "Approved";
                _context.SaveChanges();
            }
            return RedirectToAction("ViewClaims");
        }

        [HttpPost]
        public IActionResult RejectClaim(int claimID)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimID == claimID);
            if (claim != null)
            {
                claim.Status = "Rejected";
                _context.SaveChanges();
            }
            return RedirectToAction("ViewClaims");
        }
    }
}
