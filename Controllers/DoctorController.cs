using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ClinicQueue.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        private bool IsLoggedIn() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token"));

        private bool IsDoctor() =>
            HttpContext.Session.GetString("user_role")?.ToLower() == "doctor";

        public async Task<IActionResult> Queue()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsDoctor()) return View("~/Views/Shared/AccessDenied.cshtml");

            try
            {
                var queue = await _doctorService.GetQueueAsync();
                return View(queue);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.QueueEntry>());
            }
        }

        [HttpGet]
        public IActionResult AddPrescription(string appointmentId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsDoctor()) return View("~/Views/Shared/AccessDenied.cshtml");
            return View(new PrescriptionVM { AppointmentId = appointmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrescription(PrescriptionVM model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var ok = await _doctorService.AddPrescriptionAsync(model.AppointmentId, model);
                if (ok)
                {
                    TempData["Success"] = "Prescription saved!";
                    return RedirectToAction("Queue");
                }
                ModelState.AddModelError("", "Failed to save prescription");
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult AddReport(string appointmentId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsDoctor()) return View("~/Views/Shared/AccessDenied.cshtml");
            return View(new ReportVM { AppointmentId = appointmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReport(ReportVM model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var ok = await _doctorService.AddReportAsync(model.AppointmentId, model);
                if (ok)
                {
                    TempData["Success"] = "Report saved!";
                    return RedirectToAction("Queue");
                }
                ModelState.AddModelError("", "Failed to save report");
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}