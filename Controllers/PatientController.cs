using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicQueue.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IDoctorsCacheService _cache;

        public PatientController(IPatientService patientService,
                                 IDoctorsCacheService cache)
        {
            _patientService = patientService;
            _cache = cache;
        }

        private bool IsLoggedIn() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token"));

        private bool IsPatient() =>
            HttpContext.Session.GetString("user_role")?.ToLower() == "patient";

        private string GetPatientId() =>
            HttpContext.Session.GetString("user_id") ?? "";

        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsPatient()) return View("~/Views/Shared/AccessDenied.cshtml");

            var doctors = await _patientService.GetDoctorsAsync();

            ViewBag.Doctors = new SelectList(doctors, "Id", "Name");
            ViewBag.TimeSlots = GetSlots();
            ViewBag.DoctorsLoaded = doctors.Any();
            ViewBag.DoctorCount = doctors.Count;
            ViewBag.PatientId = GetPatientId();

            return View(new BookAppointmentVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(BookAppointmentVM model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsPatient()) return View("~/Views/Shared/AccessDenied.cshtml");

            if (!ModelState.IsValid)
            {
                await RefillDropdowns();
                return View(model);
            }

            try
            {
                var patientId = GetPatientId();
                await _patientService.BookAppointmentAsync(model, patientId);
                TempData["Success"] = "Appointment booked successfully!";
                return RedirectToAction("MyAppointments");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Booking failed: {ex.Message}");
                await RefillDropdowns();
                return View(model);
            }
        }

        public async Task<IActionResult> MyAppointments()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyAppointmentsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Appointment>());
            }
        }

        public async Task<IActionResult> AppointmentDetails(string id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var appt = await _patientService.GetAppointmentByIdAsync(id);
                if (appt == null) return RedirectToAction("MyAppointments");
                return View(appt);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MyAppointments");
            }
        }

        public async Task<IActionResult> Prescriptions()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyPrescriptionsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Prescription>());
            }
        }

        public async Task<IActionResult> Reports()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyReportsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Report>());
            }
        }

        private async Task RefillDropdowns()
        {
            var doctors = await _patientService.GetDoctorsAsync();
            ViewBag.Doctors = new SelectList(doctors, "Id", "Name");
            ViewBag.TimeSlots = GetSlots();
            ViewBag.DoctorsLoaded = doctors.Any();
            ViewBag.DoctorCount = doctors.Count;
            ViewBag.PatientId = GetPatientId();
        }

        // time slots in API expected format HH:MM-HH:MM
        private static SelectList GetSlots()
        {
            var slots = new[]
            {
                "09:00-09:15",
                "09:15-09:30",
                "09:30-09:45",
                "09:45-10:00",
                "10:00-10:15",
                "10:15-10:30",
                "10:30-10:45",
                "10:45-11:00",
                "11:00-11:15",
                "11:15-11:30",
                "11:30-11:45",
                "11:45-12:00",
                "12:00-12:15",
                "12:15-12:30",
                "14:00-14:15",
                "14:15-14:30",
                "14:30-14:45",
                "14:45-15:00",
                "15:00-15:15",
                "15:15-15:30",
                "15:30-15:45",
                "15:45-16:00",
                "16:00-16:15",
                "16:15-16:30",
                "16:30-16:45",
                "16:45-17:00",
                "17:00-17:15",
                "17:15-17:30"
            };
            return new SelectList(slots);
        }
    }
}





