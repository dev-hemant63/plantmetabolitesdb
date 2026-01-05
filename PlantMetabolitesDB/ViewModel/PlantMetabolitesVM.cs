using PlantMetabolitesDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TandemDB.Models;

namespace TandemDB.ViewModel
{
    public class PlantMetabolitesVM
    {
        public int PlantMetabolitesKey { get; set; }
        [Required (ErrorMessage = "Plant species name required")]
        public string PlantSpeciesName { get; set; }
        [Required(ErrorMessage = "Taxonomist name required")]
        public string TaxonomistName { get; set; }
        [Required(ErrorMessage = "Plant family required")]
        public string PlantFamilyKey { get; set; }
        public string DataSheetFile { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Compiled by required")]
        public string CompiledBy { get; set; }
        [Required(ErrorMessage = "Synonyms required")]
        public string Synonyms { get; set; }
        [Required(ErrorMessage = "Vernacular name required")]
        public string VernacularName { get; set; }
        [Required(ErrorMessage = "Specific biological activity required")]
        public string SpecificBiologicalActivity { get; set; }
        public string Distribution { get; set; }
        public string EthnobotanicalInformation { get; set; }
        public string ClassOfCompounds { get; set; }
        public string MajorConstituents { get; set; }
        public string Figure { get; set; }

        public string PlantFamilyName { get; set; }

        public IEnumerable<Master_Database> Database { get; set; }
    }
}