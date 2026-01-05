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
    [Authorize(Roles = "Superadmin,Admin")]
    public class DashboardController : Controller
    {
        private ICommonRepository _commonRepository;
        private IUsersRepository _usersRepository;
        public DashboardController()
        {
            _commonRepository = new CommonRepository(new PlantMetabolitesDBContext());
            _usersRepository = new UsersRepository(new PlantMetabolitesDBContext());

        }
        public DashboardController(ICommonRepository commonRepository, IUsersRepository usersRepository)
        {
            _commonRepository = commonRepository;
            _usersRepository = usersRepository;

        }

        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetCompundStats()
        {
            CompoundStats model = _commonRepository.CompoundStats();
            return PartialView("~/Views/Compound/_CompoundStats.cshtml", model);
        }

        public PartialViewResult GetAminUsers()
        {
            var model = _usersRepository.GetAllByUserType(2).OrderBy(z => z.FullName).Take(10).ToList();
            return PartialView("_AdminUserPartialView", model);
        }

        public PartialViewResult GetCountryWiseRegisteredUsers()
        {
            var usercount = _usersRepository.GetAllByUserType(3)
                          .GroupBy(a => a.CountryName)
                          .Select(g => new { g.Key, Count = g.Count() }).ToList();

            List<CountryUserCountViewModel> lstUserCount = new List<CountryUserCountViewModel>();
            var RegisteredUserCount = 0;
            foreach (var item in usercount)
            {
                RegisteredUserCount = RegisteredUserCount == 0 ? item.Count : RegisteredUserCount + item.Count;
                lstUserCount.Add(new CountryUserCountViewModel { CountryName = item.Key, UserCount = item.Count });
            }
            ViewBag.RegisteredUserCount = RegisteredUserCount;

            return PartialView("_RegisteredUsersPartialView", lstUserCount.OrderByDescending(d=>d.UserCount).Take(5));
        }
    }
}