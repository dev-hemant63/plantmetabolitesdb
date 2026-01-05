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
    public class LatestUpdateController : Controller
    {
        private ILatestUpdatesRepository _latestUpdatesRepository;
        GeneralClass gls = new GeneralClass();
        public LatestUpdateController()
        {
            _latestUpdatesRepository = new LatestUpdatesRepository(new PlantMetabolitesDBContext());
        }
        public LatestUpdateController(ILatestUpdatesRepository latestUpdatesRepository)
        {
            _latestUpdatesRepository = latestUpdatesRepository;
        }
        // GET: LatestUpdate
        [Authorize(Roles = "Superadmin")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult GetLatestUpdate()
        {
            var model = _latestUpdatesRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult AddLatestUpdate()
        {
            LatestUpdatesViewModel model = new LatestUpdatesViewModel();
            model.MessageTypes = gls.GetMessageType();
            return PartialView("_AddLatestUpdate", model);
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddLatestUpdate(LatestUpdatesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadedImageFile != null && model.UploadedImageFile.ContentLength > 0)
                {
                    string fileName = model.UploadedImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/LatestUpdates"), uniqueFileName);
                    model.UploadedImageFile.SaveAs(filePath);
                    model.UploadedFileName = uniqueFileName;
                }

                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _latestUpdatesRepository.Add(model);
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
        public ActionResult EditLatestUpdate(int LatestUpdatesKey)
        {
            LatestUpdatesViewModel model = _latestUpdatesRepository.GetByKey(LatestUpdatesKey);
            model.MessageTypes= gls.GetMessageType();   
            return PartialView("_EditLatestUpdate", model);
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditLatestUpdate(LatestUpdatesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadedImageFile != null && model.UploadedImageFile.ContentLength > 0)
                {
                    var prevFileName = model.UploadedFileName;
                    string fileName = model.UploadedImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/LatestUpdates"), uniqueFileName);
                    model.UploadedImageFile.SaveAs(filePath);
                    model.UploadedFileName = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/LatestUpdates/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _latestUpdatesRepository.Update(model);
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
        public ActionResult DeleteLatestUpdate(int LatestUpdatesKey)
        {
            var model = _latestUpdatesRepository.GetByKey(LatestUpdatesKey);
            var filename = model.UploadedFileName;
            if (model != null)
            {
                _latestUpdatesRepository.Delete(model.LatestUpdatesKey);
                if (filename != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/LatestUpdates/" + filename);
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
        public ActionResult ActivateDeActivate(int LatestUpdatesKey)
        {
            try
            {
                _latestUpdatesRepository.ActivateDeActivate(LatestUpdatesKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}