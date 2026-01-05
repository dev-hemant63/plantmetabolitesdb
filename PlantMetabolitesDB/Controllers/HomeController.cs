using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;

namespace PlantMetabolitesDB.Controllers
{
    public class HomeController : Controller
    {
        private IHomeRepository _homeRepository;
        public HomeController()
        {
            _homeRepository = new HomeRepository(new PlantMetabolitesDBContext());
        }

        public HomeController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }
        public ActionResult Index()
        {           
            return View();
        }
        public PartialViewResult GetAvailableDatabase()
        {
            var model = _homeRepository.GetAvailableDatabase();
            return PartialView("_DatabasePartialPage", model);
        }

        public PartialViewResult GetNewsTicker()
        {
            var model = _homeRepository.GetNewsTicker();
            return PartialView("_NewsTickerPartialPage", model);
        }

        public PartialViewResult GetDownloads()
        {
            var model = _homeRepository.GetDownloads();
            return PartialView("_DownloadsPartialPage", model);
        }


    }
}