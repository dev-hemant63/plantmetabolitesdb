using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using System.Linq;
using System.Web.Mvc;
using TandemDB.Models;
using TandemDB.Repository;
using TandemDB.ViewModel;

namespace TandemDB.Controllers
{
    [Authorize(Roles = "Superadmin,Admin")]
    public class PlantMetabolitesController : Controller
    {
        private IPlantMetabolitesRepository _plantMetabolitesRepository;
        private IDatabaseRepository _databaseRepository;
        public PlantMetabolitesController()
        {
            _plantMetabolitesRepository = new PlantMetabolitesRepository(new PlantMetabolitesDBContext());
            _databaseRepository = new DatabaseRepository(new PlantMetabolitesDBContext());
        }
        public PlantMetabolitesController(IPlantMetabolitesRepository organizationTypeRepository, IDatabaseRepository databaseRepository)
        {
            _plantMetabolitesRepository = organizationTypeRepository;
            _databaseRepository = databaseRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList()
        {
            var model = _plantMetabolitesRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddEditPlantMetabolites(int plantMetabolitesKey = 0)
        {
            //var model = new AddEditPlantMetabolitesVM
            //{
            //    FormData = new PlantMetabolitesVM(),
            //};
            var model = new PlantMetabolitesVM();

            var plantfamily = _databaseRepository.GetAll();
            model.Database = plantfamily.Where(x => x.IsActive == true).Select(x => new Master_Database
            {
                DatabaseKey = (int)x.DatabaseKey,
                DatabaseName = x.DatabaseName
            }).ToList();
            if (plantMetabolitesKey == 0)
            {
                //var data = _plantMetabolitesRepository.GetById(id);
                //model.FormData = data;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddEditPlantMetabolites(PlantMetabolitesVM req)
        {
            var plantfamily = _databaseRepository.GetAll();
            req.Database = plantfamily.Where(x => x.IsActive == true).Select(x => new Master_Database
            {
                DatabaseKey = (int)x.DatabaseKey,
                DatabaseName = x.DatabaseName
            }).ToList();

            if (ModelState.IsValid)
            {
                if (req.PlantMetabolitesKey == 0)
                {
                    _plantMetabolitesRepository.Add(req);
                }
                else
                {
                    _plantMetabolitesRepository.Update(req);
                }
                return RedirectToAction("Index");
            }
            
            return View(req);
        }
    }
}