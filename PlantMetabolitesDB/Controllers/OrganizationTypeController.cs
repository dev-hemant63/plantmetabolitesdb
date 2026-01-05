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
    [Authorize(Roles = "Superadmin")]
    public class OrganizationTypeController : Controller
    {
        private IOrganizationTypeRepository _organizationTypeRepository;

        public OrganizationTypeController()
        {
            _organizationTypeRepository = new OrganizationTypeRepository(new PlantMetabolitesDBContext());
        }
        public OrganizationTypeController(IOrganizationTypeRepository organizationTypeRepository)
        {
            _organizationTypeRepository = organizationTypeRepository;
        }

        // GET: OrganizationType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetOrganizationType()
        {
            var model = _organizationTypeRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddOrganizationType()
        {
            OrganizationTypeViewModel model = new OrganizationTypeViewModel();
            //if (TempData["Failed"] != null)
            //{
            //    ViewBag.Failed = "Add OrganizationType Failed";
            //}
            return PartialView("_AddOrganizationType", model);
        }

        [HttpPost]
        public ActionResult AddOrganizationType(OrganizationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                model.IsAddByAdmin = true;
                int result = _organizationTypeRepository.Add(model);
                if (result > 0)
                {
                    //return RedirectToAction("Index", "OrganizationType");
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Failed"] = "Failed";
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("_AddOrganizationType", "OrganizationType");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditOrganizationType(int OrganizationTypeKey)
        {
            //if (TempData["Failed"] != null)
            //{
            //    ViewBag.Failed = "Edit OrganizationType Failed";
            //}
            OrganizationTypeViewModel model = _organizationTypeRepository.GetByKey(OrganizationTypeKey);
            return PartialView("_EditOrganizationType", model);
        }

        [HttpPost]
        public ActionResult EditOrganizationType(OrganizationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                model.IsAddByAdmin = true;
                int result = _organizationTypeRepository.Update(model);
                if (result > 0)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                    // return RedirectToAction("Index", "OrganizationType");
                }
                else
                {
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Index", "OrganizationType");
                }
            }
            return View();
        }


        //public ActionResult DeleteOrganizationType(int OrganizationTypeKey)
        //{
        //    OrganizationTypeViewModel model = _organizationTypeRepository.GetOrganizationTypeByKey(OrganizationTypeKey);
        //    return View(model);
        //}

        [HttpGet]
        public ActionResult DeleteOrganizationType(int OrganizationTypeKey)
        {
            //if (TempData["Failed"] != null)
            //{
            //    ViewBag.Failed = "Delete OrganizationType Failed";
            //}
            var model = _organizationTypeRepository.GetByKey(OrganizationTypeKey);
            if (model != null)
            {
                _organizationTypeRepository.Delete(model.OrganizationTypeKey);
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
            // return RedirectToAction("Index", "OrganizationType");
        }

        [HttpGet]
        public ActionResult ActivateDeActivate(int OrganizationTypeKey)
        {
            try
            {
                _organizationTypeRepository.ActivateDeActivate(OrganizationTypeKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}