using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    public class MSSearchController : Controller
    {
        private ICompoundRepository _compoundRepository;
        private IDatabaseRepository _databaseRepository;
        private IInstrumentRepository _instrumentRepository;
        private IMS2MassSpectraRepository _ms2MassSpectraRepository;
        private IMS3MassSpectraRepository _ms3MassSpectraRepository;
        private IAductMassSpectraRepository _aductMassSpectraRepository;
        private IMassSpectraDataValuesRepository _massSpectraDataValuesRepository;

        GeneralClass gls = new GeneralClass();
        SearchDBLayer searchDBLayer = new SearchDBLayer();
        String newDataSheetFileName = String.Empty;
        List<MassSpectra_MSAdv_Search> lstMassSpectra_Advanced_Search = new List<MassSpectra_MSAdv_Search>();
        public MSSearchController()
        {
            _databaseRepository = new DatabaseRepository(new PlantMetabolitesDBContext());
            _instrumentRepository = new InstrumentRepository(new PlantMetabolitesDBContext());
            _compoundRepository = new CompoundRepository(new PlantMetabolitesDBContext());
            _ms2MassSpectraRepository = new MS2MassSpectraRepository(new PlantMetabolitesDBContext());
            _ms3MassSpectraRepository = new MS3MassSpectraRepository(new PlantMetabolitesDBContext());
            _aductMassSpectraRepository = new AductMassSpectraRepository(new PlantMetabolitesDBContext());
            _massSpectraDataValuesRepository = new MassSpectraDataValuesRepository(new PlantMetabolitesDBContext());
        }
        public MSSearchController(IDatabaseRepository databaseRepository, 
            IInstrumentRepository instrumentRepository, 
            ICompoundRepository compoundRepository,
            IMS2MassSpectraRepository ms2MassSpectraRepository,
            IMS3MassSpectraRepository ms3MassSpectraRepository,
            IAductMassSpectraRepository aductMassSpectraRepository,
            IMassSpectraDataValuesRepository massSpectraDataValuesRepository)
        {
            _databaseRepository = databaseRepository;
            _instrumentRepository = instrumentRepository;
            _compoundRepository= compoundRepository;
            _ms2MassSpectraRepository = ms2MassSpectraRepository;
            _ms3MassSpectraRepository = ms3MassSpectraRepository;
            _aductMassSpectraRepository = aductMassSpectraRepository;
            _massSpectraDataValuesRepository = massSpectraDataValuesRepository;
        }
        // GET: MSSearch
        public ActionResult Index()
        {
            MSIonSearchViewModel model = new MSIonSearchViewModel();
            model.ReportTops = gls.GetReportTop().ToList();
            model.ReportTopKey = 10;
            model.IntensityTypes = gls.GetIntensityType();
            model.IntensityTypeKey = 1;
            model.Tolerances = gls.GetTolerance();
            model.ToleranceKey = 5;
            model.Polaritys = gls.GetPolarity();
            model.PolarityKey = 1;
            model.ProductIons = gls.GetProductIons();
            model.ProductIonKey = 3;
            model.Databases = _databaseRepository.GetAll().Where(d => d.IsActive == true).ToList();

            model.Instruments = new List<InstrumentViewModel>();
            ViewBag.ProductIonCount = model.ProductIonKey;

            return View(model);
        }


        public ActionResult GetProductIon(int pdcount)
        {
            List<GenerateProductIon> model = new List<GenerateProductIon>();
            for (int i = 0; i < pdcount; i++)
            {
                model.Add(new GenerateProductIon()
                {
                    Intensity = null,
                    mz = null

                });
            }
            return PartialView("_GenerateProductIonPartialPage", model);
        }

        [HttpPost]
        public ActionResult GetMSSearch(MSIonSearchViewModel model)
        {
            MassSpectra_MSAdv_Search objMassSpectra_MSAdv_Search = null;
            if (model.GenerateProductIons != null)
            {
                
                objMassSpectra_MSAdv_Search = new MassSpectra_MSAdv_Search();
                objMassSpectra_MSAdv_Search.Query = "1";
                objMassSpectra_MSAdv_Search.Input_InstrumentKey = model.InstrumentKey;
                objMassSpectra_MSAdv_Search.Input_ParentIon = model.PrecursorIon;
                objMassSpectra_MSAdv_Search.Input_Polarity = model.PolarityKey;
                objMassSpectra_MSAdv_Search.Input_ReportTop = model.ReportTopKey;
                objMassSpectra_MSAdv_Search.Input_AnnotationKey = 0;
                objMassSpectra_MSAdv_Search.Input_AnnotationType = 1;
                objMassSpectra_MSAdv_Search.Input_IncludeMS3PrecursorIonKey = 0;
                objMassSpectra_MSAdv_Search.Input_Tolerance = model.ToleranceKey;
                objMassSpectra_MSAdv_Search.Input_DataBaseKey = model.DatabaseKey;

                List<MassSpectra_MSAdv_Search_InputValues> lstMassSpectra_MSAdv_Search_InputValues = new List<MassSpectra_MSAdv_Search_InputValues>();
                MassSpectra_MSAdv_Search_InputValues objMassSpectra_MSAdv_Search_InputValues = null;
                foreach (var item in model.GenerateProductIons)
                {
                    String txtMZ = item.mz.ToString();
                    String txtIntensity = item.Intensity.ToString();

                    if (GeneralClass.IsNumeric(txtMZ) && GeneralClass.IsNumeric(txtIntensity))
                    {
                        objMassSpectra_MSAdv_Search_InputValues = new MassSpectra_MSAdv_Search_InputValues();

                        Double dbValue;
                        Double.TryParse(txtMZ, out dbValue);
                        objMassSpectra_MSAdv_Search_InputValues.Input_mz_int = Convert.ToInt32(Math.Round(dbValue));

                        Double.TryParse(txtIntensity, out dbValue);
                        if (model.IntensityTypeKey.Equals(1))
                        {
                            objMassSpectra_MSAdv_Search_InputValues.Input_relative_int = Convert.ToInt32(Math.Round(dbValue));
                            if (!lstMassSpectra_MSAdv_Search_InputValues.Contains(objMassSpectra_MSAdv_Search_InputValues))
                                lstMassSpectra_MSAdv_Search_InputValues.Add(objMassSpectra_MSAdv_Search_InputValues);
                        }
                        else
                        {
                            objMassSpectra_MSAdv_Search_InputValues.Input_absolute_int = Convert.ToInt32(Math.Round(dbValue));
                            if (!lstMassSpectra_MSAdv_Search_InputValues.Contains(objMassSpectra_MSAdv_Search_InputValues))
                                lstMassSpectra_MSAdv_Search_InputValues.Add(objMassSpectra_MSAdv_Search_InputValues);
                        }
                    }
                }
                if (!model.IntensityTypeKey.Equals(1))
                    ReCalculateInput_relative(ref lstMassSpectra_MSAdv_Search_InputValues);

                objMassSpectra_MSAdv_Search.lstMassSpectra_MSAdv_Search_InputValues = lstMassSpectra_MSAdv_Search_InputValues;
                lstMassSpectra_Advanced_Search.Add(objMassSpectra_MSAdv_Search);
            }
            List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search_Result = new List<MassSpectra_MSAdv_Search>();
            lstMassSpectra_MSAdv_Search_Result = searchDBLayer.ReadMassSpectra_MSIon_Search(lstMassSpectra_Advanced_Search);
            Session["InputProductIons"] = lstMassSpectra_Advanced_Search;
            ViewBag.CurrDate = DateTime.UtcNow.AddMinutes(330).ToString("dd/MM/yyyy, hh:mm tt");

            return PartialView("_MSSearchResultPartialPage", lstMassSpectra_MSAdv_Search_Result);
        }

        private void ReCalculateInput_relative(ref List<MassSpectra_MSAdv_Search_InputValues> lstMassSpectra_MSAdv_Search_InputValues)
        {
            lstMassSpectra_MSAdv_Search_InputValues.Sort(delegate (MassSpectra_MSAdv_Search_InputValues a, MassSpectra_MSAdv_Search_InputValues b)
            {
                return b.Input_absolute_int.CompareTo(a.Input_absolute_int);
            }); ;

            for (int i = 0; i < lstMassSpectra_MSAdv_Search_InputValues.Count; i++)
            {
                lstMassSpectra_MSAdv_Search_InputValues[i].Input_relative_int = (lstMassSpectra_MSAdv_Search_InputValues[i].Input_absolute_int * 100) / lstMassSpectra_MSAdv_Search_InputValues[0].Input_absolute_int;
            }
        }

        [HttpGet]
        public JsonResult GetInstrument(int databaseKey)
        {
            
            if (databaseKey == 0)
            {
                var lstInstrument = _instrumentRepository.GetAll().Where(d => d.IsActive == true)
                .Select(x => new
                 {
                     x.InstrumentKey,
                     x.InstrumentName,
                 }).ToList();

                return Json(lstInstrument, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lstInstrument = _instrumentRepository.GetInstrumentByDatabaseKey(databaseKey)
               .Select(x => new
               {
                   x.InstrumentKey,
                   x.InstrumentName,
               }).ToList();

                return Json(lstInstrument, JsonRequestBehavior.AllowGet);
            }

            
        }


        public ActionResult GetDataCoverage(int compoundKey, int matchedAnnotationType, int matchedMassSpectraKey, int matchedInputIndex)
        {
            var standardData = new StandardDataViewModel();
            var sampledata = new SampleDataViewModel();
            List<MassSpectraDataValuesViewModel> lstInputData = new List<MassSpectraDataValuesViewModel>();
            List<MassSpectraDataValuesViewModel> lstLibraryData = new List<MassSpectraDataValuesViewModel>();
            List<MassSpectraDataValuesSearchResult> lstMassSpectraResultData = new List<MassSpectraDataValuesSearchResult>();
            DataCoverageViewModel DCVM = new DataCoverageViewModel();

            if (matchedMassSpectraKey > 0)
            {
                var compoundmodel = _compoundRepository.GetByKey(compoundKey);

                standardData.DatabaseName = compoundmodel.DatabaseName;
                standardData.CASNo = compoundmodel.CASNo;
                standardData.CompoundKey = compoundmodel.CompoundKey;
                standardData.CompoundName = compoundmodel.CompoundName;
                standardData.Formula = compoundmodel.Formula;
                standardData.MolecularWeight_Input = compoundmodel.MolecularWeight_Input;

                if (matchedAnnotationType == 1)
                {
                    var objMS2 = _ms2MassSpectraRepository.GetByKey(matchedMassSpectraKey);
                    standardData.MassSpectrum = objMS2.SpectraDescription + " (MS2)";
                    standardData.InstrumentName = _instrumentRepository.GetByKey(objMS2.InstrumentKey).InstrumentName;
                    standardData.Polarity = objMS2.Polarity;
                    standardData.Title = objMS2.SpectraDescription;
                    standardData.Charge = objMS2.Polarity == 1 ? "1+" : "1-";
                    if (objMS2.ParentIon == "")
                    {
                        standardData.Premass = "*";
                    }
                    else
                    {
                        standardData.Premass = objMS2.ParentIon;
                    }
                }
                else if (matchedAnnotationType == 2)
                {
                    var objAduct = _aductMassSpectraRepository.GetByKey(matchedMassSpectraKey);
                    standardData.MassSpectrum = objAduct.SpectraDescription + " (Adduct)";
                    standardData.InstrumentName = _instrumentRepository.GetByKey(objAduct.InstrumentKey).InstrumentName;
                    standardData.Polarity = objAduct.Polarity;
                    standardData.Title = objAduct.SpectraDescription;
                    standardData.Charge = objAduct.Polarity == 1 ? "1+" : "1-";
                    if (objAduct.ParentIon == "")
                    {
                        standardData.Premass = "*";
                    }
                    else
                    {
                        standardData.Premass = objAduct.ParentIon;
                    }
                }
                else
                {
                    var objMS3 = _ms3MassSpectraRepository.GetByKey(matchedMassSpectraKey);
                    standardData.MassSpectrum = objMS3.SpectraDescription + " (MS3)";
                    standardData.InstrumentName = _instrumentRepository.GetByKey(objMS3.InstrumentKey).InstrumentName;
                    standardData.Polarity = objMS3.Polarity;
                    standardData.Title = objMS3.SpectraDescription;
                    standardData.Charge = objMS3.Polarity == 1 ? "1+" : "1-";
                    if (objMS3.ParentIon == "")
                    {
                        standardData.Premass = "*";
                    }
                    else
                    {
                        standardData.Premass = objMS3.ParentIon;
                    }
                }

            }

            lstLibraryData = _massSpectraDataValuesRepository.GetMassSpectraDataValuesByKey(matchedMassSpectraKey, matchedAnnotationType).OrderBy(x => x.mz_int).ToList();
            if (Session["InputProductIons"] != null)
            {
                var result = (List<MassSpectra_MSAdv_Search>)Session["InputProductIons"];
                result = result.Where(d => d.Input_Polarity == standardData.Polarity).ToList();
                sampledata.Title = result.ElementAt(0).Query;
                sampledata.Charge = result.ElementAt(0).Input_Polarity == 1 ? "1+" : "1-";              
                if (result.ElementAt(0).Input_ParentIon == null )
                {
                    sampledata.Premass = "*";
                }
                else
                {
                    sampledata.Premass = result.ElementAt(0).Input_ParentIon;
                }

                lstInputData = (from dt in result.ElementAt(0).lstMassSpectra_MSAdv_Search_InputValues
                                select new MassSpectraDataValuesViewModel
                                {
                                    mz_int = dt.Input_mz_int,
                                    relative_int = dt.Input_relative_int,
                                    relative = dt.mznew
                                }).OrderBy(x => x.mz_int).ToList();
            }

            List<int> lstData = new List<int>();
            int libCount = lstLibraryData.Count - 1;
            int inpCount = lstInputData.Count - 1;

            var lstData1 = lstInputData.Select(d => d.mz_int).ToList();
            var lstData2 = lstLibraryData.Select(d => d.mz_int).ToList();

            var mergeList = lstData1.Union(lstData2).OrderBy(x => x).ToArray();

            var lstlibFault = new List<int>();
            var lstintFault = new List<int>();

            foreach (var item in mergeList)
            {
                int indOne = -1;
                int indTwo = -1;

                var libData = lstLibraryData.FirstOrDefault(f => f.mz_int == item );
                var inpData = lstInputData.FirstOrDefault(f => f.mz_int == item );

                indOne = lstLibraryData.IndexOf(libData);
                indTwo = lstInputData.IndexOf(inpData);

                if (indTwo>=0 && indOne >= 0)
                {
                    lstMassSpectraResultData.Add(new MassSpectraDataValuesSearchResult
                    {
                        ClassName = "text-success bold ms-matched",
                        input_mz_int = inpData.mz_int,
                        input_relative = inpData.relative_int.ToString(),
                        library_mz_int = libData.mz_int,
                        library_relative = libData.relative,

                    });
                }
                else if (indOne >= 0)
                {
                    lstMassSpectraResultData.Add(new MassSpectraDataValuesSearchResult
                    {
                        ClassName = "text-black bold ms-unmatched",
                        input_mz_int = 0,
                        input_relative = "--",
                        library_mz_int = libData.mz_int,
                        library_relative = libData.relative,

                    });
                }
                else if (indTwo >= 0)
                {
                    lstMassSpectraResultData.Add(new MassSpectraDataValuesSearchResult
                    {
                        ClassName = "text-black bold ms-unmatched",
                        input_mz_int = inpData.mz_int,
                        input_relative = inpData.relative_int.ToString(),
                        library_mz_int = 0,
                        library_relative = "--",

                    }); ;
                }
            }

            DCVM.SampleData = sampledata;
            DCVM.StandardData = standardData;
            DCVM.MassSpectraResultData = lstMassSpectraResultData;
            ViewBag.CurrDate = DateTime.UtcNow.AddMinutes(330).ToString("dd/MM/yyyy, hh:mm tt");

            return PartialView("_DataCoveragePartial", DCVM);
        }
    }
}