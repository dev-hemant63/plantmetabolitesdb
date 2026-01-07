using PlantMetabolitesDB.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TandemDB.Models
{
    public class Master_MS1MassSpectra
    {
        [Key]
        public int MS1MassSpectraKey { get; set; }

        public int PlantMetabolitesKey { get; set; }

        public int InstrumentKey { get; set; }

        public int Polarity { get; set; }

        public Int16 AnnotationKey { get; set; }

        public string PartsOfPlant { get; set; }

        public string SampleType { get; set; }

        public string TaxonomistName { get; set; }

        public string VoucherNo { get; set; }

        public string HerbariumDepositedAt { get; set; }

        public string DateOfCollection { get; set; }

        public string GeoLocation { get; set; }

        public string SpectrumAveraging { get; set; }

        public string HPLCUPLCMethodeFilePath { get; set; }

        public string ExtractionMethodeFilePath { get; set; }

        public string FingerprintFilePath { get; set; }

        public string MS1RefFilePath { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public virtual PlantMetabolites PlantMetabolites { get; set; }
        [ForeignKey("AnnotationKey")]
        public virtual Master_Annotation Master_Annotation { get; set; }
    }
}