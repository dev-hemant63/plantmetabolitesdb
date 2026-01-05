using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantMetabolitesDB.ViewModel
{
    public class MSIonSearchViewModel
    {
        [DisplayName("Name of plant family")]
        public Int16 DatabaseKey { get; set; }
        [DisplayName("Instrument")]
        public Int16 InstrumentKey { get; set; }
        public string InstrumentName { get; set; }
        [DisplayName("Precursor Ion")]
        public string PrecursorIon { get; set; }
        [DisplayName("Polarity")]
        public Int16 PolarityKey { get; set; }
        [DisplayName("Report Top")]
        public Int16 ReportTopKey { get; set; }
        [DisplayName("No. of Product Ion(s)")]
        public Int16 ProductIonKey { get; set; }
        [DisplayName("Intensity Type")]
        public Int16 IntensityTypeKey { get; set; }   
        [DisplayName("Tolerance (With in %)")]
        public Int16 ToleranceKey { get; set; }
        public List<DatabaseViewModel> Databases { get; set; }
        public List<InstrumentViewModel> Instruments { get; set; }
        public List<SelectListItem> Polaritys { get; set; }
        public List<SelectListItem> IntensityTypes { get; set; }
        public List<SelectListItem> ReportTops { get; set; }
        public List<SelectListItem> Tolerances { get; set; }
        public List<SelectListItem> ProductIons { get; set; }
        public Int16 AnnotationKey { get; set; }
        public Int16 MassSpectraType { get; set; }
        public List<GenerateProductIon> GenerateProductIons { get; set; }

    }

    public class GenerateProductIon
    {
        public Int16? mz { get; set; }
        public Int16? Intensity { get; set; }
    }
}