using System.Security.Claims;
using BookEShop.DataAccess.Repository;
using BookEShop.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BookEShopWeb.ViewComponents
{
    public class ShoppingCardViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCardViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionCard) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCard));
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionCard,
                        _unitOfWork.ShoppingCard.GetAll(u=>u.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCard));
                }
            }
            else 
            { 
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
