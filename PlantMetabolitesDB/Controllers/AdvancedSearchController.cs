using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.ViewModel;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using System.Data.Entity.Infrastructure;
using System.Runtime.InteropServices.ComTypes;

namespace PlantMetabolitesDB.Controllers
{
    public class AdvancedSearchController : Controller
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
        public AdvancedSearchController()
        {
            _databaseRepository = new DatabaseRepository(new PlantMetabolitesDBContext());
            _instrumentRepository = new InstrumentRepository(new PlantMetabolitesDBContext());
            _compoundRepository = new CompoundRepository(new PlantMetabolitesDBContext());
            _ms2MassSpectraRepository = new MS2MassSpectraRepository(new PlantMetabolitesDBContext());
            _ms3MassSpectraRepository = new MS3MassSpectraRepository(new PlantMetabolitesDBContext());
            _aductMassSpectraRepository = new AductMassSpectraRepository(new PlantMetabolitesDBContext());
            _massSpectraDataValuesRepository = new MassSpectraDataValuesRepository(new PlantMetabolitesDBContext());
        }

        public AdvancedSearchController(IDatabaseRepository databaseRepository,
           IInstrumentRepository instrumentRepository,
           ICompoundRepository compoundRepository,
           IMS2MassSpectraRepository ms2MassSpectraRepository,
           IMS3MassSpectraRepository ms3MassSpectraRepository,
           IAductMassSpectraRepository aductMassSpectraRepository,
           IMassSpectraDataValuesRepository massSpectraDataValuesRepository)
        {
            _databaseRepository = databaseRepository;
            _instrumentRepository = instrumentRepository;
            _compoundRepository = compoundRepository;
            _ms2MassSpectraRepository = ms2MassSpectraRepository;
            _ms3MassSpectraRepository = ms3MassSpectraRepository;
            _aductMassSpectraRepository = aductMassSpectraRepository;
            _massSpectraDataValuesRepository = massSpectraDataValuesRepository;
        }

        // GET: AdvancedSearch
        public ActionResult Index()
        {
            AdvancedSearchViewModel model = new AdvancedSearchViewModel();
            model.ReportTops = gls.GetReportTop().ToList();
            model.ReportTopKey = 3;
            model.IntensityTypes = gls.GetIntensityType();
            model.IntensityTypeKey = 1;
            model.Tolerances = gls.GetTolerance();
            model.ToleranceKey = 5;
            model.Databases = _databaseRepository.GetAll().Where(d => d.IsActive == true).ToList();
            model.Instruments = _instrumentRepository.GetAll().Where(d => d.IsActive == true).ToList();
            model.MS2IonSearch = true;
            model.MS3IonSearch = false;

            return View(model);
        }

