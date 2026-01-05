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
    public class TickerController : Controller
    {
        private ITickerRepository _tickerRepository;
        GeneralClass gls = new GeneralClass();
        public TickerController()
        {
            _tickerRepository = new TickerRepository(new PlantMetabolitesDBContext());
        }
        public TickerController(ITickerRepository tickerRepository)
        {
            _tickerRepository = tickerRepository;
        }
        // GET: Ticker
        [Authorize(Roles = "Superadmin")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult GetTicker()
        {
            var model = _tickerRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public ActionResult AddTicker()
        {
            TickerViewModel model = new TickerViewModel();
            model.MessageTypes = gls.GetMessageType();

            return PartialView("_AddTicker", model);
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddTicker(TickerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadedImageFile != null && model.UploadedImageFile.ContentLength > 0)
                {
                    string fileName = model.UploadedImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Ticker"), uniqueFileName);
                    model.UploadedImageFile.SaveAs(filePath);
                    model.UploadedFileName = uniqueFileName;
                }

                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _tickerRepository.Add(model);
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
        public ActionResult EditTicker(int TickerKey)
        {
            TickerViewModel model = _tickerRepository.GetByKey(TickerKey);
            model.MessageTypes = gls.GetMessageType();
            return PartialView("_EditTicker", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superadmin")]
        public ActionResult EditTicker(TickerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadedImageFile != null && model.UploadedImageFile.ContentLength > 0)
                {
                    var prevFileName = model.UploadedFileName;
                    string fileName = model.UploadedImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Ticker"), uniqueFileName);
                    model.UploadedImageFile.SaveAs(filePath);
                    model.UploadedFileName = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/Ticker/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _tickerRepository.Update(model);
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
        public ActionResult DeleteTicker(int TickerKey)
        {
            var model = _tickerRepository.GetByKey(TickerKey);
            var filename = model.UploadedFileName;
            if (model != null)
            {
                _tickerRepository.Delete(model.TickerKey);
                if (filename != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/Ticker/" + filename);
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
        public ActionResult ActivateDeActivate(int TickerKey)
        {
            try
            {
                _tickerRepository.ActivateDeActivate(TickerKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}