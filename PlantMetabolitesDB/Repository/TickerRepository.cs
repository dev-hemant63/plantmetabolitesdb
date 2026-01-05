using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class TickerRepository : ITickerRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        public TickerRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<TickerViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_Tickers
                             select new TickerViewModel
                             {
                                 TickerKey = dt.TickerKey,
                                 Title = dt.Title,
                                 URL = dt.URL,
                                 UploadedFileName = dt.UploadedFileName,
                                 MessageType = dt.MessageType ?? 0,
                                 CreatedBy = dt.CreatedBy ?? 0,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy ?? 0,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();


            lstEntity.ForEach(y => y.MessageTypeName = gls.GetMessageType().Where(d => d.Value == y.MessageType.ToString()).FirstOrDefault().Text);

            return lstEntity;
        }
        public TickerViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_Tickers
                             where dt.TickerKey == key
                             select new TickerViewModel
                             {
                                 TickerKey = dt.TickerKey,
                                 Title = dt.Title,
                                 URL = dt.URL,
                                 UploadedFileName = dt.UploadedFileName,
                                 MessageType = dt.MessageType ?? 0,
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

        public int Add(TickerViewModel entityVM)
        {
            int result = -1;
            if (entityVM.Title != null)
            {
                Master_Ticker objEntity = new Master_Ticker();
                objEntity.Title = entityVM.Title;
                objEntity.URL = entityVM.URL;
                objEntity.UploadedFileName = entityVM.UploadedFileName;
                objEntity.MessageType = entityVM.MessageType;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_Tickers.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.TickerKey;
            }
            return result;

        }
        public int Update(TickerViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_Ticker objEntity = _context.Master_Tickers.Find(entityVM.TickerKey);
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

                result = objEntity.TickerKey;
            }
            return result;
        }

        public void Delete(int key)
        {
            Master_Ticker objEntity = _context.Master_Tickers.Find(key);
            _context.Master_Tickers.Remove(objEntity);
            _context.SaveChanges();
        }
        public void ActivateDeActivate(int key)
        {
            Master_Ticker objEntity = _context.Master_Tickers.Find(key);
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

    public interface ITickerRepository : IDisposable
    {
        IEnumerable<TickerViewModel> GetAll();
        TickerViewModel GetByKey(int key);
        int Add(TickerViewModel entityVM);
        int Update(TickerViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
    }
}