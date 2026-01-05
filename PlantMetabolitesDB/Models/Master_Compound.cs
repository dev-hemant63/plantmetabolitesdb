using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.Models
{
    [Table("Master_Compound")]
    public class Master_Compound
    {
        [Key]
        public Int16 CompoundKey { get; set; }
        public string CompoundName { get; set; }
        public string CASNo { get; set; }
        public string Formula { get; set; }
        public string MolecularWeight_Input { get; set; }
        public Int16 MolecularWeight { get; set; }
        public string Smiles { get; set; }
        public string IUPACName { get; set; }
        public string OtherNames { get; set; }
        public string ChemicalStructureFile { get; set; }
        public string MSnITFragmentationFile { get; set; }
        public string SchemeofFragmentationFile { get; set; }
        public string DataSheetFile { get; set; }
        public string CompiledBy { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }      
        public Int16 DatabaseKey { get; set; }
        [ForeignKey("DatabaseKey")]
        public virtual Master_Database Databases { get; set; }
       
    }
}