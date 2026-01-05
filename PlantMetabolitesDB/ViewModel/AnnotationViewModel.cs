using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantMetabolitesDB.ViewModel
{
    public class AnnotationViewModel : CommonViewModel
    {
        public Int16 AnnotationKey { get; set; }
        [DisplayName("Annotation Name")]
        public string AnnotationName { get; set; }
        [DisplayName("Polarity")]
        public byte Polarity { get; set; }
        [DisplayName("Annotation Type")]
        public byte AnnotationType { get; set; }
        public List<SelectListItem> Polarities { get; set; }
        public List<SelectListItem> AnnotationTypes { get; set; }
        public string PolarityName { get; set; }
        public string AnnotationTypeName { get; set; }

    }

    //public class Polarity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    //public class AnnotationType
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}