using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.Models;

namespace PlantMetabolitesDB.Controllers
{
    public class DownloadController : Controller
    {
        private ILatestUpdatesRepository _latestUpdatesRepository;
        public DownloadController()
        {
            _latestUpdatesRepository = new LatestUpdatesRepository(new PlantMetabolitesDBContext());
        }
        public DownloadController(ILatestUpdatesRepository latestUpdatesRepository)
        {
            _latestUpdatesRepository = latestUpdatesRepository;
        }
        // GET: Download
        public ActionResult Index()
        {
            var model = _latestUpdatesRepository.GetAll().Where(d => d.IsActive == true).OrderByDescending(d => d.LatestUpdatesKey).ToList(); ;
            return View(model);
        }
    }
}