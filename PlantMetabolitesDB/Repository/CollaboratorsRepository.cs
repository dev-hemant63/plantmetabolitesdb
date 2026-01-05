using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class CollaboratorsRepository : ICollaboratorsRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        public CollaboratorsRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<CollaboratorsViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_Collaboratorss
                                 // join c in _context.Master_Countrys on dt.CountryKey equals c.CountryKey
                             select new CollaboratorsViewModel
                             {
                                 CollaboratorsKey = dt.CollaboratorsKey,
                                 Name = dt.Name,
                                 EmailId = dt.EmailId,
                                 Affiliation = dt.Affiliation,
                                 Designation = dt.Designation,
                                 ContactNo = dt.ContactNo,
                                 UserType = dt.UserType,
                                 CountryKey = dt.CountryKey,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 CountryName = dt.Country.CountryName,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();

            lstEntity.ForEach(y => y.CollaboratorTypeName = gls.GetCollaboratorsType().Where(d => d.Value == y.UserType.ToString()).FirstOrDefault().Text);


            return lstEntity;
        }

        public IEnumerable<CollaboratorsViewModel> GetAllByType(int ctype)
        {
            var lstEntity = (from dt in _context.Master_Collaboratorss
                             where dt.UserType == ctype && dt.IsActive == true
                             select new CollaboratorsViewModel
                             {
                                 CollaboratorsKey = dt.CollaboratorsKey,
                                 Name = dt.Name,
                                 EmailId = dt.EmailId,
                                 Affiliation = dt.Affiliation,
                                 Designation = dt.Designation,
                                 ContactNo = dt.ContactNo,
                                 UserType = dt.UserType,
                                 CountryKey = dt.CountryKey,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 CountryName = dt.Country.CountryName,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();

            return lstEntity;
        }
        public CollaboratorsViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_Collaboratorss
                                 // join c in _context.Master_Countrys on dt.CountryKey equals c.CountryKey
                             where dt.CollaboratorsKey == key
                             select new CollaboratorsViewModel
                             {
                                 CollaboratorsKey = dt.CollaboratorsKey,
                                 Name = dt.Name,
                                 EmailId = dt.EmailId,
                                 Affiliation = dt.Affiliation,
                                 Designation = dt.Designation,
                                 ContactNo = dt.ContactNo,
                                 UserType = dt.UserType,
                                 CountryKey = dt.CountryKey,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 CountryName = dt.Country.CountryName,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).SingleOrDefault();

            objEntity.CollaboratorTypeName = gls.GetCollaboratorsType().Where(d => d.Value == objEntity.UserType.ToString()).FirstOrDefault().Text;

            return objEntity;
        }

        public int Add(CollaboratorsViewModel entityVM)
        {
            int result = -1;
            if (entityVM.Name != null)
            {
                Master_Collaborators objEntity = new Master_Collaborators();
                objEntity.Name = entityVM.Name;
                objEntity.EmailId = entityVM.EmailId;
                objEntity.Affiliation = entityVM.Affiliation;
                objEntity.Designation = entityVM.Designation;
                objEntity.ContactNo = entityVM.ContactNo;
                objEntity.UserType = entityVM.UserType;
                objEntity.CountryKey = entityVM.CountryKey;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_Collaboratorss.Add(objEntity);
                _context.SaveChanges();

                result = objEntity.CollaboratorsKey;
            }
            return result;

        }

        public int Update(CollaboratorsViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_Collaborators objEntity = _context.Master_Collaboratorss.Find(entityVM.CollaboratorsKey);
                if (objEntity != null)
                {
                    objEntity.Name = entityVM.Name;
                    objEntity.EmailId = entityVM.EmailId;
                    objEntity.Affiliation = entityVM.Affiliation;
                    objEntity.Designation = entityVM.Designation;
                    objEntity.ContactNo = entityVM.ContactNo;
                    objEntity.UserType = entityVM.UserType;
                    objEntity.CountryKey = entityVM.CountryKey;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.CollaboratorsKey;
            }
            return result;
        }

        public void Delete(int key)
        {
            Master_Collaborators objEntity = _context.Master_Collaboratorss.Find(key);
            _context.Master_Collaboratorss.Remove(objEntity);
            _context.SaveChanges();
        }

        public void ActivateDeActivate(int key)
        {
            Master_Collaborators objEntity = _context.Master_Collaboratorss.Find(key);
            if (objEntity != null)
            {
                objEntity.IsActive = objEntity.IsActive ? false : true;
                _context.Entry(objEntity).State = EntityState.Modified;
                _context.SaveChanges();
            }
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

    public interface ICollaboratorsRepository : IDisposable
    {
        IEnumerable<CollaboratorsViewModel> GetAll();
        IEnumerable<CollaboratorsViewModel> GetAllByType(int ctype);
        
        CollaboratorsViewModel GetByKey(int key);
        int Add(CollaboratorsViewModel entityVM);
        int Update(CollaboratorsViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
    }
}