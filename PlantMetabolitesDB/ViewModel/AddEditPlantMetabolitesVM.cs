using PlantMetabolitesDB.Models;
using System.Collections.Generic;
using System.Windows.Documents;
using TandemDB.Models;

namespace TandemDB.ViewModel
{
    public class AddEditPlantMetabolitesVM
    {
        public PlantMetabolitesVM FormData { get; set; }
        public IEnumerable<Master_Database> Database { get; set; }
    }
}