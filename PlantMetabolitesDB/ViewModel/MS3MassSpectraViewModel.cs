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
    public class MS3MassSpectraViewModel : CommonViewModel
    {
        [Key]
        public Int16 MS3MassSpectraKey { get; set; }
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
        [DisplayName("MS3 Spectra File")]
        public string MS3SpectraFile { get; set; }
        [DisplayName("MS3 Data File")]
        public string MS3DataFile { get; set; }
        [DisplayName("MS3 Ref File")]
        public string MS3RefFile { get; set; }
        public HttpPostedFileBase MS3SpectraImageFile { get; set; }
        public HttpPostedFileBase MS3DataImageFile { get; set; }
        public HttpPostedFileBase MS3RefImageFile { get; set; }

        public List<MassSpectra_DataValues> lstMassSpectra_DataValues { get; set; }
    }
}