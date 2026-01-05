using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_Country")]
    public class Master_Country
    {
        [Key]
        public Int16? CountryKey { get; set; }
        public string CountryName { get; set; }
    }
}