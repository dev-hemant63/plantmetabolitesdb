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
    public class ContributorController : Controller
    {
        private ICollaboratorsRepository _collaboratorsRepository;
        public ContributorController()
        {
            _collaboratorsRepository = new CollaboratorsRepository(new PlantMetabolitesDBContext());
        }
        public ContributorController(ICollaboratorsRepository collaboratorsRepository)
        {
            _collaboratorsRepository = collaboratorsRepository;
        }
        // GET: Contributor
        public ActionResult Index()
        {
            var model = _collaboratorsRepository.GetAllByType(2).ToList();
            return View(model);
        }
    }
}