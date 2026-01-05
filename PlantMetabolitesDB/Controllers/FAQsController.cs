using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    [Authorize(Roles = "Superadmin")]
    public class FAQsController : Controller
    {
        // GET: FAQs
        private IFAQRepository _faqRepository;
        public FAQsController()
        {
            _faqRepository = new FAQRepository(new PlantMetabolitesDBContext());
        }
        public FAQsController(IFAQRepository faqRepository)
        {
            _faqRepository = faqRepository;
        }
        // GET: Ticker
        [Authorize(Roles = "Superadmin")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult GetFAQ()
        {
            var model = _faqRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult AddFAQ()
        {
            FAQViewModel model = new FAQViewModel();
            return PartialView("_AddFAQs", model);
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddFAQ(FAQViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadedImageFile != null && model.UploadedImageFile.ContentLength > 0)
                {
                    string fileName = model.UploadedImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/FAQs"), uniqueFileName);
                    model.UploadedImageFile.SaveAs(filePath);
                    model.UploadedFileName = uniqueFileName;
                }

                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _faqRepository.Add(model);
                if (result > 0)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Failed"] = "Failed";
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult EditFAQ(int FAQKey)
        {
            FAQViewModel model = _faqRepository.GetByKey(FAQKey);
            return PartialView("_EditFAQs", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superadmin")]
        public ActionResult EditFAQ(FAQViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadedImageFile != null && model.UploadedImageFile.ContentLength > 0)
                {
                    var prevFileName = model.UploadedFileName;
                    string fileName = model.UploadedImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/FAQs"), uniqueFileName);
                    model.UploadedImageFile.SaveAs(filePath);
                    model.UploadedFileName = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/FAQs/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _faqRepository.Update(model);
                if (result > 0)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult DeleteFAQ(int FAQKey)
        {
            var model = _faqRepository.GetByKey(FAQKey);
            var filename = model.UploadedFileName;
            if (model != null)
            {
                _faqRepository.Delete(model.FAQKey);
                if (filename != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/FAQs/" + filename);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult ActivateDeActivate(int FAQKey)
        {
            try
            {
                _faqRepository.ActivateDeActivate(FAQKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}