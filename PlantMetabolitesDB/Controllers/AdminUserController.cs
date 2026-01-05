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
    [Authorize(Roles = "Superadmin")]
    public class AdminUserController : Controller
    {
        private IUsersRepository _usersRepository;
        private ICommonRepository _commonRepository;
        private IOrganizationTypeRepository _organizationTypeRepository;
        GeneralClass gls = new GeneralClass();
        public AdminUserController()
        {
            _usersRepository = new UsersRepository(new PlantMetabolitesDBContext());
            _commonRepository = new CommonRepository(new PlantMetabolitesDBContext());
            _organizationTypeRepository = new OrganizationTypeRepository(new PlantMetabolitesDBContext());
        }
        public AdminUserController(IUsersRepository usersRepository, ICommonRepository commonRepository, IOrganizationTypeRepository organizationTypeRepository)
        {
            _usersRepository = usersRepository;
            _commonRepository = commonRepository;
            _organizationTypeRepository = organizationTypeRepository;
        }
        // GET: AdminUser
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAdminUsers()
        {
            var model = _usersRepository.GetAllByUserType(2);
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddAdminUsers()
        {
            UsersViewModel model = new UsersViewModel();
            model.Countries = _commonRepository.GetAllCountry().ToList();
            model.OrganizationTypes = _organizationTypeRepository.GetAll().ToList();
            model.UserTypes = gls.GetUserType();
            model.UserType = 2;

            return PartialView("_AddAdminUsers", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdminUsers(UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var keySalt = GeneralClass.GeneratePassword(10);
                var password = GeneralClass.EncodePassword(model.Password, keySalt);
                model.Password = model.Password;
                model.HashPassword = password;
                model.SaltPassword = keySalt;

                model.CreatedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                model.UserType = 2;
                string verificationcode = Guid.NewGuid().ToString();
                model.VerificationCode = verificationcode;

                int result = _usersRepository.Add(model);
                if (result > 0)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Failed"] = "Failed";
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditAdminUsers(int UserKey)
        {
            UsersViewModel model = _usersRepository.GetByKey(UserKey);
            model.Countries = _commonRepository.GetAllCountry().ToList();
            model.OrganizationTypes = _organizationTypeRepository.GetAll().ToList();
            model.UserTypes = gls.GetUserType();

            return PartialView("_EditAdminUsers", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdminUsers(UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                int result = _usersRepository.Update(model);
                if (result > 0)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult DeleteAdminUser(int UserKey)
        {
            var model = _usersRepository.GetByKey(UserKey);
            if (model != null)
            {
                _usersRepository.Delete(model.UserKey);
            }
            return Json("deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ActivateDeActivate(int UserKey)
        {
            try
            {
                _usersRepository.ActivateDeActivate(UserKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult ResetPassword(int UserKey)
        {
            try
            {
                _usersRepository.ResetPassword(UserKey);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

    }
}