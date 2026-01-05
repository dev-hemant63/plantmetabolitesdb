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
    public class CollaboratorsController : Controller
    {
        private ICollaboratorsRepository _collaboratorsRepository;
        private ICommonRepository _commonRepository;
        GeneralClass gls = new GeneralClass();

        public CollaboratorsController()
        {
            _collaboratorsRepository = new CollaboratorsRepository(new PlantMetabolitesDBContext());
            _commonRepository = new CommonRepository(new PlantMetabolitesDBContext());
        }
        public CollaboratorsController(ICollaboratorsRepository collaboratorsRepository, ICommonRepository commonRepository)
        {
            _collaboratorsRepository = collaboratorsRepository;
            _commonRepository = commonRepository;
        }

        // GET: Collaborators
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetCollaborators()
        {
            var model = _collaboratorsRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCollaborators()
        {
            CollaboratorsViewModel model = new CollaboratorsViewModel();
            model.CollaboratorsTypes = gls.GetCollaboratorsType();
            model.Countries = _commonRepository.GetAllCountry().ToList();

            return PartialView("_AddCollaborators", model);
        }

        [HttpPost]
        public ActionResult AddCollaborators(CollaboratorsViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _collaboratorsRepository.Add(model);
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
        public ActionResult EditCollaborators(int collaboratorsKey)
        {
            CollaboratorsViewModel model = _collaboratorsRepository.GetByKey(collaboratorsKey);
            model.CollaboratorsTypes = gls.GetCollaboratorsType();
            model.Countries = _commonRepository.GetAllCountry().ToList();

            return PartialView("_EditCollaborators", model);
        }

        [HttpPost]
        public ActionResult EditCollaborators(CollaboratorsViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey")); Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _collaboratorsRepository.Update(model);
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
        public ActionResult DeleteCollaborators(int collaboratorsKey)
        {
            var model = _collaboratorsRepository.GetByKey(collaboratorsKey);
            if (model != null)
            {
                _collaboratorsRepository.Delete(model.CollaboratorsKey);
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ActivateDeActivate(int collaboratorsKey)
        {
            try
            {
                _collaboratorsRepository.ActivateDeActivate(collaboratorsKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}