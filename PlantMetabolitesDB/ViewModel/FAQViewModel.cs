using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class FAQViewModel: CommonViewModel
    {
        public Int16 FAQKey { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Attachment (only allowed txt or pdf file and the file size upto 50 KB")]
        public string UploadedFileName { get; set; }
        public HttpPostedFileBase UploadedImageFile { get; set; }
        public String fileExtension { get; set; }
    }
}