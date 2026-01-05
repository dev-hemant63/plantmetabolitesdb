using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;

namespace PlantMetabolitesDB.Controllers
{
    public class AvailableDatabaseController : Controller
    {
        private IHomeRepository _homeRepository;
        public AvailableDatabaseController()
        {
            _homeRepository = new HomeRepository(new PlantMetabolitesDBContext());
        }

        public AvailableDatabaseController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }
        // GET: AvailableDatabase
        public ActionResult Index()
        {
            var model = _homeRepository.GetAvailableDatabase();
            return View(model);
        }
    }
}