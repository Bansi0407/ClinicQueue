using Microsoft.AspNetCore.Mvc;

namespace ClinicQueue.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // check login
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token")))
                return RedirectToAction("Login", "Auth");

            ViewBag.UserName = HttpContext.Session.GetString("user_name");
            ViewBag.UserRole = HttpContext.Session.GetString("user_role");
            ViewBag.ClinicName = HttpContext.Session.GetString("clinic_name");

            return View();
        }
    }
}