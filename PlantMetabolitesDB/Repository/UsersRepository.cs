using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        public UsersRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<UsersViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_Users
                             select new UsersViewModel
                             {
                                 UserKey = dt.UserKey,
                                 Username = dt.Username,
                                 UserType = dt.UserType,
                                 FName = dt.FName,
                                 LName = dt.LName,
                                 FullName = dt.FullName,
                                 Address = dt.Address,
                                 Mobile = dt.Mobile,
                                 Phone = dt.Phone,
                                 Password = dt.Password,
                                 IsEmailVerified = dt.IsEmailVerified,
                                 VerificationCode = dt.VerificationCode,
                                 Affiliation = dt.Affiliation,
                                 OrganizationTypeKey = dt.OrganizationTypeKey.Value,
                                 CountryKey = dt.CountryKey,
                                 Code = dt.Code,
                                 CountryName = dt.Country.CountryName,
                                 OrganizationTypeName = dt.OrganizationType.OrganizationTypeName,
                                 cntLogin = dt.cntLogin,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();

            lstEntity.ForEach(y => y.UserTypeName = gls.GetUserType().Where(d => d.Value == y.UserType.ToString()).FirstOrDefault().Text);

            return lstEntity;
        }

        public IEnumerable<UsersViewModel> GetAllByUserType(int userType)
        {
            var lstEntity = (from dt in _context.Master_Users
                             where dt.UserType == userType
                             select new UsersViewModel
                             {
                                 UserKey = dt.UserKey,
                                 Username = dt.Username,
                                 UserType = dt.UserType,
                                 FName = dt.FName,
                                 LName = dt.LName,
                                 FullName = dt.FullName,
                                 Address = dt.Address,
                                 Mobile = dt.Mobile,
                                 Phone = dt.Phone,
                                 Password = dt.Password,
                                 IsEmailVerified = dt.IsEmailVerified,
                                 VerificationCode = dt.VerificationCode,
                                 Affiliation = dt.Affiliation,
                                 OrganizationTypeKey = dt.OrganizationTypeKey.Value,
                                 CountryKey = dt.CountryKey,
                                 Code = dt.Code,
                                 CountryName = dt.Country.CountryName,
                                 OrganizationTypeName = dt.OrganizationType.OrganizationTypeName,
                                 cntLogin = dt.cntLogin,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();

            lstEntity.ForEach(y => y.UserTypeName = gls.GetUserType().Where(d => d.Value == y.UserType.ToString()).FirstOrDefault().Text);

            return lstEntity;
        }
        public UsersViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_Users
                             where dt.UserKey == key
                             select new UsersViewModel
                             {
                                 UserKey = dt.UserKey,
                                 Username = dt.Username,
                                 UserType = dt.UserType,
                                 FName = dt.FName,
                                 LName = dt.LName,
                                 FullName = dt.FullName,
                                 Address = dt.Address,
                                 Mobile = dt.Mobile,
                                 Phone = dt.Phone,
                                 Password = dt.Password,
                                 IsEmailVerified = dt.IsEmailVerified,
                                 VerificationCode = dt.VerificationCode,
                                 Affiliation = dt.Affiliation,
                                 OrganizationTypeKey = dt.OrganizationTypeKey.Value,
                                 CountryKey = dt.CountryKey,
                                 Code = dt.Code,
                                 cntLogin = dt.cntLogin,
                                 CountryName = dt.Country.CountryName,
                                 OrganizationTypeName = dt.OrganizationType.OrganizationTypeName,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).SingleOrDefault();

            objEntity.UserTypeName = gls.GetUserType().Where(d => d.Value == objEntity.UserType.ToString()).FirstOrDefault().Text;

            return objEntity;
        }

        public UsersViewModel GetUserByEmaidId(string emailId)
        {
            var objEntity = (from dt in _context.Master_Users
                             where dt.Username == emailId
                             select new UsersViewModel
                             {
                                 UserKey = dt.UserKey,
                                 Username = dt.Username,
                                 UserType = dt.UserType,
                                 FName = dt.FName,
                                 LName = dt.LName,
                                 FullName = dt.FullName,
                                 Address = dt.Address,
                                 Mobile = dt.Mobile,
                                 Phone = dt.Phone,
                                 Password = dt.Password,
                                 IsEmailVerified = dt.IsEmailVerified,
                                 VerificationCode = dt.VerificationCode,
                                 Affiliation = dt.Affiliation,
                                 OrganizationTypeKey = dt.OrganizationTypeKey.Value,
                                 CountryKey = dt.CountryKey,
                                 Code = dt.Code,
                                 cntLogin = dt.cntLogin,
                                 CountryName = dt.Country.CountryName,
                                 OrganizationTypeName = dt.OrganizationType.OrganizationTypeName,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).SingleOrDefault();

            objEntity.UserTypeName = gls.GetUserType().Where(d => d.Value == objEntity.UserType.ToString()).FirstOrDefault().Text;

            return objEntity;
        }

        public int Add(UsersViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_User objEntity = new Master_User();
                objEntity.Username = entityVM.Username;
                objEntity.UserType = entityVM.UserType;
                objEntity.FName = entityVM.FName;
                objEntity.LName = entityVM.LName;
                objEntity.Code = GenerateCode(entityVM.UserType);
                objEntity.Mobile = entityVM.Mobile;
                objEntity.Phone = entityVM.Phone;
                objEntity.CountryKey = entityVM.CountryKey;
                objEntity.Address = entityVM.Address;
                objEntity.OrganizationTypeKey = entityVM.OrganizationTypeKey;
                objEntity.Affiliation = entityVM.Affiliation;
                objEntity.IsEmailVerified = false;
                objEntity.VerificationCode = entityVM.VerificationCode;
                objEntity.Password = entityVM.Password;
                objEntity.cntLogin = 0;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;
                objEntity.HashPassword = entityVM.HashPassword;
                objEntity.SaltPassword = entityVM.SaltPassword;

                _context.Master_Users.Add(objEntity);
                _context.SaveChanges();

                result = objEntity.UserKey;
            }
            return result;

        }

        public int Update(UsersViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_User objEntity = _context.Master_Users.Find(entityVM.UserKey);
                if (objEntity != null)
                {
                    // objEntity.Username = entityVM.Username;
                    objEntity.FName = entityVM.FName;
                    objEntity.LName = entityVM.LName;
                    objEntity.Mobile = entityVM.Mobile;
                    objEntity.Phone = entityVM.Phone;
                    objEntity.CountryKey = entityVM.CountryKey;
                    objEntity.Address = entityVM.Address;
                    objEntity.OrganizationTypeKey = entityVM.OrganizationTypeKey;
                    objEntity.Affiliation = entityVM.Affiliation;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.UserKey;
            }
            return result;
        }

        public void Delete(int key)
        {
            Master_User objEntity = _context.Master_Users.Find(key);
            _context.Master_Users.Remove(objEntity);
            _context.SaveChanges();
        }

        public void ActivateDeActivate(int key)
        {
            Master_User objEntity = _context.Master_Users.Find(key);
            if (objEntity != null)
            {
                objEntity.IsActive = objEntity.IsActive ? false : true;
                _context.Entry(objEntity).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void ResetPassword(int key)
        {
            Master_User objEntity = _context.Master_Users.Find(key);
            if (objEntity != null)
            {
                var resetPassword = GeneralClass.CreateRandomPassword();
                var keySalt = GeneralClass.GeneratePassword(10);
                var password = GeneralClass.EncodePassword(resetPassword, keySalt);

                objEntity.Password = password;
                objEntity.HashPassword = password;
                objEntity.SaltPassword = keySalt;

                objEntity.LastModifiedBy = Convert.ToInt16(key);
                objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                _context.Entry(objEntity).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void IncrementcntLogin(int key)
        {
            Master_User objEntity = _context.Master_Users.Find(key);
            if (objEntity != null)
            {
                objEntity.cntLogin = objEntity.cntLogin + 1;
                _context.Entry(objEntity).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public string GenerateCode(int userType)
        {
            string result = "";
            try
            {
                string objCode = _context.Master_Users.Where(d => d.UserType == userType).OrderByDescending(d => d.UserKey).FirstOrDefault().Code;

                if (objCode != "")
                {
                    int maxCode = Convert.ToInt32(objCode.Remove(0, 2)) + 1;
                    result = (userType == 3 ? "U0" + maxCode : "A0" + maxCode);
                }

                return result;
            }
            catch (Exception)
            {
                return result = "";
            }
        }

        public bool IsEmailExists(string emailid)
        {
            return _context.Master_Users.Where(u => u.Username == emailid).Any();

        }

        public bool UserVerification(string id)
        {
            bool Status = false;
            _context.Configuration.ValidateOnSaveEnabled = true;
            var IsVerify = _context.Master_Users.Where(u => u.VerificationCode == id).FirstOrDefault();
            if (IsVerify != null)
            {
                IsVerify.IsEmailVerified = true;
                IsVerify.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                _context.Entry(IsVerify).State = EntityState.Modified;
                _context.SaveChanges();
                Status = true;
            }
            return Status;
        }


        public void UpdatePassword()
        {
            var userList = _context.Master_Users.ToList();
            foreach (var item in userList)
            {
                Master_User objEntity = _context.Master_Users.Find(item.UserKey);
                var keySalt = GeneralClass.GeneratePassword(10);
                var password = GeneralClass.EncodePassword(objEntity.Password, keySalt);

                objEntity.HashPassword = password;
                objEntity.SaltPassword = keySalt;

                _context.Entry(objEntity).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int ChangePassword(ChangePasswordViewModel model)
        {
            Master_User objEntity = _context.Master_Users.Find(model.UserKey);
            objEntity.HashPassword = model.HashPassword;
            objEntity.Password = model.NewPassword;
            objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

            _context.Entry(objEntity).State = EntityState.Modified;
            _context.SaveChanges();

            return objEntity.UserKey;
        }


        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public interface IUsersRepository : IDisposable
    {
        IEnumerable<UsersViewModel> GetAll();
        IEnumerable<UsersViewModel> GetAllByUserType(int userType);
        UsersViewModel GetByKey(int key);
        UsersViewModel GetUserByEmaidId(string emailId);
        int Add(UsersViewModel entityVM);
        int Update(UsersViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
        void ResetPassword(int key);
        void IncrementcntLogin(int key);
        string GenerateCode(int userType);
        bool IsEmailExists(string emailid);
        bool UserVerification(string verificationcode);
        int ChangePassword(ChangePasswordViewModel model);
        void UpdatePassword();

    }
}