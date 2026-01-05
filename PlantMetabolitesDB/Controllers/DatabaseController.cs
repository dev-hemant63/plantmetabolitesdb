using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    [Authorize(Roles = "Superadmin,Admin")]
    public class DatabaseController : Controller
    {
        private IDatabaseRepository _databaseRepository;
        public DatabaseController()
        {
            _databaseRepository = new DatabaseRepository(new PlantMetabolitesDBContext());
        }
        public DatabaseController(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        // GET: Database
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDatabase()
        {
            var model = _databaseRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddDatabase()
        {
            DatabaseViewModel model = new DatabaseViewModel();
            return PartialView("_AddDatabase", model);
        }

        [HttpPost]
        public ActionResult AddDatabase(DatabaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _databaseRepository.Add(model);
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
        public ActionResult EditDatabase(int DatabaseKey)
        {
            DatabaseViewModel model = _databaseRepository.GetByKey(DatabaseKey);
            return PartialView("_EditDatabase", model);
        }

        [HttpPost]
        public ActionResult EditDatabase(DatabaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _databaseRepository.Update(model);
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
        public ActionResult DeleteDatabase(int DatabaseKey)
        {
            var model = _databaseRepository.GetByKey(DatabaseKey);
            if (model != null)
            {
                _databaseRepository.Delete(model.DatabaseKey);
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ActivateDeActivate(int DatabaseKey)
        {
            try
            {
                _databaseRepository.ActivateDeActivate(DatabaseKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetDatabaseNames()
        {
            var results = _databaseRepository.GetAll().Select(x => new { x.DatabaseName }).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}