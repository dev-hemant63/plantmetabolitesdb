using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantMetabolitesDB.ViewModel
{
    public class TickerViewModel : CommonViewModel
    {
        public Int16 TickerKey { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("URL")]
        public string URL { get; set; }
        [DisplayName("Message Type")]
        public Int16 MessageType { get; set; }
        public List<SelectListItem> MessageTypes { get; set; }
        public string MessageTypeName { get; set; }
        [DisplayName("Attachment (only allowed txt or pdf file and the file size upto 50 KB")]
        public string UploadedFileName { get; set; }
        public HttpPostedFileBase UploadedImageFile { get; set; }
    }
}