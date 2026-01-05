using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;

namespace PlantMetabolitesDB.ViewModel
{
    public class CompoundViewModel : CommonViewModel
    {
        [Key]
        public Int16 CompoundKey { get; set; }
        [DisplayName("Compound Name")]
        public string CompoundName { get; set; }
        [DisplayName("CAS No")]
        public string CASNo { get; set; }
        [DisplayName("Formula")]
        public string Formula { get; set; }
        [DisplayName("Molecular Weight (Monoisotopic Mass)")]
        public string MolecularWeight_Input { get; set; }
        public Int16 MolecularWeight { get; set; }
        [DisplayName("Canonical Smiles")]
        public string Smiles { get; set; }
        [DisplayName("IUPAC Name")]
        public string IUPACName { get; set; }
        [DisplayName("Other Names")]
        public string OtherNames { get; set; } 
        public string MSnITFragmentationFile { get; set; }
        public string SchemeofFragmentationFile { get; set; }
        public string DataSheetFile { get; set; }
        [DisplayName("Compiled By")]
        public string CompiledBy { get; set; }
        [DisplayName("Database Name")]
        public Int16 DatabaseKey { get; set; }
        public string DatabaseName { get; set; }
        public List<DatabaseViewModel> Databases { get; set; }
        [DisplayName("Chemical Structure File")]
        public string ChemicalStructureFile { get; set; }
        public HttpPostedFileBase ChemicalStructureImageFile { get; set; }
        public int MS2Count { get; set; }
        public int MS3Count { get; set; }
        public int AductCount { get; set; }

    }

    public class CompoundPreviewViewModel 
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
        public string MSnITFragmentationFile { get; set; }
        public string SchemeofFragmentationFile { get; set; }
        public string DataSheetFile { get; set; }
        public string CompiledBy { get; set; }
        public Int16 DatabaseKey { get; set; }
        public string DatabaseName { get; set; }
        public string ChemicalStructureFile { get; set; }
        public List<SelectListItem> MS2List { get; set; }
        public List<SelectListItem> MS3List { get; set; }
        public List<SelectListItem> AductList { get; set; }
        public List<MSMajorNaturalLoses> MSMajorNaturalLoses { get; set; }

    }

    public class MSMajorNaturalLoses
    {
        public int Product { get; set; }
        public double RelativeIntensity { get; set; }
        public int NaturalLoss { get; set; }    

    }


    public class MassSpectraViewModel
    {
        public Int16 CompoundKey { get; set; }
        public string CompoundName { get; set; }
        public string CASNo { get; set; }
        public string Formula { get; set; }
        public string MolecularWeight_Input { get; set; }
        public string MassSpectrum { get; set; }
        public string Instrument { get; set; }
        public string MassChartLabels { get; set; }
        public string MassChartData { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

    }


}