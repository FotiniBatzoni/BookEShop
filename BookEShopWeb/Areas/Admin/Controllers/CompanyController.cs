using BookEShop.DataAccess;
using BookEShop.DataAccess.Repository;
using BookEShop.DataAccess.Repository.IRepository;
using BookEShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookEShopWeb.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Company> objCategoryList = _unitOfWork.Company.GetAll();
            return View(objCategoryList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Company obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Add(obj);
                _unitOfWork.Company.Save();
                TempData["success"] = "Company is created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);

        }


        //GET
        public IActionResult Edit(int? id)
        {

            Debug.WriteLine(id);
            if (id== null || id==0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var companyFromDbFirst = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(c => c.Id == id);

            if(companyFromDbFirst == null)
            {
                return NotFound();
            }
            return View(companyFromDbFirst);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Company obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Update(obj);
                _unitOfWork.Company.Save();
                TempData["success"] = "Company is updated successfully";
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
            // var companyFromDbFirst = _db.Company.Find(id);
            var companyFromDbFirst = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
            //var companyFromDbFirst = _db.Company.SingleOrDefault(c => c.Id == id);

            if (companyFromDbFirst == null)
            {
                return NotFound();
            }
            return View(companyFromDbFirst);
        }

        //POST
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();  
            }

            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Company.Save();
            TempData["success"] = "Company is deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
