using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class FAQRepository : IFAQRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        public FAQRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<FAQViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_FAQs
                             select new FAQViewModel
                             {
                                 FAQKey = dt.FAQKey,
                                 Title = dt.Title,
                                 Description = dt.Description,
                                 UploadedFileName = dt.UploadedFileName,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive",
                                 fileExtension = dt.UploadedFileName == null ? "" : dt.UploadedFileName.Substring(dt.UploadedFileName.IndexOf(".") + 1)
                             }).ToList();

            return lstEntity;
        }

        public FAQViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_FAQs
                             where dt.FAQKey == key
                             select new FAQViewModel
                             {
                                 FAQKey = dt.FAQKey,
                                 Title = dt.Title,
                                 Description = dt.Description,
                                 UploadedFileName = dt.UploadedFileName,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).SingleOrDefault();

            return objEntity;
        }
        public int Add(FAQViewModel entityVM)
        {
            int result = -1;
            if (entityVM.Title != null)
            {
                Master_FAQ objEntity = new Master_FAQ();
                objEntity.Title = entityVM.Title;
                objEntity.Description = entityVM.Description;
                objEntity.UploadedFileName = entityVM.UploadedFileName;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_FAQs.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.FAQKey;
            }
            return result;

        }

        public int Update(FAQViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_FAQ objEntity = _context.Master_FAQs.Find(entityVM.FAQKey);
                if (objEntity != null)
                {
                    objEntity.Title = entityVM.Title;
                    objEntity.Description = entityVM.Description;
                    objEntity.UploadedFileName = entityVM.UploadedFileName;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.FAQKey;
            }
            return result;
        }


        public void Delete(int key)
        {
            Master_FAQ objEntity = _context.Master_FAQs.Find(key);
            _context.Master_FAQs.Remove(objEntity);
            _context.SaveChanges();
        }
        public void ActivateDeActivate(int key)
        {
            Master_FAQ objEntity = _context.Master_FAQs.Find(key);
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

    public interface IFAQRepository : IDisposable
    {
        IEnumerable<FAQViewModel> GetAll();
        FAQViewModel GetByKey(int key);
        int Add(FAQViewModel entityVM);
        int Update(FAQViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
    }
}