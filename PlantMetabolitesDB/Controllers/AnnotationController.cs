using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    [Authorize(Roles = "Superadmin,Admin")]
    public class AnnotationController : Controller
    {
        private IAnnotationRepository _annotationRepository;
        GeneralClass gls = new GeneralClass();
        public AnnotationController()
        {
            _annotationRepository = new AnnotationRepository(new PlantMetabolitesDBContext());
        }
        public AnnotationController(IAnnotationRepository annotationRepository)
        {
            _annotationRepository = annotationRepository;
        }

        // GET: Annotation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAnnotation()
        {
            var model = _annotationRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddAnnotation()
        {
            AnnotationViewModel model = new AnnotationViewModel();
            //if (TempData["Failed"] != null)
            //{
            //    ViewBag.Failed = "Add Annotation Failed";
            //}
            model.Polarities = gls.GetPolarity();
            model.AnnotationTypes = gls.GetAnnotationType();

            return PartialView("_AddAnnotation", model);
        }

        [HttpPost]
        public ActionResult AddAnnotation(AnnotationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _annotationRepository.Add(model);
                if (result > 0)
                {
                    //return RedirectToAction("Index", "Annotation");
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Failed"] = "Failed";
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("_AddAnnotation", "Annotation");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditAnnotation(int AnnotationKey)
        {
            if (TempData["Failed"] != null)
            {
                ViewBag.Failed = "Edit Annotation Failed";
            }
            AnnotationViewModel model = _annotationRepository.GetByKey(AnnotationKey);
            model.Polarities = gls.GetPolarity();
            model.AnnotationTypes = gls.GetAnnotationType();
           
            return PartialView("_EditAnnotation", model);
        }

        [HttpPost]
        public ActionResult EditAnnotation(AnnotationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _annotationRepository.Update(model);
                if (result > 0)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                    // return RedirectToAction("Index", "Annotation");
                }
                else
                {
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Index", "Annotation");
                }
            }
            return View();
        }


        //public ActionResult DeleteAnnotation(int AnnotationKey)
        //{
        //    AnnotationViewModel model = _annotationRepository.GetAnnotationByKey(AnnotationKey);
        //    return View(model);
        //}

        [HttpGet]
        public ActionResult DeleteAnnotation(int AnnotationKey)
        {
            //if (TempData["Failed"] != null)
            //{
            //    ViewBag.Failed = "Delete Annotation Failed";
            //}
            var model = _annotationRepository.GetByKey(AnnotationKey);
            if (model != null)
            {
                _annotationRepository.Delete(model.AnnotationKey);
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
            // return RedirectToAction("Index", "Annotation");
        }

        [HttpGet]
        public ActionResult ActivateDeActivate(int AnnotationKey)
        {
            try
            {
                _annotationRepository.ActivateDeActivate(AnnotationKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult GetAnnotationNames()
        {
            var results = _annotationRepository.GetAll().Select(x => new { x.AnnotationName }).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}