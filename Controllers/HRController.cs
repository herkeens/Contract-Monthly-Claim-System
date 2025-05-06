using CMCS_Auto.Models;
using CMCS_Auto.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;

namespace CMCS_Auto.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ApprovedClaimsReport(DateTime? startDate, DateTime? endDate)
        {
            var approvedClaims = _context.Claims
                .Where(c => c.Status == "Approved")
                .Join(_context.Users,
                      claim => claim.LecturerID,
                      user => user.UserID,
                      (claim, user) => new
                      {
                          claim.ClaimID,
                          LecturerName = user.FirstName + " " + user.LastName,
                          claim.SubmissionDate,
                          claim.HoursWorked,
                          claim.HourlyRate,
                          claim.FinalPayment,
                          claim.Status
                      });

            // Apply date range filter if specified
            if (startDate.HasValue)
            {
                approvedClaims = approvedClaims.Where(c => c.SubmissionDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                approvedClaims = approvedClaims.Where(c => c.SubmissionDate <= endDate.Value);
            }

            var claimsList = approvedClaims.ToList();

            ViewBag.TotalClaims = _context.Claims.Count();
            ViewBag.ApprovedClaims = _context.Claims.Count(c => c.Status == "Approved");
            ViewBag.PendingClaims = _context.Claims.Count(c => c.Status == "Pending");
            ViewBag.RejectedClaims = _context.Claims.Count(c => c.Status == "Rejected");

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(claimsList);
        }

        public IActionResult DownloadApprovedClaimsCSV(DateTime? startDate, DateTime? endDate)
        {
            var approvedClaims = _context.Claims
                .Where(c => c.Status == "Approved")
                .Join(
                    _context.Users,
                    claim => claim.LecturerID,
                    user => user.UserID,
                    (claim, user) => new
                    {
                        LecturerID = user.UserID,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        SubmissionDate = claim.SubmissionDate,
                        HoursWorked = claim.HoursWorked,
                        HourlyRate = claim.HourlyRate,
                        FinalPayment = claim.FinalPayment
                    });

            // Apply date range filter if specified
            if (startDate.HasValue)
            {
                approvedClaims = approvedClaims.Where(c => c.SubmissionDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                approvedClaims = approvedClaims.Where(c => c.SubmissionDate <= endDate.Value);
            }

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("LecturerID,FirstName,LastName,SubmissionDate,HoursWorked,HourlyRate,FinalPayment");

            foreach (var claim in approvedClaims)
            {
                csvBuilder.AppendLine($"{claim.LecturerID},{claim.FirstName},{claim.LastName},{claim.SubmissionDate:MM/dd/yyyy},{claim.HoursWorked},{claim.HourlyRate:F2},{claim.FinalPayment:F2}");
            }

            var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(bytes, "text/csv", "ApprovedClaimsReport.csv");
        }

        [HttpGet]
        public IActionResult UpdateLecturer(int? userID)
        {
            // Get all lecturers
            var lecturers = _context.Users.Where(u => u.Role == "Lecturer").ToList();

            // Find the selected lecturer if userID is provided
            var selectedLecturer = userID.HasValue ? _context.Users.FirstOrDefault(u => u.UserID == userID) : null;

            // Create the HRViewModel
            var model = new HRViewModel
            {
                Lecturers = lecturers,
                SelectedLecturer = selectedLecturer
            };

            return View(model); // Pass the HRViewModel to the view
        }

        [HttpPost]
        public IActionResult UpdateLecturer(HRViewModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == model.SelectedLecturer.UserID);
            if (user != null)
            {
                // Update the lecturer details
                user.FirstName = model.SelectedLecturer.FirstName;
                user.LastName = model.SelectedLecturer.LastName;
                user.Email = model.SelectedLecturer.Email;
                user.ContactNumber = model.SelectedLecturer.ContactNumber;
                user.NQFLevel = model.SelectedLecturer.NQFLevel;

                _context.SaveChanges();
            }
            return RedirectToAction("ApprovedClaimsReport");
        }


    }
}
