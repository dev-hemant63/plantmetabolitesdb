using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class AductIonMassViewModel
    {
        [DisplayName("Enter M.W.")]
        public double? MolecularWeight { get; set; }
        public List<AductIonMassPositiveViewModel> AductIonMassPositives { get; set; }
        public List<AductIonMassNegativeViewModel> AductIonMassNegatives { get; set; }
    }

    public class AductIonMassPositiveViewModel
    {
        public string IonName { get; set; }
        public string IonMass { get; set; }
        public string Charge { get; set; }
        public double AductIonMass { get; set; }
        public double PositiveMass { get; set; }


    }

    public class AductIonMassNegativeViewModel
    {
        public string IonName { get; set; }
        public string IonMass { get; set; }
        public string Charge { get; set; }
        public double AductIonMass { get; set; }
        public double NegativeMass { get; set; }


    }
}