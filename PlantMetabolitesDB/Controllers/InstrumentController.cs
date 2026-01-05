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
    public class InstrumentController : Controller
    {
        private IInstrumentRepository _instrumentRepository;
        private IDatabaseRepository _databaseRepository;
        public InstrumentController()
        {
            _instrumentRepository = new InstrumentRepository(new PlantMetabolitesDBContext());
            _databaseRepository = new DatabaseRepository(new PlantMetabolitesDBContext());
        }
        public InstrumentController(IInstrumentRepository instrumentRepository, IDatabaseRepository databaseRepository)
        {
            _instrumentRepository = instrumentRepository;
            _databaseRepository = databaseRepository;
        }

        // GET: Instrument
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetInstrument()
        {
            var model = _instrumentRepository.GetAll();

            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddInstrument()
        {
            InstrumentViewModel model = new InstrumentViewModel();
            model.Databases = _databaseRepository.GetAll().Where(d => d.IsActive == true).ToList();

            return PartialView("_AddInstrument", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInstrument(InstrumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _instrumentRepository.Add(model);
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
        public ActionResult EditInstrument(int InstrumentKey)
        {
            if (TempData["Failed"] != null)
            {
                ViewBag.Failed = "Edit Instrument Failed";
            }
            InstrumentViewModel model = _instrumentRepository.GetByKey(InstrumentKey);
            model.Databases = _databaseRepository.GetAll().Where(d => d.IsActive == true).ToList();

            return PartialView("_EditInstrument", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInstrument(InstrumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _instrumentRepository.Update(model);
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
        public ActionResult DeleteInstrument(int InstrumentKey)
        {
            var model = _instrumentRepository.GetByKey(InstrumentKey);
            if (model != null)
            {
                _instrumentRepository.Delete(model.InstrumentKey);
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ActivateDeActivate(int InstrumentKey)
        {
            try
            {
                _instrumentRepository.ActivateDeActivate(InstrumentKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult MapDatabase(int InstrumentKey)
        {
            var lstDatabase = _databaseRepository.GetAll().Where(d => d.IsActive == true).ToList();
            var lstDatabseInstrument = _instrumentRepository.GetDatabaseByInstrumentKey(InstrumentKey);
            List<DbInstViewModel> model = new List<DbInstViewModel>();

            foreach (var dbItem in lstDatabase)
            {
                foreach (var dbInsItem in lstDatabseInstrument)
                {
                    if (dbItem.DatabaseKey == dbInsItem.DatabaseKey)
                    {
                        dbItem.IsCheked = true;
                    }
                }
            }
            foreach (var dbItem in lstDatabase)
            {
                model.Add(new DbInstViewModel
                {
                    DatabaseKey = dbItem.DatabaseKey,
                    InstrumentKey = Convert.ToInt16(InstrumentKey),
                    DatabaseName = dbItem.DatabaseName,
                    IsCheked = dbItem.IsCheked,
                });
            }
            return PartialView("_MapDatabase", model);
        }

        [HttpPost]

        public ActionResult MapDatabase(List<DbInstViewModel> lstDbInstViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!lstDbInstViewModel.Any(d => d.IsCheked == true) || lstDbInstViewModel == null)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                int result = _instrumentRepository.AddMapDatabaseInstrument(lstDbInstViewModel);
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
        public JsonResult GetInstrumentNames()
        {
            var results = _instrumentRepository.GetAll().Select(x => new { x.InstrumentName }).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}