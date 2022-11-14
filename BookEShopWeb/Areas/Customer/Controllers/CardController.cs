using System.Security.Claims;
using BookEShop.DataAccess.Repository;
using BookEShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookEShopWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCardVM ShoppingCardVM { get; set; }

        public int OrderTotal { get; set; }

        public CardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCardVM = new ShoppingCardVM()
            {
                ListCard = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties:"Product")
            };
            foreach(var card in ShoppingCardVM.ListCard)
            {
                card.Price = GetPriceBasedOdQuantity(card.Count, card.Product.Price, card.Product.Price50, card.Product.Price100);
                ShoppingCardVM.CardTotal += (card.Price * card.Count);
            }
       
            return View(ShoppingCardVM);
        }

        private double GetPriceBasedOdQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if(quantity <= 100)
                {
                    return price = price50;    
                }
                return price100;
            }
        }
    }
}
