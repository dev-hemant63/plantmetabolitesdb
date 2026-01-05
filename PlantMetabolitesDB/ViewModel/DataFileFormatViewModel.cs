using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantMetabolitesDB.ViewModel
{
    public class DataFileFormatViewModel
    {
        [DisplayName("Source Format")]
        public string DataFormatName { get; set; }
        public List<SelectListItem> DataFormats { get; set; }

        [DisplayName("Total No. Of Values")]
        public int TotalValue { get; set; }
        public List<SelectListItem> TotalValues { get; set; }

        [DisplayName("Browse File")]
        public string UploadedFileName { get; set; }
        public HttpPostedFileBase UploadedImageFile { get; set; }

    }
    public class DataInput
    {
        protected string _Title;
        protected string _Charge;
        protected Double _PreMass;
        protected int _totalValue;
        protected List<DataInputValues> _lstDataInputValues;

        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        public string Charge
        {
            get
            {
                return _Charge;
            }
            set
            {
                _Charge = value;
            }
        }

        public int TotalValue
        {
            get
            {
                return _totalValue;
            }
            set
            {
                _totalValue = value;
            }
        }

        public Double PreMass
        {
            get
            {
                return _PreMass;
            }
            set
            {
                _PreMass = value;
            }
        }

        public List<DataInputValues> lstDataInputValues
        {
            get
            {
                return _lstDataInputValues;
            }
            set
            {
                _lstDataInputValues = value;
            }
        }
    }

    public class DataInputValues
    {
        protected double _MZ;
        protected double _RI;
        protected double _RIpercent;

        public double MZ
        {
            get
            {
                return _MZ;
            }
            set
            {
                _MZ = value;
            }
        }

        public double RI
        {
            get
            {
                return _RI;
            }
            set
            {
                _RI = value;
            }
        }

        public double RIpercent
        {
            get
            {
                return _RIpercent;
            }
            set
            {
                _RIpercent = value;
            }
        }

    }

    public enum InputFormatType
    {
        NoFormat = 0,
        MGF = 1,
        Xcalibur = 2,
        TQD = 3,
        AminoAcid = 4,
        tms = 5
    }
}
