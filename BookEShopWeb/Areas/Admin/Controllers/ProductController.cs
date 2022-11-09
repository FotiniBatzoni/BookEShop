using BookEShop.DataAccess;
using BookEShop.DataAccess.Repository;
using BookEShop.DataAccess.Repository.IRepository;
using BookEShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace BookEShopWeb.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll();
            return View(objProductList);
        }



        //GET
        public IActionResult Upsert(int? id)
        {
            Product product = new();
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(
                u=> new SelectListItem
                {
                    Text= u.Name,  
                    Value=u.Id.ToString(),
                }
            );
            IEnumerable<SelectListItem> CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }
            );
            Debug.WriteLine(id);
            if (id== null || id==0)
            {
                return View(product);
            }
            else
            {

            }
        
            return View(product);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product obj)
        {
            
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Product.Save();
                TempData["success"] = "Product is updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);

        }

        //GET
        public IActionResult Delete(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }
            // var productFromDb = _db.Product.Find(id);
            var productFromDbFirst = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            //var productFromDbSingle = _db.Product.SingleOrDefault(c => c.Id == id);

            if (productFromDbFirst == null)
            {
                return NotFound();
            }
            return View(productFromDbFirst);
        }

        //POST
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();  
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Product.Save();
            TempData["success"] = "Product is deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
