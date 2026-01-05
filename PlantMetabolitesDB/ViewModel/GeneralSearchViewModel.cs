using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class GeneralSearchViewModel
    {
        [DisplayName("Name")]
        public string CompoundName { get; set; }
        public string CASNo { get; set; }

        [DisplayName("Name of plant species")]
        public string NameOfPlantSpecies { get; set; }

        [DisplayName("Family of plant species")]
        public string FamilyOfPlantSpecies { get; set; }

        [DisplayName("Specific biological activity")]
        public string SpecificBiologicalActivity { get; set; }

        [DisplayName("Molecular Weight")]
        public string MolecularWeight { get; set; }
        [DisplayName("Chemical Formula")]
        public string Formula { get; set; }
        public int SearchType { get; set; }
        public List<GeneralSearchResultViewModel> GeneralSearchResults { get; set; }
    }

    public class GeneralSearchResultViewModel   
    {
        public int CompoundKey { get; set; }
        public string CompoundName { get; set; }
        public string Formula { get; set; }
        public string MolecularWeight { get; set; }


        public string NameOfPlantSpecies { get; set; }
        public string FamilyOfPlantSpecies { get; set; }
        public string Metabolites { get; set; }
        public string MSFP { get; set; }

    }
}