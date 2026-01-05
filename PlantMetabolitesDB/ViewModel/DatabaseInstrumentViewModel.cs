using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class DatabaseInstrumentViewModel
    {
        public int DatabaseInstrumentKey { get; set; }
        public Int16 DatabaseKey { get; set; }
        public Int16 InstrumentKey { get; set; }
    }

    public class DbInstViewModel
    {
        public Int16 InstrumentKey { get; set; }
        public Int16 DatabaseKey { get; set; }
        public string DatabaseName { get; set; }
        public bool IsCheked { get; set; }


    }
}