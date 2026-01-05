using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class LatestUpdatesRepository: ILatestUpdatesRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        public LatestUpdatesRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<LatestUpdatesViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_LatestUpdatess
                             select new LatestUpdatesViewModel
                             {
                                 LatestUpdatesKey = dt.LatestUpdatesKey,
                                 Title = dt.Title,
                                 URL = dt.URL,
                                 UploadedFileName = dt.UploadedFileName,
                                 MessageType = dt.MessageType,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();

            lstEntity.ForEach(y => y.MessageTypeName = gls.GetMessageType().Where(d => d.Value == y.MessageType.ToString()).FirstOrDefault().Text);

            return lstEntity;
        }
        public LatestUpdatesViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_LatestUpdatess
                             where dt.LatestUpdatesKey == key
                             select new LatestUpdatesViewModel
                             {
                                 LatestUpdatesKey = dt.LatestUpdatesKey,
                                 Title = dt.Title,
                                 URL = dt.URL,
                                 UploadedFileName = dt.UploadedFileName,
                                 MessageType = dt.MessageType,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).SingleOrDefault();

            objEntity.MessageTypeName = gls.GetMessageType().Where(d => d.Value == objEntity.MessageType.ToString()).FirstOrDefault().Text;

            return objEntity;
        }

        public int Add(LatestUpdatesViewModel entityVM)
        {
            int result = -1;
            if (entityVM.Title != null)
            {
                Master_LatestUpdates objEntity = new Master_LatestUpdates();
                objEntity.Title = entityVM.Title;
                objEntity.URL = entityVM.URL;
                objEntity.UploadedFileName = entityVM.UploadedFileName;
                objEntity.MessageType = entityVM.MessageType;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_LatestUpdatess.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.LatestUpdatesKey;
            }
            return result;

        }
        public int Update(LatestUpdatesViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_LatestUpdates objEntity = _context.Master_LatestUpdatess.Find(entityVM.LatestUpdatesKey);
                if (objEntity != null)
                {
                    objEntity.Title = entityVM.Title;
                    objEntity.URL = entityVM.URL;
                    objEntity.UploadedFileName = entityVM.UploadedFileName;
                    objEntity.MessageType = entityVM.MessageType;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.LatestUpdatesKey;
            }
            return result;
        }

        public void Delete(int key)
        {
            Master_LatestUpdates objEntity = _context.Master_LatestUpdatess.Find(key);
            _context.Master_LatestUpdatess.Remove(objEntity);
            _context.SaveChanges();
        }
        public void ActivateDeActivate(int key)
        {
            Master_LatestUpdates objEntity = _context.Master_LatestUpdatess.Find(key);
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

    public interface ILatestUpdatesRepository : IDisposable
    {
        IEnumerable<LatestUpdatesViewModel> GetAll();
        LatestUpdatesViewModel GetByKey(int key);
        int Add(LatestUpdatesViewModel entityVM);
        int Update(LatestUpdatesViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
    }
}