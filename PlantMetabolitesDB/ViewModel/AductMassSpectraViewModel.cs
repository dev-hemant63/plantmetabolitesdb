using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using System.Web.Mvc;
using System.ComponentModel;

namespace PlantMetabolitesDB.ViewModel
{
    public class AductMassSpectraViewModel : CommonViewModel
    {
        [Key]
        public Int16 AductMassSpectraKey { get; set; }
        [DisplayName("Polarity")]
        public byte Polarity { get; set; }
        [DisplayName("Parent Ion")]
        public string ParentIon { get; set; }
        [DisplayName("Spectra Description")]
        public string SpectraDescription { get; set; }
        [ForeignKey("InstrumentKey")]
        [DisplayName("Instrument")]
        public Int16 InstrumentKey { get; set; }
        public List<InstrumentViewModel> Instruments { get; set; }
        [ForeignKey("AnnotationKey")]
        [DisplayName("Annotation")]
        public Int16 AnnotationKey { get; set; }
        public List<AnnotationViewModel> Annotations { get; set; }
        [ForeignKey("CompoundKey")]
        public Int16 CompoundKey { get; set; }
        public CompoundViewModel Compounds { get; set; }
        public List<SelectListItem> Polarities { get; set; }
        public string PolarityName { get; set; }
        public string InstrumentName { get; set; }
        public string AnnotationName { get; set; }
        [DisplayName("Aduct Spectra File")]
        public string AductSpectraFile { get; set; }
        [DisplayName("Aduct Data File")]
        public string AductDataFile { get; set; }
        [DisplayName("Aduct Ref File")]
        public string AductRefFile { get; set; }
        public HttpPostedFileBase AductSpectraImageFile { get; set; }
        public HttpPostedFileBase AductDataImageFile { get; set; }
        public HttpPostedFileBase AductRefImageFile { get; set; }
        public List<MassSpectra_DataValues> lstMassSpectra_DataValues { get; set; }
    }
}