/*using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicQueue.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        private bool IsLoggedIn() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token"));

        private bool IsPatient() =>
            HttpContext.Session.GetString("user_role")?.ToLower() == "patient";

        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsPatient()) return View("~/Views/Shared/AccessDenied.cshtml");

            try
            {
                var doctors = await _patientService.GetDoctorsAsync();
                ViewBag.Doctors = new SelectList(doctors, "Id", "Name");
                ViewBag.TimeSlots = GetSlots();
            }
            catch
            {
                ViewBag.Doctors = new SelectList(new List<object>());
                ViewBag.TimeSlots = GetSlots();
            }

            return View(new BookAppointmentVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(BookAppointmentVM model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                await RefillDropdowns();
                return View(model);
            }

            try
            {
                var ok = await _patientService.BookAppointmentAsync(model);
                if (ok)
                {
                    TempData["Success"] = "Appointment booked!";
                    return RedirectToAction("MyAppointments");
                }
                ModelState.AddModelError("", "Booking failed");
                await RefillDropdowns();
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await RefillDropdowns();
                return View(model);
            }
        }

        public async Task<IActionResult> MyAppointments()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyAppointmentsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Appointment>());
            }
        }

        public async Task<IActionResult> AppointmentDetails(string id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var appt = await _patientService.GetAppointmentByIdAsync(id);
                if (appt == null) return RedirectToAction("MyAppointments");
                return View(appt);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public async Task<IActionResult> Prescriptions()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyPrescriptionsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Prescription>());
            }
        }

        public async Task<IActionResult> Reports()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyReportsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Report>());
            }
        }

        private async Task RefillDropdowns()
        {
            try
            {
                var docs = await _patientService.GetDoctorsAsync();
                ViewBag.Doctors = new SelectList(docs, "Id", "Name");
            }
            catch
            {
                ViewBag.Doctors = new SelectList(new List<object>());
            }
            ViewBag.TimeSlots = GetSlots();
        }

        private static SelectList GetSlots()
        {
            var s = new[]
            {
                "09:00 AM","09:30 AM","10:00 AM","10:30 AM",
                "11:00 AM","11:30 AM","12:00 PM","02:00 PM",
                "02:30 PM","03:00 PM","03:30 PM","04:00 PM","05:00 PM"
            };
            return new SelectList(s);
        }
    }
}*/






/*using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicQueue.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IDoctorsCacheService _cache;

        public PatientController(IPatientService patientService,
                                 IDoctorsCacheService cache)
        {
            _patientService = patientService;
            _cache = cache;
        }

        private bool IsLoggedIn() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token"));

        private bool IsPatient() =>
            HttpContext.Session.GetString("user_role")?.ToLower() == "patient";

        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsPatient()) return View("~/Views/Shared/AccessDenied.cshtml");

            var doctors = await _patientService.GetDoctorsAsync();

            ViewBag.Doctors = new SelectList(doctors, "Id", "Name");
            ViewBag.TimeSlots = GetSlots();
            ViewBag.DoctorsLoaded = doctors.Any();
            ViewBag.DoctorCount = doctors.Count;
            ViewBag.CacheHasDocs = _cache.HasDoctors();

            return View(new BookAppointmentVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(BookAppointmentVM model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsPatient()) return View("~/Views/Shared/AccessDenied.cshtml");

            if (!ModelState.IsValid)
            {
                await RefillDropdowns();
                return View(model);
            }

            try
            {
                await _patientService.BookAppointmentAsync(model);
                TempData["Success"] = "Appointment booked successfully!";
                return RedirectToAction("MyAppointments");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Booking failed: {ex.Message}");
                await RefillDropdowns();
                return View(model);
            }
        }

        public async Task<IActionResult> MyAppointments()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyAppointmentsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Appointment>());
            }
        }

        public async Task<IActionResult> AppointmentDetails(string id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var appt = await _patientService.GetAppointmentByIdAsync(id);
                if (appt == null) return RedirectToAction("MyAppointments");
                return View(appt);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MyAppointments");
            }
        }

        public async Task<IActionResult> Prescriptions()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyPrescriptionsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Prescription>());
            }
        }

        public async Task<IActionResult> Reports()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            try
            {
                var list = await _patientService.GetMyReportsAsync();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.Report>());
            }
        }

        private async Task RefillDropdowns()
        {
            var doctors = await _patientService.GetDoctorsAsync();
            ViewBag.Doctors = new SelectList(doctors, "Id", "Name");
            ViewBag.TimeSlots = GetSlots();
            ViewBag.DoctorsLoaded = doctors.Any();
            ViewBag.DoctorCount = doctors.Count;
        }

        private static SelectList GetSlots()
        {
            var s = new[]
            {
                "09:00 AM", "09:30 AM", "10:00 AM", "10:30 AM",
                "11:00 AM", "11:30 AM", "12:00 PM", "02:00 PM",
                "02:30 PM", "03:00 PM", "03:30 PM", "04:00 PM",
                "05:00 PM"
            };
            return new SelectList(s);
        }
    }
}*/


