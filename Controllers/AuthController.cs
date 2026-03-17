using ClinicQueue.Services;
using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ClinicQueue.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // show login page
        [HttpGet]
        public IActionResult Login()
        {
            // if already logged in, go to dashboard
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(ApiClient.TOKEN_KEY)))
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        // handle login form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _authService.LoginAsync(model);

                if (response == null || string.IsNullOrEmpty(response.Token))
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }

                // save token and user info in session
                HttpContext.Session.SetString(ApiClient.TOKEN_KEY, response.Token);
                HttpContext.Session.SetString("user_name", response.User?.Name ?? "");
                HttpContext.Session.SetString("user_role", response.User?.Role ?? "");
                HttpContext.Session.SetString("user_id", response.User?.Id ?? "");
                HttpContext.Session.SetString("clinic_name", response.User?.ClinicName ?? "Clinic");

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Login failed: " + ex.Message);
                return View(model);
            }
        }

        // logout - clear session
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}