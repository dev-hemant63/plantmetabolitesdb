using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_Database_Instrument")]
    public class Master_Database_Instrument
    {
        [Key]
        public int DatabaseInstrumentKey { get; set; }
        public Int16 DatabaseKey { get; set; }
        public Int16 InstrumentKey { get; set; }
    }
}