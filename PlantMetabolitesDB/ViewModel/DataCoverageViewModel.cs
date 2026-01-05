using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class DataCoverageViewModel
    {
        public SampleDataViewModel SampleData { get; set; }
        public StandardDataViewModel StandardData { get; set; }
        public List<MassSpectraDataValuesSearchResult> MassSpectraResultData { get; set; }

    }

    public class SampleDataViewModel
    {
        public Int16 CompoundKey { get; set; }
        public string CompoundName { get; set; }
        public string CASNo { get; set; }
        public string Formula { get; set; }
        public string MolecularWeight_Input { get; set; }
        public string MassSpectrum { get; set; }
        public Int16 Polarity { get; set; }

        public string Title { get; set; }
        public string Charge { get; set; }
        public string Premass { get; set; }
    }


    public class StandardDataViewModel {
        public Int16 CompoundKey { get; set; }
        public string DatabaseName { get; set; }
        public string CompoundName { get; set; }
        public string InstrumentName { get; set; }
        public string CASNo { get; set; }
        public string Formula { get; set; }
        public string MolecularWeight_Input { get; set; }
        public string MassSpectrum { get; set; }
        public string Query { get; set; }
        public Int16 Polarity { get; set; }
        public string MassSpectrumDescription { get; set; }
        public string Title { get; set; }
        public string Charge { get; set; }
        public string Premass { get; set; }

    }
}