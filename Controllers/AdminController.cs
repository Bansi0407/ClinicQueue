using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ClinicQueue.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private bool IsLoggedIn() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token"));

        private bool IsAdmin() =>
            HttpContext.Session.GetString("user_role")?.ToLower() == "admin";

        public async Task<IActionResult> Clinic()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsAdmin()) return View("~/Views/Shared/AccessDenied.cshtml");

            try
            {
                var clinic = await _adminService.GetClinicAsync();
                return View(clinic);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model: null);
            }
        }

        public async Task<IActionResult> Users()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsAdmin()) return View("~/Views/Shared/AccessDenied.cshtml");

            try
            {
                var users = await _adminService.GetUsersAsync();
                return View(users);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.User>());
            }
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsAdmin()) return View("~/Views/Shared/AccessDenied.cshtml");
            return View(new CreateUserVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserVM model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsAdmin()) return View("~/Views/Shared/AccessDenied.cshtml");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _adminService.CreateUserAsync(model);
                TempData["Success"] = $"User '{model.Name}' created successfully!";
                return RedirectToAction("Users");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                // shows real api error now
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }
    }
}








/*using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ClinicQueue.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // check login helper
        private bool IsLoggedIn() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token"));

        private bool IsAdmin() =>
            HttpContext.Session.GetString("user_role")?.ToLower() == "admin";

        public async Task<IActionResult> Clinic()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsAdmin()) return View("~/Views/Shared/AccessDenied.cshtml");

            try
            {
                var clinic = await _adminService.GetClinicAsync();
                return View(clinic);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model: null);
            }
        }

        public async Task<IActionResult> Users()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsAdmin()) return View("~/Views/Shared/AccessDenied.cshtml");

            try
            {
                var users = await _adminService.GetUsersAsync();
                return View(users);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ClinicQueue.Models.User>());
            }
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!IsAdmin()) return View("~/Views/Shared/AccessDenied.cshtml");
            return View(new CreateUserVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserVM model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var ok = await _adminService.CreateUserAsync(model);
                if (ok)
                {
                    TempData["Success"] = "User created successfully!";
                    return RedirectToAction("Users");
                }
                ModelState.AddModelError("", "Failed to create user");
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
}*/

