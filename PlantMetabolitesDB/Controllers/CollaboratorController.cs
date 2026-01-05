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
    public class CollaboratorController : Controller
    {
        private ICollaboratorsRepository _collaboratorsRepository;
        public CollaboratorController()
        {
            _collaboratorsRepository = new CollaboratorsRepository(new PlantMetabolitesDBContext());
        }
        public CollaboratorController(ICollaboratorsRepository collaboratorsRepository)
        {
            _collaboratorsRepository = collaboratorsRepository;
        }
        // GET: Colloborator
        public ActionResult Index()
        {
            var model = _collaboratorsRepository.GetAllByType(1);
            return View(model);
        }

    }
}