using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TandemDB.Models;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_Annotation")]
    public class Master_Annotation
    {
        [Key]
        public Int16 AnnotationKey { get; set; }
        public string AnnotationName { get; set; }
        public byte Polarity { get; set; }
        public byte AnnotationType { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public virtual ICollection<Master_MS1MassSpectra> Master_MS1MassSpectra { get; set; }
    }
}