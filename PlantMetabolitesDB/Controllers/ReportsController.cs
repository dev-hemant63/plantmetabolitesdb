using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Controllers
{
    public class ReportsController : Controller
    {
        private IUsersRepository _usersRepository;
        GeneralClass gls = new GeneralClass();
        public ReportsController()
        {
            _usersRepository = new UsersRepository(new PlantMetabolitesDBContext());
        }
        public ReportsController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserReport()
        {
            UserReportViewModel model = new UserReportViewModel();
            model.UserTypes = gls.GetUserType().Where(d => d.Value != "1").ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult GetUserReport(int userType)
        {
            var users = _usersRepository.GetAllByUserType(userType).ToList();
            return PartialView("_UserResultPartialPage", users);
        }

        [HttpGet]
        public FileContentResult ExportToExcel(int usertype)
        {
           // int userTypeforexport = Convert.ToInt32(Request.Form["hidUserType"]);

            var users = _usersRepository.GetAllByUserType(usertype).ToList();
            List<UserExportViewModel> usersExport = new List<UserExportViewModel>();
            foreach (var item in users)
            {
                usersExport.Add(new UserExportViewModel
                {
                    Code = item.Code,
                    Username = item.Username,
                    Fullname = item.FullName,
                    Address = item.Address,
                    Phone = item.Phone,
                    Mobile = item.Mobile,
                    Country = item.CountryName,
                    Organization = item.OrganizationTypeName,
                    Affiliation = item.Affiliation,
                    Status = item.StatusName,
                    RegistrationDate = item.CreatedOn.Value.ToShortDateString(),

                });
            }
            string strHeading = usertype == 3 ? "Registered User Report" : "Admin User Reports";
            string strFileName= usertype == 3 ? "RegisteredUserReport.xlsx" : "AdminUserReports.xlsx";

            string[] columns = { "Code", "Username", "Fullname", "Address", "Phone", "Mobile", "Country" ,"Organization","Affiliation","Status","RegistrationDate"};
            byte[] filecontent = ExcelExportHelper.ExportExcel(usersExport, strHeading, true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, strFileName);

        }
    }
}