        [HttpPost]
        public ActionResult GetAdvancedSearch(AdvancedSearchViewModel model)
        {
            if (model.DataFile != null && model.DataFile.ContentLength > 0)
            {
                string fileName = model.DataFile.FileName;
                string extension = Path.GetExtension(fileName);
                newDataSheetFileName = $@"{Guid.NewGuid()}" + extension + "";
                string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/UserDataFiles/"), newDataSheetFileName);
                model.DataFile.SaveAs(filePath);
                model.DataFileName = newDataSheetFileName;
                model.DataFilePathWithName = filePath;

                // string fileNameWithPath = Request.MapPath("~/UploadedFiles/MS2/UserDataFiles/" + newDataSheetFileName);
                StreamReader myFile = new StreamReader(filePath);
                String myString = myFile.ReadToEnd();
                myFile.Dispose();
                ViewBag.UserDataFile = "<pre>" + myString + "</pre>";


                lstMassSpectra_Advanced_Search = gls.GetMassSpectra_Advanced_Search(model);

                ViewBag.InputDataFiles = lstMassSpectra_Advanced_Search;
                Session["InputDataFiles"] = lstMassSpectra_Advanced_Search;

            }

            List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search_Result = searchDBLayer.ReadMassSpectra_Advanced_Search(lstMassSpectra_Advanced_Search);

            lstMassSpectra_MSAdv_Search_Result = lstMassSpectra_MSAdv_Search_Result.OrderByDescending(a => a.Data_mz_MatchFactor).ToList();
            lstMassSpectra_MSAdv_Search_Result = lstMassSpectra_MSAdv_Search_Result.Where(x => x.Data_mz_MatchFactor > 0.1).ToList();
            ViewBag.CurrDate = DateTime.UtcNow.AddMinutes(330).ToString("dd/MM/yyyy, hh:mm tt");

            return PartialView("_AdvancedSearchResultPartialPage", lstMassSpectra_MSAdv_Search_Result);
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

            GCDataInputValues objDataInputValues = null;
            List<GCDataInputValues> lstDataInputValues = new List<GCDataInputValues>();
            GCDataInputValues1 objDataInputValues1 = null;
            List<GCDataInputValues1> lstDataInputValues1 = new List<GCDataInputValues1>();

            List<GCDataInputValues> _lstLibraryData = new List<GCDataInputValues>();
            List<GCDataInputValues1> _lstInputData = new List<GCDataInputValues1>();

            List<int> tempMZ = new List<int>();
            List<int> tMZ = new List<int>();
            List<int> tMZ1 = new List<int>();

            String comdata = "<div class='clsgd'>Standard/Library Data</div>";
            String comdata1 = "<div class='clsgd'>Input Data</div>";

            comdata += "<div>START IONS</div>";
            comdata1 += "<div>START IONS</div>";

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
                    standardData.Premass = objMS2.ParentIon;

                    comdata += "<div>TITLE=" + objMS2.SpectraDescription + "</div>";
                    if (objMS2.Polarity == 1)
                    {
                        comdata += "<div>CHARGE=1+</div>";
                    }
                    else
                    {
                        comdata += "<div>CHARGE=1-</div>";
                    }

                    if (objMS2.ParentIon == "")
                    {
                        comdata += "<div>PREMASS= * </div>";
                    }
                    else
                    {
                        comdata += "<div>PREMASS=" + objMS2.ParentIon + "</div>";
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
                    standardData.Premass = objAduct.ParentIon;

                    comdata += "<div>TITLE=" + objAduct.SpectraDescription + "</div>";
                    if (objAduct.Polarity == 1)
                    {
                        comdata += "<div>CHARGE=1+</div>";
                    }
                    else
                    {
                        comdata += "<div>CHARGE=1-</div>";
                    }

                    if (objAduct.ParentIon == "")
                    {
                        comdata += "<div>PREMASS= * </div>";
                    }
                    else
                    {
                        comdata += "<div>PREMASS=" + objAduct.ParentIon + "</div>";
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
                    standardData.Premass = objMS3.ParentIon;

                    comdata += "<div>TITLE=" + objMS3.SpectraDescription + "</div>";
                    if (objMS3.Polarity == 1)
                    {
                        comdata += "<div>CHARGE=1+</div>";
                    }
                    else
                    {
                        comdata += "<div>CHARGE=1-</div>";
                    }

                    if (objMS3.ParentIon == "")
                    {
                        comdata += "<div>PREMASS= * </div>";
                    }
                    else
                    {
                        comdata += "<div>PREMASS=" + objMS3.ParentIon + "</div>";
                    }
                }

            }

            lstLibraryData = _massSpectraDataValuesRepository.GetMassSpectraDataValuesByKey(matchedMassSpectraKey, matchedAnnotationType).OrderBy(x => x.mz_int).ToList();

            foreach (var it in lstLibraryData)
            {               
                if (!tempMZ.Contains(Int32.Parse(it.mz)))
                {
                    tempMZ.Add(Int32.Parse(it.mz));
                }
                if (!tMZ.Contains(Int32.Parse(it.mz)))
                {
                    tMZ.Add(Int32.Parse(it.mz));
                }


                objDataInputValues = new GCDataInputValues();
                objDataInputValues.MZ = Convert.ToDouble(it.mz);
                objDataInputValues.RI = Convert.ToDouble(it.relative);
                lstDataInputValues.Add(objDataInputValues);
            }

            if (Session["InputDataFiles"] != null)
            {
                var result = (List<MassSpectra_MSAdv_Search>)Session["InputDataFiles"];
                result = result.Where(d => d.Input_Polarity == standardData.Polarity).ToList();
                sampledata.Title = result.ElementAt(0).Query;
                sampledata.Charge = result.ElementAt(0).Input_Polarity == 1 ? "1+" : "1-";
                sampledata.Premass = result.ElementAt(0).Input_ParentIon;

                lstInputData = (from dt in result.ElementAt(0).lstMassSpectra_MSAdv_Search_InputValues
                                select new MassSpectraDataValuesViewModel
                                {
                                    mz_int = dt.Input_mz_int,
                                    relative_int = dt.Input_relative_int,
                                    relative = dt.mznew
                                }).OrderBy(x => x.mz_int).ToList();

                comdata1 += "<div>TITLE= " + result.ElementAt(0).Query + "</div>";
              
                if (result.ElementAt(0).Input_Polarity == 1)
                {
                    comdata1 += "<div>CHARGE=1+</div>";
                }
                else
                {
                    comdata1 += "<div>CHARGE=1-</div>";
                }

                if (result.ElementAt(0).Input_ParentIon == "")
                {
                    comdata1 += "<div>PREMASS= * </div>";
                }
                else
                {
                    comdata1 += "<div>PREMASS=" + result.ElementAt(0).Input_ParentIon + "</div>";
                }
            }

            foreach (var it in lstInputData)
            {              
                objDataInputValues1 = new GCDataInputValues1();
                objDataInputValues1.MZ1 = Convert.ToDouble(it.mz_int);
                objDataInputValues1.RI1 = Convert.ToDouble(it.relative);
                lstDataInputValues1.Add(objDataInputValues1);
                if (!tempMZ.Contains(Int32.Parse(String.Format("{0}", it.mz_int))))
                {
                    tempMZ.Add(Int32.Parse(String.Format("{0}", it.mz_int)));
                }
                if (!tMZ1.Contains(Int32.Parse(String.Format("{0}", it.mz_int))))
                {
                    tMZ1.Add(Int32.Parse(String.Format("{0}", it.mz_int)));
                }
            }

            lstDataInputValues.OrderByDescending(x => x.MZ).ToList();
            lstDataInputValues1.OrderByDescending(x => x.MZ1).ToList();
            tempMZ.Sort();

            for (int i = 0; i < tempMZ.Count; i++)
            {
                String temp_mz = Convert.ToString(tempMZ[i]);

                int count = 0;
                for (int k = 0; k < lstDataInputValues.Count; k++)
                {
                    String tmpmz = Convert.ToString(lstDataInputValues[k].MZ);
                    String tmpri = Convert.ToString(lstDataInputValues[k].RI);

                    if ((temp_mz == tmpmz))
                    {
                        count = 1;
                        if (tMZ1.Contains(Int32.Parse(tmpmz)))
                        {
                            comdata += "<div style='color: #00994B;'><span class='left_value'>" + string.Format("{0}", tmpmz) + "</span><span class='right_value'>" + string.Format("{0:00}", tmpri) + "</span></div>";
                            _lstLibraryData.Add(new GCDataInputValues
                            {
                                MZ = Convert.ToDouble(tmpmz),
                                RI = Convert.ToDouble(tmpri)
                            });
                        }
                        else
                        {
                            comdata += "<div><span class='left_value'>" + string.Format("{0}", tmpmz) + "</span><span class='right_value'>" + string.Format("{0:00}", tmpri) + "</span></div>";
                            _lstLibraryData.Add(new GCDataInputValues
                            {
                                MZ = Convert.ToDouble(tmpmz),
                                RI = Convert.ToDouble(tmpri)
                            });
                        }
                    }

                }
                if (count == 0)
                {
                    comdata += "<div><span class='left_value'>-- </span><span class='right_value'> --</span></div>";
                    _lstLibraryData.Add(new GCDataInputValues
                    {
                        MZ = Convert.ToDouble(0),
                        RI = Convert.ToDouble(0)
                    });
                }

            }

            _lstLibraryData.OrderByDescending(d => d.RI).ToList();

            for (int i = 0; i < tempMZ.Count; i++)
            {
                String temp_mz = Convert.ToString(tempMZ[i]);

                int count = 0;
                for (int k = 0; k < lstDataInputValues1.Count; k++)
                {
                    String tmpmz1 = Convert.ToString(lstDataInputValues1[k].MZ1);
                    String tmpri1 = Convert.ToString(lstDataInputValues1[k].RI1);

                    if ((temp_mz == tmpmz1))
                    {
                        count = 1;
                        if (tMZ.Contains(Int32.Parse(tmpmz1)))
                        {
                            comdata1 += "<div style='color: #00994B;'><span class='left_value'>" + string.Format("{0}", tmpmz1) + "</span><span class='right_value'>" + string.Format("{0:00}", tmpri1) + "</span></div>";
                        }
                        else
                        {
                            comdata1 += "<div><span class='left_value'>" + string.Format("{0}", tmpmz1) + "</span><span class='right_value'>" + string.Format("{0:00}", tmpri1) + "</span></div>";
                        }
                    }

                }
                if (count == 0)
                {
                    comdata1 += "<div><span class='left_value'>-- </span><span class='right_value'> --</span></div>";

                }

            }

            comdata += "<div>END IONS</div>";
            comdata1 += "<div>END IONS</div>";

            ViewBag.ComData = comdata;
            ViewBag.ComData1 = comdata1;
            ViewBag.CurrDate = DateTime.UtcNow.AddMinutes(330).ToString("dd/MM/yyyy, hh:mm tt");

            DCVM.SampleData = sampledata;
            DCVM.StandardData = standardData;
            DCVM.MassSpectraResultData = lstMassSpectraResultData;

            return PartialView("_DataCoveragePartial", DCVM);

        }
    }
}
