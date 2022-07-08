using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
#if role || cookies
using System.Security.Claims;
#endif

namespace WebASP_MVC.Controllers
{
    public class FoodDeliveryController : Controller
    {
#if role
        [Authorize(Roles = "admin, user")]
        public IActionResult Index()
        {
            string? role = User?.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
            return View();
        }
        [Authorize(Roles = "admin")]
        public IActionResult About()
        {
            return Content("Вход только для администратора");
        }
#elif cookies || Identity
        [Authorize]
#else
        public IActionResult Index()
        {
            return View();
        }
#endif
    }
}
