
using BookEShop.DataAccess.Repository;
using BookEShop.Models;
using BookEShop.Models.ViewModels;
using BookEShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace BookEShopWeb.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCard cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType")
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCard shoppingCard)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCard.ApplicationUserId = claim.Value;

            ShoppingCard cardFromDb = _unitOfWork.ShoppingCard.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCard.ProductId);

            if(cardFromDb == null)
            {
                _unitOfWork.ShoppingCard.Add(shoppingCard);

                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCard,
                    _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
            }
            else
            {
                _unitOfWork.ShoppingCard.IncrementCount(cardFromDb,shoppingCard.Count);

                _unitOfWork.Save();
            }
         

            return RedirectToAction("Index");
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}