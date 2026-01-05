using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_LatestUpdates")]
    public class Master_LatestUpdates
    {
        [Key]
        public Int16 LatestUpdatesKey { get; set; }
        public string Title { get; set; }
        public string UploadedFileName { get; set; }
        public string URL { get; set; }
        public Int16 MessageType { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}