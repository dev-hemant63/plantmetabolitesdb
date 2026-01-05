using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    public class AductIonCalculatorController : Controller
    {
        // GET: AductIonCalculator
        public ActionResult Index()
        {
            XmlDocument doc = new XmlDocument();
            AductIonMassViewModel model = new AductIonMassViewModel();

            List<AductIonMassPositiveViewModel> aductpositives = new List<AductIonMassPositiveViewModel>();
            doc.Load(Server.MapPath("~/Models/AductIonCalcforXML.xml"));
            foreach (XmlNode node in doc.SelectNodes("/AdductIonMass/Ion"))
            {
                aductpositives.Add(new AductIonMassPositiveViewModel
                {
                    IonName = node["IonName"].InnerText,
                    IonMass = node["IonMass"].InnerText,
                    Charge = node["Charge"].InnerText,
                    AductIonMass = Convert.ToDouble(node["Mult"].InnerText),
                    PositiveMass = Convert.ToDouble(node["Mass"].InnerText)
                });
            }
            model.AductIonMassPositives = aductpositives;

            List<AductIonMassNegativeViewModel> aductnegatives = new List<AductIonMassNegativeViewModel>();
            doc.Load(Server.MapPath("~/Models/AductIonCalcforXML_Negative.xml"));
            foreach (XmlNode node in doc.SelectNodes("/AdductIonMass/Ion"))
            {
                aductnegatives.Add(new AductIonMassNegativeViewModel
                {
                    IonName = node["IonName"].InnerText,
                    IonMass = node["IonMass"].InnerText,
                    Charge = node["Charge"].InnerText,
                    AductIonMass = Convert.ToDouble(node["Mult"].InnerText),
                    NegativeMass = Convert.ToDouble(node["Mass"].InnerText)
                });
            }
            model.AductIonMassNegatives= aductnegatives;

            return View(model);
        }
    }
}