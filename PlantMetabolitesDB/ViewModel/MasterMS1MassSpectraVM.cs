using System.Collections.Generic;
using System.Web;
using System.Windows.Documents;

namespace TandemDB.ViewModel
{
    public class MasterMS1MassSpectraVM
    {
        public int MS1MassSpectraKey { get; set; }

        public int PlantMetabolitesKey { get; set; }

        public int InstrumentKey { get; set; }

        public int Polarity { get; set; }

        public int AnnotationKey { get; set; }

        public string PartsOfPlant { get; set; }

        public string SampleType { get; set; }

        public string TaxonomistName { get; set; }

        public string VoucherNo { get; set; }

        public string HerbariumDepositedAt { get; set; }

        public string DateOfCollection { get; set; }

        public string GeoLocation { get; set; }

        public string SpectrumAveraging { get; set; }
        public string HPLCProfile { get; set; }
        public string LCMSProfile { get; set; }

        public HttpPostedFileBase HPLCUPLCMethodeFile { get; set; }
        public string HPLCUPLCMethodeFilePath { get; set; }

        public HttpPostedFileBase ExtractionMethodeFile { get; set; }
        public string ExtractionMethodeFilePath { get; set; }

        public HttpPostedFileBase FingerprintFile { get; set; }
        public string FingerprintFilePath { get; set; }

        public HttpPostedFileBase MS1RefFile { get; set; }
        public string MS1RefFilePath { get; set; }
        public List<MultipleFileList> RawFiles { get; set; }
        public List<MultipleFileList> LCMSProfiles { get; set; }
        public List<MultipleFileList> HPLCProfiles { get; set; }
    }

    public class MultipleFileList
    {
        public HttpPostedFileBase File { get; set; }
        public string FilePath { get; set; }
        public string Remark { get; set; }
    }
}