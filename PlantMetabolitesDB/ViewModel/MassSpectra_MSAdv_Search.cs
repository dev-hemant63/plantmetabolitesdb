using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class MassSpectra_MSAdv_Search
    {
        public String Query { get; set; }
        public String Data_QM { get; set; }
        public String Data_MW { get; set; }
        public String Data_Name { get; set; }
        public String Data_Formula { get; set; }
        public String Data_MF { get; set; }
        public String Data_MHit { get; set; }
        public String Data_CompoundKey { get; set; }
        public Int32 Data_mz_DataCoverage { get; set; }
        public Int32 Data_RI_DataCoverage { get; set; }
        public Double Data_mz_MatchFactor { get; set; }
        public Double Data_RI_MatchFactor { get; set; }

        public Int16 Input_InstrumentKey { get; set; }
        public Int16 Input_DataBaseKey { get; set; }
        public Int16 Input_Polarity { get; set; }
        public String Input_ParentIon { get; set; }
        public Int16 Input_AnnotationKey { get; set; }
        public Int16 Input_AnnotationType { get; set; }
        public Int16 Input_IncludeMS3PrecursorIonKey { get; set; }
        public Int16 IncludeMS3MassSpectraKey { get; set; }
        public Int16 IncludeMS2MassSpectraKey { get; set; }
        public Int16 Input_Tolerance { get; set; }
        public Int16 Input_ReportTop { get; set; }
        public Int16 matchedAnnotationType { get; set; }
        public Int16 matchedMassSpectraKey { get; set; }
        public Int32 matchedInputIndex { get; set; }

        public List<MassSpectra_MSAdv_Search_InputValues> lstMassSpectra_MSAdv_Search_InputValues { get; set; }
    }

    public class MassSpectra_MSAdv_Search_InputValues
    {
        public Int32 Input_mz_int { get; set; }
        public Int32 Input_absolute_int { get; set; }
        public Int32 Input_relative_int { get; set; }
        public string mznew { get; set; }
        public Int32 Data_mz_int { get; set; }
        public Int32 Data_relative_int { get; set; }
    }

}