using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_MS2MassSpectra")]
    public class Master_MS2MassSpectra
    {
        [Key]
        public Int16 MS2MassSpectraKey { get; set; }
        public byte Polarity { get; set; }
        public string ParentIon { get; set; }
        public string SpectraDescription { get; set; }
        public string MS2SpectraFile { get; set; }
        public string MS2DataFile { get; set; }
        public string MS2RefFile { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public Int16 InstrumentKey { get; set; }
        [ForeignKey("InstrumentKey")]
        public virtual Master_Instrument Instruments { get; set; }
        public Int16 AnnotationKey { get; set; }
        [ForeignKey("AnnotationKey")]
        public virtual Master_Annotation Annotations { get; set; }      
        public Int16 CompoundKey { get; set; }
        [ForeignKey("CompoundKey")]
        public virtual Master_Compound Compounds { get; set; }

    }
}