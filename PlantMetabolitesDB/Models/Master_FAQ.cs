using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    public class Master_FAQ
    {
        [Key]
        public Int16 FAQKey { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UploadedFileName { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}