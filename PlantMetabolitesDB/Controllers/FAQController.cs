using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.Models;

namespace PlantMetabolitesDB.Controllers
{
    public class FAQController : Controller
    {
        private IFAQRepository _faqRepository;
        public FAQController()
        {
            _faqRepository = new FAQRepository(new PlantMetabolitesDBContext());
        }
        public FAQController(IFAQRepository faqRepository)
        {
            _faqRepository = faqRepository;
        }
        // GET: FAQ
        public ActionResult Index()
        {
            var model = _faqRepository.GetAll().Where(d => d.IsActive == true).OrderByDescending(d => d.FAQKey).ToList();
          

            return View(model);
        }

        
    }
}