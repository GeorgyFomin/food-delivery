using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebASP_MVC.Controllers
{
    public class FoodDeliveryController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
