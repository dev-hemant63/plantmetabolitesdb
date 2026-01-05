using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    [Authorize(Roles = "Superadmin,Admin,User")]
    public class CompoundController : Controller
    {
        private ICommonRepository _commonRepository;
        private ICompoundRepository _compoundRepository;
        private IDatabaseRepository _databaseRepository;
        private IInstrumentRepository _instrumentRepository;
        private IAnnotationRepository _annotationRepository;
        private IMS2MassSpectraRepository _ms2MassSpectraRepository;
        private IMS3MassSpectraRepository _ms3MassSpectraRepository;
        private IAductMassSpectraRepository _aductMassSpectraRepository;
        private IMassSpectraDataValuesRepository _massSpectraDataValuesRepository;

        GeneralClass gls = new GeneralClass();
        public CompoundController()
        {
            _commonRepository = new CommonRepository(new PlantMetabolitesDBContext());
            _compoundRepository = new CompoundRepository(new PlantMetabolitesDBContext());
            _databaseRepository = new DatabaseRepository(new PlantMetabolitesDBContext());
            _instrumentRepository = new InstrumentRepository(new PlantMetabolitesDBContext());
            _annotationRepository = new AnnotationRepository(new PlantMetabolitesDBContext());
            _ms2MassSpectraRepository = new MS2MassSpectraRepository(new PlantMetabolitesDBContext());
            _ms3MassSpectraRepository = new MS3MassSpectraRepository(new PlantMetabolitesDBContext());
            _aductMassSpectraRepository = new AductMassSpectraRepository(new PlantMetabolitesDBContext());
            _massSpectraDataValuesRepository = new MassSpectraDataValuesRepository(new PlantMetabolitesDBContext());
        }
        public CompoundController(ICommonRepository commonRepository,
            ICompoundRepository compoundRepository,
            IDatabaseRepository databaseRepository,
            IInstrumentRepository instrumentRepository,
            IAnnotationRepository annotationRepository,
            IMS2MassSpectraRepository ms2MassSpectraRepository,
            IMS3MassSpectraRepository ms3MassSpectraRepository,
            IAductMassSpectraRepository aductMassSpectraRepository,
            IMassSpectraDataValuesRepository massSpectraDataValuesRepository)
        {
            _commonRepository = commonRepository;
            _compoundRepository = compoundRepository;
            _databaseRepository = databaseRepository;
            _instrumentRepository = instrumentRepository;
            _annotationRepository = annotationRepository;
            _ms2MassSpectraRepository = ms2MassSpectraRepository;
            _ms3MassSpectraRepository = ms3MassSpectraRepository;
            _aductMassSpectraRepository = aductMassSpectraRepository;
            _massSpectraDataValuesRepository = massSpectraDataValuesRepository;
        }
        // GET: Compound
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetCompounds()
        {
            var model = _compoundRepository.GetAll();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCompound()
        {
            CompoundViewModel model = new CompoundViewModel();
            model.Databases = _databaseRepository.GetAll().Where(d => d.IsActive == true).ToList();

            return PartialView("_AddCompound", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompound(CompoundViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ChemicalStructureImageFile != null && model.ChemicalStructureImageFile.ContentLength > 0)
                {
                    string fileName = model.ChemicalStructureImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";

                    uniqueFileName = model.CompoundName + "_" + uniqueFileName;
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Compound/ChemicalStructure/"), uniqueFileName);
                    model.ChemicalStructureImageFile.SaveAs(filePath);
                    model.ChemicalStructureFile = uniqueFileName;
                }
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _compoundRepository.Add(model);
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
        public ActionResult EditCompound(int CompoundKey)
        {
            CompoundViewModel model = _compoundRepository.GetByKey(CompoundKey);
            model.Databases = _databaseRepository.GetAll().Where(d => d.IsActive == true).ToList();
            return PartialView("_EditCompound", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompound(CompoundViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ChemicalStructureImageFile != null && model.ChemicalStructureImageFile.ContentLength > 0)
                {
                    var prevFileName = model.ChemicalStructureImageFile;
                    string fileName = model.ChemicalStructureImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";

                    uniqueFileName = model.CompoundName + "_" + uniqueFileName;
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Compound/ChemicalStructure/"), uniqueFileName);
                    model.ChemicalStructureImageFile.SaveAs(filePath);
                    model.ChemicalStructureFile = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/Compound/ChemicalStructure/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _compoundRepository.Update(model);
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
        public ActionResult DeleteCompound(int compoundKey)
        {
            var model = _compoundRepository.GetByKey(compoundKey);
            var chemicalstructurefile = model.ChemicalStructureFile;
            if (model != null)
            {
                _compoundRepository.Delete(model.CompoundKey);
                if (chemicalstructurefile != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/Compound/ChemicalStructure/" + chemicalstructurefile);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                //Delete Ms2 File
                var ms2 = _ms2MassSpectraRepository.GetByCompoundKey(model.CompoundKey);
                foreach (var item in ms2)
                {
                    var ms2spectra = _ms2MassSpectraRepository.GetByKey(item.MS2MassSpectraKey);
                    var datafile = ms2spectra.MS2DataFile;
                    var reffile = ms2spectra.MS2RefFile;

                    _ms2MassSpectraRepository.Delete(item.MS2MassSpectraKey);
                    if (datafile != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/MS2/DataFile/" + datafile);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    if (reffile != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/MS2/RefFile/" + reffile);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                //Delete MS3 File
                var ms3 = _ms3MassSpectraRepository.GetByCompoundKey(model.CompoundKey);
                foreach (var item in ms3)
                {
                    var ms3spectra = _ms3MassSpectraRepository.GetByKey(item.MS3MassSpectraKey);
                    var datafile = ms3spectra.MS3DataFile;
                    var reffile = ms3spectra.MS3RefFile;

                    _ms3MassSpectraRepository.Delete(item.MS3MassSpectraKey);
                    if (datafile != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/MS3/DataFile/" + datafile);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    if (reffile != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/MS3/RefFile/" + reffile);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                //Delete Aduct File
                var aduct = _aductMassSpectraRepository.GetByCompoundKey(model.CompoundKey);
                foreach (var item in aduct)
                {
                    var aductspectra = _aductMassSpectraRepository.GetByKey(item.AductMassSpectraKey);
                    var datafile = aductspectra.AductDataFile;
                    var reffile = aductspectra.AductRefFile;

                    _aductMassSpectraRepository.Delete(item.AductMassSpectraKey);
                    if (datafile != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/Aduct/DataFile/" + datafile);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    if (reffile != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/Aduct/RefFile/" + reffile);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCompoundMS2(int CompoundKey, int DatabaseKey, string MSTypeKey)
        {
            ViewBag.DatabaseKey = DatabaseKey;
            if (MSTypeKey == "MS2")
            {
                MS2MassSpectraViewModel model = new MS2MassSpectraViewModel();
                model.CompoundKey = Convert.ToInt16(CompoundKey);
                model.Instruments = _instrumentRepository.GetInstrumentByDatabaseKey(DatabaseKey).ToList();
                model.Annotations = new List<AnnotationViewModel>();
                model.Polarities = gls.GetPolarity();
                return PartialView("_AddMS2MassSpectra", model);
            }
            else if (MSTypeKey == "MS3")
            {
                MS3MassSpectraViewModel model = new MS3MassSpectraViewModel();
                model.CompoundKey = Convert.ToInt16(CompoundKey);
                model.Instruments = _instrumentRepository.GetInstrumentByDatabaseKey(DatabaseKey).ToList();
                model.Annotations = new List<AnnotationViewModel>();
                model.Polarities = gls.GetPolarity();
                return PartialView("_AddMS3MassSpectra", model);
            }
            else
            {
                AductMassSpectraViewModel model = new AductMassSpectraViewModel();
                model.CompoundKey = Convert.ToInt16(CompoundKey);
                model.Instruments = _instrumentRepository.GetInstrumentByDatabaseKey(DatabaseKey).ToList();
                model.Annotations = new List<AnnotationViewModel>();
                model.Polarities = gls.GetPolarity();
                return PartialView("_AddAductMassSpectra", model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompoundMS2(MS2MassSpectraViewModel model)
        {
            List<MassSpectra_DataValues> lstMassSpectra_DataValues = new List<MassSpectra_DataValues>();
            if (ModelState.IsValid)
            {
                if (model.MS2DataImageFile != null && model.MS2DataImageFile.ContentLength > 0)
                {
                    string fileName = model.MS2DataImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS2/DataFile"), uniqueFileName);
                    model.MS2DataImageFile.SaveAs(filePath);
                    model.MS2DataFile = uniqueFileName;
                    model.lstMassSpectra_DataValues = GeneralClass.GetMassSpectra_DataValues(filePath, 1);
                }
                if (model.MS2RefImageFile != null && model.MS2RefImageFile.ContentLength > 0)
                {
                    string fileName = model.MS2RefImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS2/RefFile"), uniqueFileName);
                    model.MS2RefImageFile.SaveAs(filePath);
                    model.MS2RefFile = uniqueFileName;
                }
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _ms2MassSpectraRepository.Add(model);
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
            else
            {
                TempData["Failed"] = "Failed";
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompoundMS3(MS3MassSpectraViewModel model)
        {
            List<MassSpectra_DataValues> lstMassSpectra_DataValues = new List<MassSpectra_DataValues>();
            if (ModelState.IsValid)
            {
                if (model.MS3DataImageFile != null && model.MS3DataImageFile.ContentLength > 0)
                {
                    string fileName = model.MS3DataImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS3/DataFile"), uniqueFileName);
                    model.MS3DataImageFile.SaveAs(filePath);
                    model.MS3DataFile = uniqueFileName;
                    model.lstMassSpectra_DataValues = GeneralClass.GetMassSpectra_DataValues(filePath, 3);
                }
                if (model.MS3RefImageFile != null && model.MS3RefImageFile.ContentLength > 0)
                {
                    string fileName = model.MS3RefImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS3/RefFile"), uniqueFileName);
                    model.MS3RefImageFile.SaveAs(filePath);
                    model.MS3RefFile = uniqueFileName;
                }
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _ms3MassSpectraRepository.Add(model);
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
            else
            {
                TempData["Failed"] = "Failed";
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompoundAduct(AductMassSpectraViewModel model)
        {
            List<MassSpectra_DataValues> lstMassSpectra_DataValues = new List<MassSpectra_DataValues>();
            if (ModelState.IsValid)
            {
                if (model.AductDataImageFile != null && model.AductDataImageFile.ContentLength > 0)
                {
                    string fileName = model.AductDataImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Aduct/DataFile"), uniqueFileName);
                    model.AductDataImageFile.SaveAs(filePath);
                    model.AductDataFile = uniqueFileName;
                    model.lstMassSpectra_DataValues = GeneralClass.GetMassSpectra_DataValues(filePath, 2);
                }
                if (model.AductRefImageFile != null && model.AductRefImageFile.ContentLength > 0)
                {
                    string fileName = model.AductRefImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Aduct/RefFile"), uniqueFileName);
                    model.AductRefImageFile.SaveAs(filePath);
                    model.AductRefFile = uniqueFileName;
                }
                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _aductMassSpectraRepository.Add(model);
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
            else
            {
                TempData["Failed"] = "Failed";
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult EditCompoundMS2(int MS2MassSpectraKey, int databasekey)
        {
            var model = _ms2MassSpectraRepository.GetByKey(MS2MassSpectraKey);
            model.Instruments = _instrumentRepository.GetInstrumentByDatabaseKey(databasekey).ToList();
            model.Annotations = _annotationRepository.GetAll().Where(a => a.IsActive == true && a.Polarity == model.Polarity).ToList();
            model.Polarities = gls.GetPolarity();

            return PartialView("_AddEditMS2MassSpectra", model);
        }

        [HttpGet]
        public ActionResult EditCompoundMS3(int MS3MassSpectraKey, int databasekey)
        {
            var model = _ms3MassSpectraRepository.GetByKey(MS3MassSpectraKey);
            model.Instruments = _instrumentRepository.GetInstrumentByDatabaseKey(databasekey).ToList();
            model.Annotations = _annotationRepository.GetAll().Where(a => a.IsActive == true && a.Polarity == model.Polarity).ToList();
            model.Polarities = gls.GetPolarity();

            return PartialView("_AddEditMS3MassSpectra", model);
        }

        [HttpGet]
        public ActionResult EditCompoundAduct(int AductMassSpectraKey, int databasekey)
        {
            var model = _aductMassSpectraRepository.GetByKey(AductMassSpectraKey);
            model.Instruments = _instrumentRepository.GetInstrumentByDatabaseKey(databasekey).ToList();
            model.Annotations = _annotationRepository.GetAll().Where(a => a.IsActive == true && a.Polarity == model.Polarity).ToList();
            model.Polarities = gls.GetPolarity();

            return PartialView("_AddEditAductMassSpectra", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompoundMS2(MS2MassSpectraViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.MS2DataImageFile != null && model.MS2DataImageFile.ContentLength > 0)
                {
                    var prevFileName = model.MS2DataFile;
                    string fileName = model.MS2DataImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS2/DataFile"), uniqueFileName);
                    model.MS2DataImageFile.SaveAs(filePath);
                    model.MS2DataFile = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/MS2/DataFile/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                if (model.MS2RefImageFile != null && model.MS2RefImageFile.ContentLength > 0)
                {
                    var prevFileName = model.MS2RefFile;
                    string fileName = model.MS2RefImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS2/RefFile"), uniqueFileName);
                    model.MS2RefImageFile.SaveAs(filePath);
                    model.MS2RefFile = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/MS2/RefFile/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _ms2MassSpectraRepository.Update(model);
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
            else
            {
                TempData["Failed"] = "Failed";
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompoundMS3(MS3MassSpectraViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.MS3DataImageFile != null && model.MS3DataImageFile.ContentLength > 0)
                {
                    var prevFileName = model.MS3DataFile;
                    string fileName = model.MS3DataImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS3/DataFile"), uniqueFileName);
                    model.MS3DataImageFile.SaveAs(filePath);
                    model.MS3DataFile = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/MS3/DataFile/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                if (model.MS3RefImageFile != null && model.MS3RefImageFile.ContentLength > 0)
                {
                    var prevFileName = model.MS3RefFile;
                    string fileName = model.MS3RefImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS3/RefFile"), uniqueFileName);
                    model.MS3RefImageFile.SaveAs(filePath);
                    model.MS3RefFile = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/MS3/RefFile/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _ms3MassSpectraRepository.Update(model);
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
            else
            {
                TempData["Failed"] = "Failed";
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompoundAduct(AductMassSpectraViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.AductDataImageFile != null && model.AductDataImageFile.ContentLength > 0)
                {
                    var prevFileName = model.AductDataFile;
                    string fileName = model.AductDataImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Aduct/DataFile"), uniqueFileName);
                    model.AductDataImageFile.SaveAs(filePath);
                    model.AductDataFile = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/Aduct/DataFile/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                if (model.AductRefImageFile != null && model.AductRefImageFile.ContentLength > 0)
                {
                    var prevFileName = model.AductRefFile;
                    string fileName = model.AductRefImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Aduct/RefFile"), uniqueFileName);
                    model.AductRefImageFile.SaveAs(filePath);
                    model.AductRefFile = uniqueFileName;
                    if (prevFileName != null)
                    {
                        string fullPath = Request.MapPath("~/UploadedFiles/Aduct/RefFile/" + prevFileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _aductMassSpectraRepository.Update(model);
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
            else
            {
                TempData["Failed"] = "Failed";
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DeleteCompoundMS2(int MS2MassSpectraKey)
        {
            var model = _ms2MassSpectraRepository.GetByKey(MS2MassSpectraKey);
            var datafile = model.MS2DataFile;
            var reffile = model.MS2RefFile;

            if (model != null)
            {
                _ms2MassSpectraRepository.Delete(model.MS2MassSpectraKey);
                if (datafile != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/MS2/DataFile/" + datafile);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                if (reffile != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/MS2/RefFile/" + reffile);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteCompoundMS3(int MS3MassSpectraKey)
        {
            var model = _ms3MassSpectraRepository.GetByKey(MS3MassSpectraKey);
            var datafile = model.MS3DataFile;
            var reffile = model.MS3RefFile;

            if (model != null)
            {
                _ms3MassSpectraRepository.Delete(model.MS3MassSpectraKey);
                if (datafile != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/MS3/DataFile/" + datafile);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                if (reffile != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/MS3/RefFile/" + reffile);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteCompoundAduct(int AductMassSpectraKey)
        {
            var model = _aductMassSpectraRepository.GetByKey(AductMassSpectraKey);
            var datafile = model.AductDataFile;
            var reffile = model.AductRefFile;

            if (model != null)
            {
                _aductMassSpectraRepository.Delete(model.AductMassSpectraKey);
                if (datafile != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/Aduct/DataFile/" + datafile);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                if (reffile != null)
                {
                    string fullPath = Request.MapPath("~/UploadedFiles/Aduct/RefFile/" + reffile);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult GetMS2List(int compoundKey, int instrumentKey)
        {
            var model = _ms2MassSpectraRepository.GetAllByInstrumentKey(instrumentKey, compoundKey);
            return PartialView("_MS2ListPartialPage", model);
        }

        [HttpGet]
        public PartialViewResult GetMS3List(int compoundKey, int instrumentKey)
        {
            var model = _ms3MassSpectraRepository.GetAllByInstrumentKey(instrumentKey, compoundKey);
            return PartialView("_MS3ListPartialPage", model);
        }

        [HttpGet]
        public PartialViewResult GetAductList(int compoundKey, int instrumentKey)
        {
            var model = _aductMassSpectraRepository.GetAllByInstrumentKey(instrumentKey, compoundKey);
            return PartialView("_AductListPartialPage", model);
        }

        [HttpGet]
        public JsonResult GetAnnotation(int polarity)
        {
            var lstAnnotation = _annotationRepository.GetAll()
                .Where(a => a.IsActive == true && a.Polarity == polarity)
                .Select(x => new
                {
                    AnnotationKey = x.AnnotationKey,
                    AnnotationName = x.AnnotationName,
                }).ToList();

            return Json(lstAnnotation, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult CompoundSummary(int CompoundKey)
        {
            var model = new CompoundPreviewViewModel();
            var compoundmodel = _compoundRepository.GetByKey(CompoundKey);

            if (compoundmodel != null)
            {
                model.DatabaseKey = compoundmodel.DatabaseKey;
                model.DatabaseName = compoundmodel.DatabaseName;
                model.IUPACName = compoundmodel.IUPACName;
                model.OtherNames = compoundmodel.OtherNames;
                model.CASNo = compoundmodel.CASNo;
                model.CompoundKey = compoundmodel.CompoundKey;
                model.ChemicalStructureFile = compoundmodel.ChemicalStructureFile;
                string fileNameWithPath = Request.MapPath("~/UploadedFiles/Compound/ChemicalStructure/" + compoundmodel.ChemicalStructureFile);
                if (!System.IO.File.Exists(fileNameWithPath))
                {
                    model.ChemicalStructureFile = "";
                }
                model.CompoundName = compoundmodel.CompoundName;
                model.Formula = compoundmodel.Formula;
                model.Smiles = compoundmodel.Smiles;
                model.MolecularWeight_Input = compoundmodel.MolecularWeight_Input;
                model.CompiledBy = compoundmodel.CompiledBy;
                model.MS2List = _ms2MassSpectraRepository.GetByCompoundKey(CompoundKey).Select(x => new SelectListItem { Value = x.MS2MassSpectraKey.ToString(), Text = x.SpectraDescription }).ToList();
                model.MS3List = _ms3MassSpectraRepository.GetByCompoundKey(CompoundKey).Select(x => new SelectListItem { Value = x.MS3MassSpectraKey.ToString(), Text = x.SpectraDescription }).ToList();
                model.AductList = _aductMassSpectraRepository.GetByCompoundKey(CompoundKey).Select(x => new SelectListItem { Value = x.AductMassSpectraKey.ToString(), Text = x.SpectraDescription }).ToList();

            }

            var naturalLoss = new List<MSMajorNaturalLoses>();

            foreach (var pl in gls.GetPolarity().Where(p => p.Value != ""))
            {
                var lstMS2 = _ms2MassSpectraRepository.GetByCompoundKey(CompoundKey).Where(p => p.Polarity.ToString() == pl.Value).ToList();
                if (lstMS2.Count > 0)
                {
                    var objMS2 = lstMS2.OrderByDescending(d => d.MS2MassSpectraKey).FirstOrDefault();
                    ViewBag.ParentIon = objMS2.ParentIon + " " + _annotationRepository.GetByKey(objMS2.AnnotationKey).AnnotationName;
                    naturalLoss = _massSpectraDataValuesRepository.GetMassSpectraDataValuesByKey(objMS2.MS2MassSpectraKey,1).OrderByDescending(d => d.relative_int).Select(x => new MSMajorNaturalLoses
                    {
                        Product = Convert.ToInt32(x.mz),
                        RelativeIntensity = Math.Round(Convert.ToDouble(x.relative)),
                        NaturalLoss = Convert.ToInt32(objMS2.ParentIon) - Convert.ToInt32(x.mz)

                    }).ToList();

                    naturalLoss = naturalLoss.Where(x => x.NaturalLoss != 0).Take(5).ToList();

                    break;
                }

                var lstMS3 = _ms3MassSpectraRepository.GetByCompoundKey(CompoundKey).Where(p => p.Polarity.ToString() == pl.Value).ToList();
                if (lstMS3.Count > 0)
                {
                    var objMS3 = lstMS3.OrderByDescending(d => d.MS3MassSpectraKey).FirstOrDefault();
                    ViewBag.ParentIon = objMS3.ParentIon + " " + _annotationRepository.GetByKey(objMS3.AnnotationKey).AnnotationName;
                    naturalLoss = _massSpectraDataValuesRepository.GetMassSpectraDataValuesByKey(objMS3.MS3MassSpectraKey,3).OrderByDescending(d => d.relative_int).Select(x => new MSMajorNaturalLoses
                    {
                        Product = Convert.ToInt32(x.mz),
                        RelativeIntensity = Math.Round(Convert.ToDouble(x.relative)),
                        NaturalLoss = Convert.ToInt32(objMS3.ParentIon) - Convert.ToInt32(x.mz)

                    }).ToList();

                    naturalLoss = naturalLoss.Where(x => x.NaturalLoss != 0).Take(5).ToList();

                    break;
                }

                var lstAduct = _aductMassSpectraRepository.GetByCompoundKey(CompoundKey).Where(p => p.Polarity.ToString() == pl.Value).ToList();
                if (lstAduct.Count > 0)
                {
                    var objAduct = lstAduct.OrderByDescending(d => d.AductMassSpectraKey).FirstOrDefault();
                    ViewBag.ParentIon = objAduct.ParentIon + " " + _annotationRepository.GetByKey(objAduct.AnnotationKey).AnnotationName;
                    naturalLoss = _massSpectraDataValuesRepository.GetMassSpectraDataValuesByKey(objAduct.AductMassSpectraKey,2).OrderByDescending(d => d.relative_int).Select(x => new MSMajorNaturalLoses
                    {
                        Product = Convert.ToInt32(x.mz),
                        RelativeIntensity = Math.Round(Convert.ToDouble(x.relative)),
                        NaturalLoss = Convert.ToInt32(objAduct.ParentIon) - Convert.ToInt32(x.mz)

                    }).ToList();

                    naturalLoss = naturalLoss.Where(x => x.NaturalLoss != 0).Take(5).ToList();
                   
                    break;
                }
            }

            model.MSMajorNaturalLoses = naturalLoss;

            StringBuilder sb = new StringBuilder();
            sb = _compoundRepository.LoadXML(model.CompoundName);
            ViewBag.OtherInformation = sb.ToString();

            return PartialView("~/Views/Compound/_PreviewCompound.cshtml", model);
        }

        [HttpGet]
        public JsonResult GetCompoundNames()
        {
            var results = _compoundRepository.GetAll().Select(x => new { x.CompoundName }).DistinctBy(x => x.CompoundName).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCompiledByNames()
        {
            var results = _compoundRepository.GetAll().Select(x => new { x.CompiledBy }).DistinctBy(x => x.CompiledBy).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCASNoNames()
        {
            var results = _compoundRepository.GetAll().Where(x => x.CASNo != "").Select(x => new { x.CASNo }).DistinctBy(x => x.CASNo).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOtherNames()
        {
            var results = _compoundRepository.GetAll().Select(x => new { x.OtherNames }).DistinctBy(x => x.OtherNames).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetCompundStats()
        {
            CompoundStats model = _commonRepository.CompoundStats();

            return PartialView("_CompoundStats", model);
            //return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetMassSpectraDetails(int spectraKey, string spectraType)
        {       
            string strData = string.Empty;
            int massspectraType;
            MassSpectraViewModel model=new MassSpectraViewModel();
            if (spectraType == "MS2")
            {
                massspectraType = 1;
                var objMS = _ms2MassSpectraRepository.GetByKey(spectraKey);
                ViewBag.MassSpectrum = "MS2 : " + objMS.SpectraDescription;
                var objcompound = _compoundRepository.GetByKey(objMS.CompoundKey);
                model.CompoundName = objcompound.CompoundName;
                model.CASNo = objcompound.CASNo;
                model.Formula = objcompound.Formula;
                model.Instrument = _instrumentRepository.GetByKey(objMS.InstrumentKey).InstrumentName;
                model.MassSpectrum = objMS.SpectraDescription;
                model.MolecularWeight_Input = objcompound.MolecularWeight_Input;
                model.CompoundKey = objMS.CompoundKey;

                if (objMS.MS2RefFile != null)
                {
                    string fileNameWithPath = Request.MapPath("~/UploadedFiles/MS2/RefFile/" + objMS.MS2RefFile);
                    StreamReader myFile = new StreamReader(fileNameWithPath);
                    String myString = myFile.ReadToEnd();
                    myFile.Dispose();
                    String strReplace;
                    for (Int32 counter = 10; counter > 1; counter--)
                    {
                        strReplace = String.Concat((string[])ArrayList.Repeat("\r\n", counter).ToArray(typeof(String)));
                        myString = myString.Replace(strReplace, "\r\n");
                        strReplace = String.Concat((string[])ArrayList.Repeat("\r\n ", counter).ToArray(typeof(String)));
                        myString = myString.Replace(strReplace, "\r\n");
                    }

                    myString = myString.Replace("\r\n", "<br>");
                    ViewBag.ReferenceFile = "<pre>" + myString + "</pre>";

                }
                if (objMS.MS2DataFile != null)
                {
                    string fileNameWithPath = Request.MapPath("~/UploadedFiles/MS2/DataFile/" + objMS.MS2DataFile);
                    StreamReader myFile = new StreamReader(fileNameWithPath);
                    String myString = myFile.ReadToEnd();
                    myFile.Dispose();

                    ViewBag.DataFile = "<pre>" + myString + "</pre>";
                    // ViewBag.DataFile = "<pre style='font-weight: bold;'><u>m/z</u>\t<u>R.I.</u></pre><pre>" + myString.Replace("\r\n", "<br>") + "</pre>";
                }

            }
            else if (spectraType == "MS3")
            {
                massspectraType = 3;
                var objMS = _ms3MassSpectraRepository.GetByKey(spectraKey);
                ViewBag.MassSpectrum = "MS3 : " + objMS.SpectraDescription;
                var objcompound = _compoundRepository.GetByKey(objMS.CompoundKey);
                model.CompoundName = objcompound.CompoundName;
                model.CASNo = objcompound.CASNo;
                model.Formula = objcompound.Formula;
                model.Instrument = _instrumentRepository.GetByKey(objMS.InstrumentKey).InstrumentName;
                model.MassSpectrum = objMS.SpectraDescription;
                model.MolecularWeight_Input = objcompound.MolecularWeight_Input;
                model.CompoundKey = objMS.CompoundKey;

                if (objMS.MS3RefFile != null)
                {
                    string fileNameWithPath = Request.MapPath("~/UploadedFiles/MS3/RefFile/" + objMS.MS3RefFile);
                    StreamReader myFile = new StreamReader(fileNameWithPath);
                    String myString = myFile.ReadToEnd();
                    myFile.Dispose();
                    String strReplace;
                    for (Int32 counter = 10; counter > 1; counter--)
                    {
                        strReplace = String.Concat((string[])ArrayList.Repeat("\r\n", counter).ToArray(typeof(String)));
                        myString = myString.Replace(strReplace, "\r\n");
                        strReplace = String.Concat((string[])ArrayList.Repeat("\r\n ", counter).ToArray(typeof(String)));
                        myString = myString.Replace(strReplace, "\r\n");
                    }

                    myString = myString.Replace("\r\n", "<br>");

                    ViewBag.ReferenceFile = "<pre>" + myString + "</pre>";
                }
                if (objMS.MS3DataFile != null)
                {
                    string fileNameWithPath = Request.MapPath("~/UploadedFiles/MS3/DataFile/" + objMS.MS3DataFile);
                    StreamReader myFile = new StreamReader(fileNameWithPath);
                    String myString = myFile.ReadToEnd();
                    myFile.Dispose();

                    ViewBag.DataFile = "<pre>" + myString + "</pre>";
                    // ViewBag.DataFile = "<pre style='font-weight: bold;'><u>m/z</u>\t<u>R.I.</u></pre><pre>" + myString.Replace("\r\n", "<br>") + "</pre>";
                }
 
            }
            else
            {
                massspectraType = 2;
                var objMS = _aductMassSpectraRepository.GetByKey(spectraKey);
                ViewBag.MassSpectrum = "Adduct : " + objMS.SpectraDescription;
                var objcompound = _compoundRepository.GetByKey(objMS.CompoundKey);
                model.CompoundName = objcompound.CompoundName;
                model.CASNo = objcompound.CASNo;
                model.Formula = objcompound.Formula;
                model.Instrument = _instrumentRepository.GetByKey(objMS.InstrumentKey).InstrumentName;
                model.MassSpectrum = objMS.SpectraDescription;
                model.MolecularWeight_Input = objcompound.MolecularWeight_Input;
                model.CompoundKey = objMS.CompoundKey;

                if (objMS.AductRefFile != null)
                {
                    string fileNameWithPath = Request.MapPath("~/UploadedFiles/Aduct/RefFile/" + objMS.AductRefFile);
                    StreamReader myFile = new StreamReader(fileNameWithPath);
                    String myString = myFile.ReadToEnd();
                    myFile.Dispose();
                    String strReplace;
                    for (Int32 counter = 10; counter > 1; counter--)
                    {
                        strReplace = String.Concat((string[])ArrayList.Repeat("\r\n", counter).ToArray(typeof(String)));
                        myString = myString.Replace(strReplace, "\r\n");
                        strReplace = String.Concat((string[])ArrayList.Repeat("\r\n ", counter).ToArray(typeof(String)));
                        myString = myString.Replace(strReplace, "\r\n");
                    }

                    myString = myString.Replace("\r\n", "<br>");
                    ViewBag.ReferenceFile = "<pre>" + myString + "</pre>";
                }
                if (objMS.AductDataFile != null)
                {
                    string fileNameWithPath = Request.MapPath("~/UploadedFiles/Aduct/DataFile/" + objMS.AductDataFile);
                    StreamReader myFile = new StreamReader(fileNameWithPath);
                    String myString = myFile.ReadToEnd();
                    myFile.Dispose();

                    ViewBag.DataFile = "<pre>" + myString + "</pre>";
                    //  ViewBag.DataFile = "<pre style='font-weight: bold;'><u>m/z</u>\t<u>R.I.</u></pre><pre>" + myString.Replace("\r\n", "<br>") + "</pre>";
                }            
            }

           // int massspectraType = (spectraType == "MS2" ? 1 : spectraType == "MS3" ? 3 : 2);
            // var chartdata = _massSpectraDataValuesRepository.GetMassSpectraDataValuesByKey(spectraKey, massspectraType);
            ViewBag.IframeUrl = AppSettings.BaseUrl + "/Chart/Index/?sk=" + spectraKey + "&st=" + massspectraType;

            //if (chartdata.Count() > 0)
            //{
            //    foreach (var it in chartdata)
            //    {
            //        strData += ("[" + it.mz + ", " + it.relative + "],");
            //    }
            //    strData = strData.Substring(0, strData.Length - 1);
            //    strData = "var s1 = [" + strData.Trim() + "];";
            //}
            //else
            //    strData = "var s1 = [[]];";

            //ViewBag.ChartData = "<script class='code' type='text/javascript'>" + strData + "</script>";

            //if (chartdata.Count() > 0)
            //{
            //    foreach (var it in chartdata)
            //    {
            //        strLabels += ("'"+it.mz + "',");
            //        strDataset += (it.relative + ",");
            //    }

            //    strLabels = strLabels.Substring(0, strLabels.Length - 1);
            //    strLabels = "[" + strLabels.Trim() + "]";

            //    strDataset = strDataset.Substring(0, strDataset.Length - 1);
            //    strDataset = "[" + strDataset.Trim() + "]";
            //}
            //else
            //{
            //    strLabels = "[]";
            //    strDataset = "[]";
            //}

            //  model.MassChartLabels = "";
            //  model.MassChartData = "";

            return PartialView("_MassSpectraDetails", model);
        }

        [HttpGet]
        public ActionResult ActivateDeActivate(int compoundKey)
        {
            try
            {
                _compoundRepository.ActivateDeActivate(compoundKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}