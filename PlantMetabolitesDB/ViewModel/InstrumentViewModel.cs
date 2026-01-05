using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class InstrumentViewModel : CommonViewModel
    {
        public Int16 InstrumentKey { get; set; }
        [DisplayName("Instrument Name")]
        public string InstrumentName { get; set; }
        [DisplayName("Database Name")]
        public Int16 DatabaseKey { get; set; }
        public List<DatabaseViewModel> Databases { get; set; }
        public int DatabaseInstrumentKey { get; set; }
        public string DatabaseName { get; set; }

    }
}