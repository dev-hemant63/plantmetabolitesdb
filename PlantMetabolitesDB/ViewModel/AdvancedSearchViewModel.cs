using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantMetabolitesDB.ViewModel
{
    public class AdvancedSearchViewModel
    {
        public string SearchTitle { get; set; }
        [DisplayName("Name of plant family")]
        public Int16 DatabaseKey { get; set; }
        [DisplayName("Instrument")]
        public Int16 InstrumentKey { get; set; }
        [DisplayName("MS2 Ion Search")]
        public bool? MS2IonSearch { get; set; }
        [DisplayName("MS3 Ion Search")]
        public bool? MS3IonSearch { get; set; }
        [DisplayName("Intensity Type")]
        public Int16 IntensityTypeKey { get; set; }
        [DisplayName("Report Top")]
        public Int16 ReportTopKey { get; set; }
        [DisplayName("Tolerance (With in %)")]
        public Int16 ToleranceKey { get; set; }
        [DisplayName("Data File")]
        public string DataFileName { get; set; }
        public HttpPostedFileBase DataFile { get; set; }
        public List<DatabaseViewModel> Databases { get; set; }
        public List<InstrumentViewModel> Instruments { get; set; }
        public List<SelectListItem> IntensityTypes { get; set; }
        public List<SelectListItem> ReportTops { get; set; }
        public List<SelectListItem> Tolerances { get; set; }
        public Int16 AnnotationKey { get; set; }
        public Int16 MassSpectraType { get; set; }
        public string DataFilePathWithName { get; set; }


    }
}