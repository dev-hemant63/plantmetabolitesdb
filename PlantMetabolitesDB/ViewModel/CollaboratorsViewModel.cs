using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;

namespace PlantMetabolitesDB.ViewModel
{
    public class CollaboratorsViewModel: CommonViewModel
    {
        public Int16 CollaboratorsKey { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Email Id")]
        public string EmailId { get; set; }
        [DisplayName("Type")]
        public Int16 UserType { get; set; }
        [DisplayName("Designation")]
        public string Designation { get; set; }
        [DisplayName("Affiliation")]
        public string Affiliation { get; set; }
        [DisplayName("Phone")]
        public string ContactNo { get; set; }
        [DisplayName("Country")]
        public Int16 CountryKey { get; set; }
        public List<Master_Country> Countries { get; set; }
        public string CountryName { get; set; }
        public string CollaboratorTypeName { get; set; }
        public List<SelectListItem> CollaboratorsTypes { get; set; }

    }
}