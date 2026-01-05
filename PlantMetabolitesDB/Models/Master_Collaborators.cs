using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_Collaborators")]
    public class Master_Collaborators
    {
        [Key]
        public Int16 CollaboratorsKey { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public Int16 UserType { get; set; }
        public string Designation { get; set; }
        public string Affiliation { get; set; }
        public string ContactNo { get; set; }
        public Int16 CountryKey { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        [ForeignKey("CountryKey")]
        public virtual Master_Country Country { get; set; }
    }
}