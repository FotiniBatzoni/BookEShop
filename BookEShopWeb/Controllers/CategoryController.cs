using Microsoft.AspNetCore.Mvc;

namespace BookEShopWeb.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
