using ClinicQueue.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicQueue.Controllers
{
    public class ReceptionistController : Controller
    {
        private readonly IReceptionService _receptionService;

        public ReceptionistController(IReceptionService receptionService)
        {
            _receptionService = receptionService;
        }

        private bool IsLoggedIn() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token"));

        private bool IsReceptionist() =>
            HttpContext.Session.GetString("user_role")?.ToLower() == "receptionist";

        public async Task<IActionResult> DailyQueue(string? date)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsReceptionist()) return View("~/Views/Shared/AccessDenied.cshtml");

            // default to today
            var selectedDate = string.IsNullOrEmpty(date)
                ? DateTime.Today.ToString("yyyy-MM-dd")
                : date;

            ViewBag.SelectedDate = selectedDate;

            try
            {
                var queue = await _receptionService.GetDailyQueueAsync(selectedDate);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string queueId, string status, string date)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                await _receptionService.UpdateStatusAsync(queueId, status);
                TempData["Success"] = "Status updated!";
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("DailyQueue", new { date });
        }
    }
}