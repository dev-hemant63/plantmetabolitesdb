using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
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
        private IInstrumentRepository _instrumentRepository;
        public PlantMetabolitesController()
        {
            _plantMetabolitesRepository = new PlantMetabolitesRepository(new PlantMetabolitesDBContext());
            _databaseRepository = new DatabaseRepository(new PlantMetabolitesDBContext());
            _instrumentRepository = new InstrumentRepository(new PlantMetabolitesDBContext());
        }
        public PlantMetabolitesController(
            IPlantMetabolitesRepository organizationTypeRepository,
            IDatabaseRepository databaseRepository,
            IInstrumentRepository instrumentRepository
        )
        {
            _plantMetabolitesRepository = organizationTypeRepository;
            _databaseRepository = databaseRepository;
            _instrumentRepository = instrumentRepository;
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetList()
        {
            var model = _plantMetabolitesRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddEditPlantMetabolites(int plantMetabolitesKey = 0)
        {
            var model = new PlantMetabolitesVM
            {
                Database = new System.Collections.Generic.List<Master_Database>()
            };
            
            if (plantMetabolitesKey > 0)
            {
                model = _plantMetabolitesRepository.GetById(plantMetabolitesKey);
            }
            var plantfamily = _databaseRepository.GetAll();
            model.Database = plantfamily.Where(x => x.IsActive == true).Select(x => new Master_Database
            {
                DatabaseKey = x.DatabaseKey,
                DatabaseName = x.DatabaseName
            }).ToList();

            return View(model);
        }
        [HttpPost]
        public ActionResult AddEditPlantMetabolites(PlantMetabolitesVM req)
        {
            var plantfamily = _databaseRepository.GetAll();
            req.Database = plantfamily.Where(x => x.IsActive == true).Select(x => new Master_Database
            {
                DatabaseKey = x.DatabaseKey,
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

        [HttpPost]
        public ActionResult Delete(int plantMetabolitesKey)
        {
            return Json(_plantMetabolitesRepository.Delete(plantMetabolitesKey));
        }

        [HttpPost]
        public ActionResult Frm_MS1(int plantMetabolitesKey)
        {
            var frm_ms_model = new Frm_MS_VM
            {
                plantMetabolitesKey = plantMetabolitesKey,
                PlantMetabolitesDetails = _plantMetabolitesRepository.GetById(plantMetabolitesKey),
                MasterMS1MassSpectra = new MasterMS1MassSpectraVM(),
                InstrumentList = _instrumentRepository.GetAll().Where(i => i.IsActive).ToList()
            };
            return PartialView("_Frm_MS1", frm_ms_model);
        }
        [HttpPost]
        public ActionResult SaveMS1(MasterMS1MassSpectraVM req)
        {
            if (req.HPLCUPLCMethodeFile != null)
            {
                string fileName = SaveFile(req.HPLCUPLCMethodeFile, "MS1/HPLCUPLC/");
                req.HPLCUPLCMethodeFilePath = fileName;
            }
            if (req.ExtractionMethodeFile != null)
            {
                string fileName = SaveFile(req.ExtractionMethodeFile, "MS1/ExtractionMethode/");
                req.ExtractionMethodeFilePath = fileName;
            }
            if (req.FingerprintFile != null)
            {
                string fileName = SaveFile(req.FingerprintFile, "MS1/FingerprintFile/");
                req.FingerprintFilePath = fileName;
            }
            if (req.MS1RefFile != null)
            {
                string fileName = SaveFile(req.MS1RefFile, "MS1/MS1RefFile/");
                req.MS1RefFilePath = fileName;
            }

            //write other files and logic for RawFiles, LCMSProfiles, HPLCProfiles if needed

            return Json(0);
        }

        private string SaveFile(HttpPostedFileBase file, string folder)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyhhmmssfff")+Path.GetExtension(file.FileName);
            var uploads = Server.MapPath("~/UploadedFiles/" + folder);
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);
            file.SaveAs(filePath);

            return fileName;
        }
    }
}