using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;
using PlantMetabolitesDB.ViewModel;
using BotDetect.Web.Mvc;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;

namespace PlantMetabolitesDB.Controllers
{
    public class AccountController : Controller
    {
        private IUsersRepository _usersRepository;
        private ICommonRepository _commonRepository;
        private IOrganizationTypeRepository _organizationTypeRepository;
        GeneralClass gls = new GeneralClass();
        public AccountController()
        {
            _usersRepository = new UsersRepository(new PlantMetabolitesDBContext());
            _commonRepository = new CommonRepository(new PlantMetabolitesDBContext());
            _organizationTypeRepository = new OrganizationTypeRepository(new PlantMetabolitesDBContext());

        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaValidationActionFilter("CaptchaCode", "ExampleCaptcha", "Wrong Captcha!")]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // TODO: Captcha validation failed, show error message
                    ViewBag.Message = "Invalid captcha";
                    MvcCaptcha.ResetCaptcha("ExampleCaptcha");
                    return View();
                }
                else
                {
                    // TODO: captcha validation succeeded; execute the protected action
                    using (PlantMetabolitesDBContext context = new PlantMetabolitesDBContext())
                    {
                        var get_user = context.Master_Users.Where(u => u.Username.ToLower() == model.UserName.ToLower()).FirstOrDefault();
                        var hashCode = get_user.SaltPassword;
                        if (get_user != null)
                        {
                            var encodingPasswordString = GeneralClass.EncodePassword(model.UserPassword, hashCode);
                            if (encodingPasswordString == get_user.HashPassword)
                            {
                                if (get_user.IsEmailVerified)
                                {
                                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                                    GeneralClass.SetCookie(System.Web.HttpContext.Current, "UserKey", get_user.UserKey.ToString());

                                    if (get_user.UserType == 3)
                                    {
                                        return RedirectToAction("Index", "Home");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Dashboard");
                                    }
                                }
                                else
                                {
                                    ViewBag.Message = "Email address not verified. First verify your email";
                                    ModelState.AddModelError("", "Email address not verified. First verify your email");
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Invalid Username or Password";
                                ModelState.AddModelError("", "invalid Username or Password");
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Invalid Username or Password";
                            ModelState.AddModelError("", "invalid Username or Password");
                        }

                        return View();
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Invalid Username or Password";
                return View();
            }

        }

        [HttpGet]
        public ActionResult Register()
        {
            var model = new UsersViewModel();
            model.Countries = _commonRepository.GetAllCountry().ToList();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult Register(UsersViewModel model)
        {
            model.Countries = _commonRepository.GetAllCountry().ToList();

            if (ModelState.IsValid)
            {
                var IsExists = _usersRepository.IsEmailExists(model.Username);
                if (IsExists)
                {
                    var data = new { Result = "UserExists", EmailId = model.Username };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var keySalt = GeneralClass.GeneratePassword(10);
                    var password = GeneralClass.EncodePassword(model.Password, keySalt);
                    model.Password = model.Password;
                    model.HashPassword = password;
                    model.SaltPassword = keySalt;

                    model.IsEmailVerified = false;
                    string verificationcode = Guid.NewGuid().ToString();
                    model.VerificationCode = verificationcode;
                    model.UserType = 3;
                    model.CreatedBy = Convert.ToInt16(Session["UserKey"]);
                    var GenarateUserVerificationLink = "/Account/UserVerification/" + verificationcode;
                    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

                    int result = _usersRepository.Add(model);
                    if (result > 0)
                    {
                        //send email to user
                        gls.SendEmailToUser(model.Username, link);
                        var data = new { Result = "Success", EmailId = model.Username };
                        return Json(data, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        var data = new { Result = "Failed", EmailId = model.Username };
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                var data = new { Result = "Failed", EmailId = model.Username };
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        // [ValidateAntiForgeryToken]
        public ActionResult UserVerification(string id)
        {
            var IsVerify = _usersRepository.UserVerification(id);
            if (IsVerify)
            {
                ViewBag.Message = "Email Verification completed";
            }
            else
            {
                ViewBag.Message = "Invalid Request...Email not verify";
                ViewBag.Status = false;
            }

            return View("UserVerification");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var IsExists = _usersRepository.IsEmailExists(model.EmailId);
            if (!IsExists)
            {
                ModelState.AddModelError("EmailNotExists", "This email is not exists");
                return View();
            }
            var objUser = _usersRepository.GetUserByEmaidId(model.EmailId);

            //send email to user
            string mailsubject = "Recover your password";
            string mailbody = "Hello " + objUser.FullName + "," +
                "<br/><br/><b> Please find the password:  </b> " + objUser.Password;

            gls.SendEmail(objUser.Username, mailsubject, mailbody);
            ModelState.AddModelError("PasswordSend", "Your password sent to your registered email address.");
            return View();
        }

        [Authorize(Roles = "Superadmin,Admin,User")]
        public ActionResult Logout()
        {
            Response.ExpiresAbsolute = DateTime.UtcNow.AddDays(-1d);
            Response.Expires = -1500;
            Response.CacheControl = "no-cache";
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }



        [HttpGet]
        [Authorize(Roles = "User,Superadmin,Admin")]
        public ActionResult UserProfile()
        {
            var userkey = GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey");
            var model = _usersRepository.GetByKey(Convert.ToInt32(userkey));

            model.Countries = _commonRepository.GetAllCountry().ToList();
            model.OrganizationTypes = _organizationTypeRepository.GetAll().ToList();
            model.UserType = 3;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "User,Superadmin,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.LastModifiedBy = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
                model.UserType = 3;
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

        [Authorize(Roles = "Superadmin,Admin")]
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel();
            return View(model);
        }

        
        [HttpPost]
        [Authorize(Roles = "Superadmin,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            Int16 userkey = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
            try
            {
                using (PlantMetabolitesDBContext context = new PlantMetabolitesDBContext())
                {
                    var get_user = context.Master_Users.Where(u => u.UserKey == userkey).FirstOrDefault();
                    var hashCode = get_user.SaltPassword;
                    if (get_user != null)
                    {
                        var encodingOldPasswordString = GeneralClass.EncodePassword(model.OldPassword, hashCode);
                        if (encodingOldPasswordString == get_user.HashPassword)
                        {
                            var newpassword = GeneralClass.EncodePassword(model.NewPassword, get_user.SaltPassword);
                            model.HashPassword = newpassword;
                            model.UserKey = userkey;
                            int kk = _usersRepository.ChangePassword(model);
                            if (kk > 0)
                            {
                                return Json("Success", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("Failed", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json("InvalidPassword", JsonRequestBehavior.AllowGet);
                        }
                    }
                }

            }
            catch (Exception)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }

            return View(model);
        }


        [Authorize(Roles = "User")]
        public ActionResult ChangeUserPassword()
        {
            var model = new ChangePasswordViewModel();
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUserPassword(ChangePasswordViewModel model)
        {
            Int16 userkey = Convert.ToInt16(GeneralClass.GetCookie(System.Web.HttpContext.Current, "UserKey"));
            try
            {
                using (PlantMetabolitesDBContext context = new PlantMetabolitesDBContext())
                {
                    var get_user = context.Master_Users.Where(u => u.UserKey == userkey).FirstOrDefault();
                    var hashCode = get_user.SaltPassword;
                    if (get_user != null)
                    {
                        var encodingOldPasswordString = GeneralClass.EncodePassword(model.OldPassword, hashCode);
                        if (encodingOldPasswordString == get_user.HashPassword)
                        {
                            var newpassword = GeneralClass.EncodePassword(model.NewPassword, get_user.SaltPassword);
                            model.HashPassword = newpassword;
                            model.UserKey = userkey;
                            int kk = _usersRepository.ChangePassword(model);
                            if (kk > 0)
                            {
                                return Json("Success", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("Failed", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json("InvalidPassword", JsonRequestBehavior.AllowGet);
                        }
                    }
                }

            }
            catch (Exception)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }

            return View(model);
        }
    }
}