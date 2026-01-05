using Microsoft.Ajax.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    public class MolecularWeightController : Controller
    {
        private ISearchRepository _searchRepository;
        private ICompoundRepository _compoundRepository;
        private IMS2MassSpectraRepository _ms2MassSpectraRepository;
        private IMS3MassSpectraRepository _ms3MassSpectraRepository;
        private IAductMassSpectraRepository _aductMassSpectraRepository;
        private IMassSpectraDataValuesRepository _massSpectraDataValuesRepository;
        private IAnnotationRepository _annotationRepository;
        private IInstrumentRepository _instrumentRepository;

        GeneralClass gls = new GeneralClass();

        public MolecularWeightController()
        {
            _searchRepository = new SearchRepository(new PlantMetabolitesDBContext());
            _compoundRepository = new CompoundRepository(new PlantMetabolitesDBContext());
            _ms2MassSpectraRepository = new MS2MassSpectraRepository(new PlantMetabolitesDBContext());
            _ms3MassSpectraRepository = new MS3MassSpectraRepository(new PlantMetabolitesDBContext());
            _aductMassSpectraRepository = new AductMassSpectraRepository(new PlantMetabolitesDBContext());
            _massSpectraDataValuesRepository = new MassSpectraDataValuesRepository(new PlantMetabolitesDBContext());
            _annotationRepository = new AnnotationRepository(new PlantMetabolitesDBContext());
            _instrumentRepository = new InstrumentRepository(new PlantMetabolitesDBContext());
        }

        public MolecularWeightController(ISearchRepository searchRepository, IInstrumentRepository instrumentRepository,
           ICompoundRepository compoundRepository, IMS2MassSpectraRepository ms2MassSpectraRepository,
           IMS3MassSpectraRepository ms3MassSpectraRepository, IAnnotationRepository annotationRepository,
           IAductMassSpectraRepository aductMassSpectraRepository, IMassSpectraDataValuesRepository massSpectraDataValuesRepository)
        {
            _searchRepository = searchRepository;
            _compoundRepository = compoundRepository;
            _ms2MassSpectraRepository = ms2MassSpectraRepository;
            _ms3MassSpectraRepository = ms3MassSpectraRepository;
            _aductMassSpectraRepository = aductMassSpectraRepository;
            _massSpectraDataValuesRepository = massSpectraDataValuesRepository;
            _annotationRepository = annotationRepository;
            _instrumentRepository = instrumentRepository;
        }

        // GET: MolecularWeight
        public ActionResult Index()
        {
            GeneralSearchViewModel model = new GeneralSearchViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult GetMolecularSearch(GeneralSearchViewModel model)
        {
            IEnumerable<GeneralSearchResultViewModel> lstEntity = new List<GeneralSearchResultViewModel>();

            if (ModelState.IsValid)
            {
                lstEntity = _searchRepository.GetGeneralSearchResult(model).ToList();

            }
            ViewBag.CurrDate = DateTime.UtcNow.AddMinutes(330).ToString("dd/MM/yyyy, hh:mm tt");

            return PartialView("_SearchResultPartial", lstEntity);

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
            ViewBag.CurrDate = DateTime.UtcNow.AddMinutes(330).ToString("dd/MM/yyyy, hh:mm tt");

            return PartialView("~/Views/Compound/_PreviewCompound.cshtml", model);
        }

        [HttpGet]
        public JsonResult GetMolecularWeightNames()
        {
            var results = _compoundRepository.GetAll().Where(x => x.MolecularWeight_Input != "" && x.IsActive == true).Select(x => new { x.MolecularWeight_Input }).DistinctBy(x => x.MolecularWeight_Input).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetMassSpectraDetails(int spectraKey, string spectraType)
        {
            string strLabels = string.Empty;
            string strDataset = string.Empty;
            int massspectraType;
            MassSpectraViewModel model = new MassSpectraViewModel();
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

                    // ViewBag.DataFile = "<pre style='font-weight: bold;'><u>m/z</u>\t<u>R.I.</u></pre><pre>" + myString.Replace("\r\n", "<br>") + "</pre>";
                    ViewBag.DataFile = "<pre>" + myString + "</pre>";
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

                    // ViewBag.DataFile = "<pre style='font-weight: bold;'><u>m/z</u>\t<u>R.I.</u></pre><pre>" + myString.Replace("\r\n", "<br>") + "</pre>";
                    ViewBag.DataFile = "<pre>" + myString + "</pre>";
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

                    // ViewBag.DataFile = "<pre style='font-weight: bold;'><u>m/z</u>\t<u>R.I.</u></pre><pre>" + myString.Replace("\r\n", "<br>") + "</pre>";
                    ViewBag.DataFile = "<pre>" + myString + "</pre>";
                }
            }

            //int massspectraType = spectraType == "MS2" ? 1 : spectraType == "MS3" ? 2 : 3;
            var chartdata = _massSpectraDataValuesRepository.GetMassSpectraDataValuesByKey(spectraKey, massspectraType);
            if (chartdata.Count() > 0)
            {
                foreach (var it in chartdata)
                {
                    strLabels += ("'" + it.mz + "',");
                    strDataset += (it.relative + ",");
                }

                strLabels = strLabels.Substring(0, strLabels.Length - 1);
                strLabels = "[" + strLabels.Trim() + "]";

                strDataset = strDataset.Substring(0, strDataset.Length - 1);
                strDataset = "[" + strDataset.Trim() + "]";
            }
            else
            {
                strLabels = "[]";
                strDataset = "[]";
            }

            model.MassChartLabels = strLabels;
            model.MassChartData = strDataset;

            return PartialView("~/Views/Compound/_MassSpectraDetails.cshtml", model);
        }

    }
}