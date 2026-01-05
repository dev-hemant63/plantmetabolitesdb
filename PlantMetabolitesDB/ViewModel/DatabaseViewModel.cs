using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class DatabaseViewModel : CommonViewModel
    {
        public Int16 DatabaseKey { get; set; }
        [DisplayName("Name of plant family")]
        public string DatabaseName { get; set; }
        public bool IsCheked { get; set; }
        
    }

    public class DatabaseSearch
    {
        public string DatabaseName { get; set; }
    }
}