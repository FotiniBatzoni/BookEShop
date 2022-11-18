using BookEShop.DataAccess;
using BookEShop.DataAccess.Repository;
using BookEShop.DataAccess.Repository.IRepository;
using BookEShop.Models;
using BookEShop.Models.ViewModels;
using BookEShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace BookEShopWeb.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            Company company = new();

            Debug.WriteLine(id);
            if (id== null || id==0)
            {
                //create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(company);
            }
            else
            {
                //update Product
                company = _unitOfWork.Company.GetFirstOrDefault(u=>u.Id==id);
                return View(company);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            
            if (ModelState.IsValid)
            { 
                if (obj.Id == 0)
               {

                    _unitOfWork.Company.Add(obj);
                    _unitOfWork.Company.Save();
                    TempData["success"] = "Company is created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    _unitOfWork.Company.Save();
                    TempData["success"] = "Company is updated successfully";
                }


                return RedirectToAction("Index");
            }
            return View(obj);

        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }

        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

     
        _unitOfWork.Company.Remove(obj);
            _unitOfWork.Company.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }

}
