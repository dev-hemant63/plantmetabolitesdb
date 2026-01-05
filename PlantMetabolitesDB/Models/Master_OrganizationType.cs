using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_OrganizationType")]
    public class Master_OrganizationType
    {
        [Key]
        public Int16 OrganizationTypeKey { get; set; }
        public string OrganizationTypeName { get; set; }
        public bool IsAddByAdmin { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}