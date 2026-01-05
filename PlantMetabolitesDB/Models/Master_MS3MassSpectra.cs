using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_MS3MassSpectra")]
    public class Master_MS3MassSpectra
    {
        [Key]
        public Int16 MS3MassSpectraKey { get; set; }
        public byte Polarity { get; set; }
        public string ParentIon { get; set; }
        public string SpectraDescription { get; set; }
        public string MS3SpectraFile { get; set; }
        public string MS3DataFile { get; set; }
        public string MS3RefFile { get; set; }
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