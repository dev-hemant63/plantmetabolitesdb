using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class OrganizationTypeRepository : IOrganizationTypeRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        public OrganizationTypeRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<OrganizationTypeViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_OrganizationTypes
                       select new OrganizationTypeViewModel
                       {
                           OrganizationTypeKey = dt.OrganizationTypeKey,
                           OrganizationTypeName = dt.OrganizationTypeName,
                           IsAddByAdmin = dt.IsAddByAdmin,
                           CreatedBy = dt.CreatedBy,
                           CreatedOn = dt.CreatedOn,
                           IsActive = dt.IsActive,
                           LastModifiedBy = dt.LastModifiedBy,
                           LastModifiedOn = dt.LastModifiedOn,
                           StatusName = dt.IsActive == true ? "Active" : "InActive"
                       }).ToList();

            return lstEntity;
        }
        public OrganizationTypeViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_OrganizationTypes
                                 where dt.OrganizationTypeKey == key
                                 select new OrganizationTypeViewModel
                                 {
                                     OrganizationTypeKey = dt.OrganizationTypeKey,
                                     OrganizationTypeName = dt.OrganizationTypeName,
                                     IsAddByAdmin = dt.IsAddByAdmin,
                                     CreatedBy = dt.CreatedBy,
                                     CreatedOn = dt.CreatedOn,
                                     IsActive = dt.IsActive,
                                     LastModifiedBy = dt.LastModifiedBy,
                                     LastModifiedOn = dt.LastModifiedOn,
                                     StatusName = dt.IsActive == true ? "Active" : "InActive"
                                 }).SingleOrDefault();

            return objEntity;
        }

        public int Add(OrganizationTypeViewModel entityVM)
        {
            int result = -1;
            if (entityVM.OrganizationTypeName != null)
            {
                Master_OrganizationType objEntity = new Master_OrganizationType();
                objEntity.OrganizationTypeName = entityVM.OrganizationTypeName;
                objEntity.IsAddByAdmin = entityVM.IsAddByAdmin;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_OrganizationTypes.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.OrganizationTypeKey;
            }
            return result;

        }
        public int Update(OrganizationTypeViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_OrganizationType objEntity = _context.Master_OrganizationTypes.Find(entityVM.OrganizationTypeKey);
                if (objEntity != null)
                {
                    objEntity.OrganizationTypeName = entityVM.OrganizationTypeName;
                    objEntity.IsAddByAdmin= entityVM.IsAddByAdmin;
                    // objEntity.CreatedBy = entityVM.CreatedBy;
                    // objEntity.CreatedOn = entityVM.CreatedOn;
                    // objEntity.IsActive = entityVM.IsActive;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.OrganizationTypeKey;
            }
            return result;
        }
        public void Delete(int key)
        {
            Master_OrganizationType objEntity = _context.Master_OrganizationTypes.Find(key);
            _context.Master_OrganizationTypes.Remove(objEntity);
            _context.SaveChanges();

        }

        public void ActivateDeActivate(int key)
        {
            Master_OrganizationType objEntity = _context.Master_OrganizationTypes.Find(key);
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

    public interface IOrganizationTypeRepository : IDisposable
    {
        IEnumerable<OrganizationTypeViewModel> GetAll();
        OrganizationTypeViewModel GetByKey(int key);
        int Add(OrganizationTypeViewModel entityVM);
        int Update(OrganizationTypeViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
    }

}