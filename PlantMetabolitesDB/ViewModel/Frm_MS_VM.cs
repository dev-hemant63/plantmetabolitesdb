using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;
using System.Collections.Generic;
using TandemDB.Models;

namespace TandemDB.ViewModel
{
    public class Frm_MS_VM
    {
        public int plantMetabolitesKey { get; set; }
        public IEnumerable<InstrumentViewModel> InstrumentList { get; set; }
        public PlantMetabolitesVM PlantMetabolitesDetails { get; set; }
        public MasterMS1MassSpectraVM MasterMS1MassSpectra { get; set; }
    }
}