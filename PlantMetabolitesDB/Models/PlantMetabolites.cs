using PlantMetabolitesDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TandemDB.Models
{
    public class PlantMetabolites
    {
        [Key]
        public int PlantMetabolitesKey { get; set; }
        public string PlantSpeciesName { get; set; }
        public string TaxonomistName { get; set; }
        public Int16 PlantFamilyKey { get; set; }
        public string DataSheetFile { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string CompiledBy { get; set; }
        public string Synonyms { get; set; }
        public string Figure { get; set; }


        public virtual Master_Database Master_Databases { get; set; }
        public virtual ICollection<M02_VernacularName> M02_VernacularName { get; set; }
        public virtual ICollection<M03_BiologicalActivity> M03_BiologicalActivity { get; set; }
        public virtual ICollection<M04_Distribution> M04_Distribution { get; set; }
        public virtual ICollection<M05_EthnobotanicalInfo> M05_EthnobotanicalInfo { get; set; }
        public virtual ICollection<M06_CompuondClass> M06_CompuondClass { get; set; }
        public virtual ICollection<M07_Constituents> M07_Constituents { get; set; }
        public virtual ICollection<Master_MS1MassSpectra> Master_MS1MassSpectra { get; set; }
    }
}