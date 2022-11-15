﻿using System.Security.Claims;
using BookEShop.DataAccess.Repository;
using BookEShop.Models.ViewModels;
using BookEShop.Models;
using BookEShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BookEShopWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
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
                includeProperties:"Product"),
                OrderHeader = new()
            };
            foreach(var card in ShoppingCardVM.ListCard)
            {
                card.Price = GetPriceBasedOdQuantity(card.Count, card.Product.Price, card.Product.Price50, card.Product.Price100);
                ShoppingCardVM.OrderHeader.OrderTotal += (card.Price * card.Count);
            }
       
            return View(ShoppingCardVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
             var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCardVM = new ShoppingCardVM()
            {
                ListCard = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value,
               includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCardVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                u => u.Id == claim.Value);

            //ShoppingCardVM.OrderHeader.Name = ShoppingCardVM.OrderHeader.ApplicationUser.Name;
            //ShoppingCardVM.OrderHeader.PhoneNumber = ShoppingCardVM.OrderHeader.ApplicationUser.PhoneNumber;
            //ShoppingCardVM.OrderHeader.StreetAddress = ShoppingCardVM.OrderHeader.ApplicationUser.StreetAddress;
            //ShoppingCardVM.OrderHeader.City = ShoppingCardVM.OrderHeader.ApplicationUser.City;
            //ShoppingCardVM.OrderHeader.State = ShoppingCardVM.OrderHeader.ApplicationUser.State;
            //ShoppingCardVM.OrderHeader.PostalCode = ShoppingCardVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var card in ShoppingCardVM.ListCard)
            {
                card.Price = GetPriceBasedOdQuantity(card.Count, card.Product.Price, card.Product.Price50, card.Product.Price100);
                ShoppingCardVM.OrderHeader.OrderTotal += (card.Price * card.Count);
            }

            return View(ShoppingCardVM);
    
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCardVM.ListCard = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value,
               includeProperties: "Product");

            ShoppingCardVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCardVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCardVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCardVM.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var card in ShoppingCardVM.ListCard)
            {
                card.Price = GetPriceBasedOdQuantity(card.Count, card.Product.Price, card.Product.Price50, card.Product.Price100);
                ShoppingCardVM.OrderHeader.OrderTotal += (card.Price * card.Count);
            }

            _unitOfWork.OrderHeader.Add(ShoppingCardVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var card in ShoppingCardVM.ListCard)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = card.ProductId,
                    OrderId = ShoppingCardVM.OrderHeader.Id,
                    Price = card.Price,
                    Count = card.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //stripe settings 
                var domain = "https://localhost:44300/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                       "card",
                    },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/card/OrderConfirmation?id={ShoppingCardVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/card/index",
                };

                foreach (var item in ShoppingCardVM.ListCard)
                {

                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            },

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);

                }

                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCardVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCardVM.OrderHeader.Id });
            }
        }


        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book", "<p>New Order Created</p>");
            List<ShoppingCard> shoppingCarts = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId ==
            orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCard.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCard.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCard.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCard.GetFirstOrDefault(c => c.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCard.Remove(cart);
            }
            else
            {
                _unitOfWork.ShoppingCard.DecrementCount(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCard.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCard.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
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
