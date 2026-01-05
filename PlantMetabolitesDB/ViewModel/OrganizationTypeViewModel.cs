using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class OrganizationTypeViewModel : CommonViewModel
    {
        public Int16 OrganizationTypeKey { get; set; }
        [DisplayName("Organization Type Name")]
        public string OrganizationTypeName { get; set; }
        public bool IsAddByAdmin { get; set; }
    }
}