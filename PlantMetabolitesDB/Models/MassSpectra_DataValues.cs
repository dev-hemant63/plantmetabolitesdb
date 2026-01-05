using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantMetabolitesDB.Models
{
    [Table("MassSpectra_DataValues")]
    public class MassSpectra_DataValues
    {
        [Key]
        public int MSDataValueKey { get; set; }
        public Int16 MassSpectraKey { get; set; }
        public string mz { get; set; }
        public string absolute { get; set; }
        public string relative { get; set; }
        public int mz_int { get; set; }
        public int absolute_int { get; set; }
        public int relative_int { get; set; }
        public Int16 MassSpectraType { get; set; }
    }
}