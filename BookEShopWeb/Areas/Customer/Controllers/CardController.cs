using Microsoft.AspNetCore.Mvc;

namespace BookEShopWeb.Areas.Customer.Controllers
{
    public class CardController : Controller
    {
        [Area("Customer")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
