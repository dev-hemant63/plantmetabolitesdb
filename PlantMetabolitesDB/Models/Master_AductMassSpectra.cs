using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_AductMassSpectra")]
    public class Master_AductMassSpectra
    {
        [Key]
        public Int16 AductMassSpectraKey { get; set; }
        public byte Polarity { get; set; }
        public string ParentIon { get; set; }
        public string SpectraDescription { get; set; }
        public string AductSpectraFile { get; set; }
        public string AductDataFile { get; set; }
        public string AductRefFile { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }   
        public Int16 InstrumentKey { get; set; }
        [ForeignKey("InstrumentKey")]
        public Master_Instrument Instruments { get; set; }       
        public Int16 AnnotationKey { get; set; }
        [ForeignKey("AnnotationKey")]
        public Master_Annotation Annotations { get; set; }       
        public Int16 CompoundKey { get; set; }
        [ForeignKey("CompoundKey")]
        public Master_Compound Compounds { get; set; }
    }
}