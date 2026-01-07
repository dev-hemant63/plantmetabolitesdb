using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private IAnnotationRepository _annotationRepository;
        public PlantMetabolitesController()
        {
            _plantMetabolitesRepository = new PlantMetabolitesRepository(new PlantMetabolitesDBContext());
            _databaseRepository = new DatabaseRepository(new PlantMetabolitesDBContext());
            _instrumentRepository = new InstrumentRepository(new PlantMetabolitesDBContext());
            _annotationRepository = new AnnotationRepository(new PlantMetabolitesDBContext());
        }
        public PlantMetabolitesController(
            IPlantMetabolitesRepository organizationTypeRepository,
            IDatabaseRepository databaseRepository,
            IInstrumentRepository instrumentRepository,
            IAnnotationRepository annotationRepository
        )
        {
            _plantMetabolitesRepository = organizationTypeRepository;
            _databaseRepository = databaseRepository;
            _instrumentRepository = instrumentRepository;
            _annotationRepository = annotationRepository;
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
                InstrumentList = _instrumentRepository.GetAll().Where(i => i.IsActive).ToList(),
                MS1MassSpectraData = _plantMetabolitesRepository.GetMS1MassSpectraList()
            };
            return PartialView("_Frm_MS1", frm_ms_model);
        }
        [HttpPost]
        public async Task<ActionResult> GetAnnotationByPolarity(int polarity)
        {
            var result = _annotationRepository.GetAnnotationByPolarity(polarity);
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteMS1Record(int MS1MassSpectraKey)
        {
            return Json(_plantMetabolitesRepository.DeleteMS1Record(MS1MassSpectraKey));
        }
        [HttpPost]
        public async Task<ActionResult> GetMS1MassSpectraById(int MS1MassSpectraKey)
        {
            var result = _plantMetabolitesRepository.GetMS1MassSpectraById(MS1MassSpectraKey);
            return Json(new MasterMS1MassSpectraVM
            {
                MS1MassSpectraKey = result.MS1MassSpectraKey,
                PlantMetabolitesKey = result.PlantMetabolitesKey,
                InstrumentKey = result.InstrumentKey,
                Polarity = result.Polarity,
                AnnotationKey = result.AnnotationKey,
                PartsOfPlant = result.PartsOfPlant,
                SampleType = result.SampleType,
                TaxonomistName = result.TaxonomistName,
                VoucherNo = result.VoucherNo,
                HerbariumDepositedAt = result.HerbariumDepositedAt,
                DateOfCollection = result.DateOfCollection,
                GeoLocation = result.GeoLocation,
                SpectrumAveraging = result.SpectrumAveraging,
                HPLCProfile = "",
                LCMSProfile = "",
                HPLCUPLCMethodeFilePath = result.HPLCUPLCMethodeFilePath,
                ExtractionMethodeFilePath = result.ExtractionMethodeFilePath,
                FingerprintFilePath = result.FingerprintFilePath,
                MS1RefFilePath = result.MS1RefFilePath,
            });
        }
        [HttpPost]
        public ActionResult SaveMS1(MasterMS1MassSpectraVM req)
        {
            int result = 0;
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
            if (req.RawFiles != null && req.RawFiles.Count() > 0)
            {
                foreach (var rawFile in req.RawFiles)
                {
                    if (rawFile.File != null)
                    {
                        string fileName = SaveFile(rawFile.File, "MS1/RawFiles/");
                        rawFile.FilePath = fileName;
                    }
                }
            }
            if (req.LCMSProfiles != null && req.LCMSProfiles.Count() > 0)
            {
                foreach (var lcms in req.LCMSProfiles)
                {
                    if (lcms.File != null)
                    {
                        string fileName = SaveFile(lcms.File, "MS1/LCMSProfiles/");
                        lcms.FilePath = fileName;
                    }
                }
            }
            if (req.HPLCProfiles != null && req.HPLCProfiles.Count() > 0)
            {
                foreach (var hplc in req.HPLCProfiles)
                {
                    if (hplc.File != null)
                    {
                        string fileName = SaveFile(hplc.File, "MS1/HPLCProfiles/");
                        hplc.FilePath = fileName;
                    }
                }
            }

            if (req.MS1MassSpectraKey == 0) //insert
            {
                result = _plantMetabolitesRepository.AddMS1MassSpectra(new Master_MS1MassSpectra
                {
                    PlantMetabolitesKey = req.PlantMetabolitesKey,
                    InstrumentKey = req.InstrumentKey,
                    Polarity = req.Polarity,
                    AnnotationKey = req.AnnotationKey,
                    PartsOfPlant = req.PartsOfPlant,
                    SampleType = req.SampleType,
                    TaxonomistName = req.TaxonomistName,
                    VoucherNo = req.VoucherNo,
                    HerbariumDepositedAt = req.HerbariumDepositedAt,
                    DateOfCollection = req.DateOfCollection,
                    GeoLocation = req.GeoLocation,
                    SpectrumAveraging = req.SpectrumAveraging,
                    HPLCUPLCMethodeFilePath = req.HPLCUPLCMethodeFilePath,
                    ExtractionMethodeFilePath = req.ExtractionMethodeFilePath,
                    FingerprintFilePath = req.FingerprintFilePath,
                    MS1RefFilePath = req.MS1RefFilePath,
                    CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey")),
                    CreatedOn = DateTime.Now
                });
            }
            else
            {
                result = _plantMetabolitesRepository.UpdateMS1MassSpectra(new Master_MS1MassSpectra
                {
                    PlantMetabolitesKey = req.PlantMetabolitesKey,
                    InstrumentKey = req.InstrumentKey,
                    Polarity = req.Polarity,
                    AnnotationKey = req.AnnotationKey,
                    PartsOfPlant = req.PartsOfPlant,
                    SampleType = req.SampleType,
                    TaxonomistName = req.TaxonomistName,
                    VoucherNo = req.VoucherNo,
                    HerbariumDepositedAt = req.HerbariumDepositedAt,
                    DateOfCollection = req.DateOfCollection,
                    GeoLocation = req.GeoLocation,
                    SpectrumAveraging = req.SpectrumAveraging,
                    HPLCUPLCMethodeFilePath = req.HPLCUPLCMethodeFilePath,
                    ExtractionMethodeFilePath = req.ExtractionMethodeFilePath,
                    FingerprintFilePath = req.FingerprintFilePath,
                    MS1RefFilePath = req.MS1RefFilePath,
                    LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey")),
                    LastModifiedOn = DateTime.Now
                });
            }
            return Json(result);
        }

        private string SaveFile(HttpPostedFileBase file, string folder)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyhhmmssfff") + Path.GetExtension(file.FileName);
            var uploads = Server.MapPath("~/UploadedFiles/" + folder);
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);
            file.SaveAs(filePath);

            return fileName;
        }
    }
}