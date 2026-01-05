using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;

namespace PlantMetabolitesDB.ViewModel
{
    public class AdvisoryBoardViewModel : CommonViewModel
    {
        public Int16 AdvisioryBoradKey { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        public Int16 DisplayOrder { get; set; }
        [DisplayName("Designation")]
        public string Designation { get; set; }
        [DisplayName("Affiliation")]
        public string Affiliation { get; set; }
        [DisplayName("Country")]
        public Int16 CountryKey { get; set; }
        public List<Master_Country> Countries { get; set; }
        public List<SelectListItem> DisplayOrders { get; set; }
        public string CountryName { get; set; }

    }
}