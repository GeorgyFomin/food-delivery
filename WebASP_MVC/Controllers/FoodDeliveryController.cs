using Microsoft.AspNetCore.Mvc;
#if role || cookies
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
#elif cookies
[Authorize]
        public IActionResult Index()
        {
            return View();
        }
#else
        public IActionResult Index()
        {
            return View();
        }
#endif
    }
}
