using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class MassSpectraDataValuesViewModel
    {
        public Int16 MassSpectraKey { get; set; }
        public string mz { get; set; }
        public string relative { get; set; }
        public int mz_int { get; set; }
        public int relative_int { get; set; }

    }


    public class MassSpectraDataValuesSearchResult
    {
        public int input_mz_int { get; set; }
        public string input_relative { get; set; }
        public int library_mz_int { get; set; }
        public string library_relative { get; set; }
        public string ClassName { get; set; }

    }


    public class GCDataInputValues
    {
        protected double _MZ;
        protected double _RI;

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


    }

    public class GCDataInput
    {
        protected List<GCDataInputValues> _lstDataInputValues;
        protected List<GCDataInputValues1> _lstDataInputValues1;

        public List<GCDataInputValues> lstDataInputValues
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
        public List<GCDataInputValues1> lstDataInputValues1
        {
            get
            {
                return _lstDataInputValues1;
            }
            set
            {
                _lstDataInputValues1 = value;
            }
        }
    }

    public class GCDataInputValues1
    {
        protected double _MZ1;
        protected double _RI1;

        public double MZ1
        {
            get
            {
                return _MZ1;
            }
            set
            {
                _MZ1 = value;
            }
        }

        public double RI1
        {
            get
            {
                return _RI1;
            }
            set
            {
                _RI1 = value;
            }
        }

    }
}