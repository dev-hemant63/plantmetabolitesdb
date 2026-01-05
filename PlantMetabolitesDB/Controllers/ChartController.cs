using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    public class ChartController : Controller
    {
        private IMassSpectraDataValuesRepository _massSpectraDataValuesRepository;
        public ChartController()
        {
            _massSpectraDataValuesRepository = new MassSpectraDataValuesRepository(new PlantMetabolitesDBContext());
        }

        public ChartController(IMassSpectraDataValuesRepository massSpectraDataValuesRepository)
        {
           
            _massSpectraDataValuesRepository = massSpectraDataValuesRepository;

        }
        // GET: Chart
        public ActionResult Index(int sk, int st)
        {
            string strData = string.Empty;

            var chartdata = _massSpectraDataValuesRepository.GetMassSpectraDataValuesByKey(sk, st);

            if (chartdata.Count() > 0)
            {
                foreach (var it in chartdata)
                {
                    strData += ("[" + it.mz + ", " + it.relative + "],");
                }
                strData = strData.Substring(0, strData.Length - 1);
                //strData = "[" + strData.Trim() + "];";
                strData = strData.Trim();
            }
            else
                strData = "[];";

            ViewBag.ChartData = strData;
            return View();
        }
    }
}