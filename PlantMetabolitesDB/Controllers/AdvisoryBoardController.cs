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
    public class AdvisoryBoardController : Controller
    {
        private IAdvisoryBoardRepository _advisoryBoardRepository;
        private ICommonRepository _commonRepository;
        GeneralClass gls = new GeneralClass();
        public AdvisoryBoardController()
        {
            _advisoryBoardRepository = new AdvisoryBoardRepository(new PlantMetabolitesDBContext());
            _commonRepository = new CommonRepository(new PlantMetabolitesDBContext());
        }
        public AdvisoryBoardController(IAdvisoryBoardRepository advisoryBoardRepository, ICommonRepository commonRepository)
        {
            _advisoryBoardRepository = advisoryBoardRepository;
            _commonRepository = commonRepository;
        }

        // GET: AdvisoryBoard
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAdvisoryBoard()
        {
            var model = _advisoryBoardRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddAdvisoryBoard()
        {
            AdvisoryBoardViewModel model = new AdvisoryBoardViewModel();
            model.DisplayOrders = gls.GetDisplayOrder();
            model.Countries = _commonRepository.GetAllCountry().ToList();

            return PartialView("_AddAdvisoryBoard", model);
        }

        [HttpPost]
        public ActionResult AddAdvisoryBoard(AdvisoryBoardViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _advisoryBoardRepository.Add(model);
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
        public ActionResult EditAdvisoryBoard(int advisoryBoardKey)
        {
            AdvisoryBoardViewModel model = _advisoryBoardRepository.GetByKey(advisoryBoardKey);
            model.DisplayOrders = gls.GetDisplayOrder();
            model.Countries = _commonRepository.GetAllCountry().ToList();

            return PartialView("_EditAdvisoryBoard", model);
        }

        [HttpPost]
        public ActionResult EditAdvisoryBoard(AdvisoryBoardViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _advisoryBoardRepository.Update(model);
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
        public ActionResult DeleteAdvisoryBoard(int advisoryBoardKey)
        {
            var model = _advisoryBoardRepository.GetByKey(advisoryBoardKey);
            if (model != null)
            {
                _advisoryBoardRepository.Delete(model.AdvisioryBoradKey);
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ActivateDeActivate(int advisoryBoardKey)
        {
            try
            {
                _advisoryBoardRepository.ActivateDeActivate(advisoryBoardKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}