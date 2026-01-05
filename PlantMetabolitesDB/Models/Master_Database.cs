using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TandemDB.Models;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_Database")]
    public class Master_Database
    {
        [Key]
        public int DatabaseKey { get; set; }
        public string DatabaseName { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public virtual ICollection<PlantMetabolites> PlantMetabolites { get; set; }
    }
}