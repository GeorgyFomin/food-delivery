using Microsoft.AspNetCore.Mvc;

namespace WebASP_MVC.Controllers
{
    public class FoodDeliveryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
