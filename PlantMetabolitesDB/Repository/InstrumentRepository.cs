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
    public class InstrumentRepository : IInstrumentRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        public InstrumentRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<InstrumentViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_Instruments
                             select new InstrumentViewModel
                             {
                                 InstrumentKey = dt.InstrumentKey,
                                 InstrumentName = dt.InstrumentName,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();

            return lstEntity;
        }
        public InstrumentViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_Instruments
                             where dt.InstrumentKey == key
                             select new InstrumentViewModel
                             {
                                 InstrumentKey = dt.InstrumentKey,
                                 InstrumentName = dt.InstrumentName,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).SingleOrDefault();

            return objEntity;
        }

        public IEnumerable<InstrumentViewModel> GetInstrumentByDatabaseKey(int databasekey)
        {
            var lstEntity = new List<InstrumentViewModel>();
            lstEntity = (from ins in _context.Master_Instruments
                         join dtins in _context.Master_Database_Instruments on ins.InstrumentKey equals dtins.InstrumentKey
                         where dtins.DatabaseKey == databasekey
                         select new InstrumentViewModel
                         {
                             InstrumentKey = ins.InstrumentKey,
                             InstrumentName = ins.InstrumentName,
                         }).ToList();

            return lstEntity;
        }

        public IEnumerable<DatabaseInstrumentViewModel> GetDatabaseByInstrumentKey(int instrumentkey)
        {
            var lstEntity = (from dtins in _context.Master_Database_Instruments
                             where dtins.InstrumentKey == instrumentkey
                             select new DatabaseInstrumentViewModel
                             {
                                 DatabaseInstrumentKey = dtins.DatabaseInstrumentKey,
                                 DatabaseKey = dtins.DatabaseKey,
                                 InstrumentKey = dtins.InstrumentKey,
                             }).ToList();

            return lstEntity;
        }

        public int Add(InstrumentViewModel entityVM)
        {
            int result = -1;
            if (entityVM.InstrumentName != null)
            {
                Master_Instrument objEntity = new Master_Instrument();
                objEntity.InstrumentName = entityVM.InstrumentName;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_Instruments.Add(objEntity);
                _context.SaveChanges();

                result = objEntity.InstrumentKey;
            }
            return result;

        }
        public int Update(InstrumentViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_Instrument objEntity = _context.Master_Instruments.Find(entityVM.InstrumentKey);
                if (objEntity != null)
                {
                    objEntity.InstrumentName = entityVM.InstrumentName;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                //Master_Database_Instrument objEntity2 = _context.Master_Database_Instruments.Find(entityVM.DatabaseInstrumentKey);
                //if (objEntity2 != null)
                //{
                //    objEntity2.InstrumentKey = entityVM.InstrumentKey;
                //    objEntity2.DatabaseKey = entityVM.DatabaseKey;

                //    _context.Entry(objEntity2).State = EntityState.Modified;
                //    _context.SaveChanges();
                //}
                result = objEntity.InstrumentKey;
            }
            return result;
        }
        public void Delete(int key)
        {
            Master_Instrument objEntity = _context.Master_Instruments.Find(key);
            _context.Master_Instruments.Remove(objEntity);
            _context.SaveChanges();
        }

        public void ActivateDeActivate(int key)
        {
            Master_Instrument objEntity = _context.Master_Instruments.Find(key);
            if (objEntity != null)
            {
                objEntity.IsActive = objEntity.IsActive ? false : true;
                _context.Entry(objEntity).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int AddMapDatabaseInstrument(List<DbInstViewModel> entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Int16 instrumentKey = Convert.ToInt16(entityVM[0].InstrumentKey);
                var lstEntity = _context.Master_Database_Instruments.Where(d => d.InstrumentKey == instrumentKey).ToList();
                if (lstEntity.Count > 0)
                {
                    _context.Master_Database_Instruments.RemoveRange(lstEntity);
                    _context.SaveChanges();
                }
                //Map Database Instrument
                try
                {
                    Master_Database_Instrument objMapDatabase = new Master_Database_Instrument();
                   //objMapDatabase.InstrumentKey = entityVM.ElementAt(0).InstrumentKey;
                    foreach (var item in entityVM)
                    {
                        if (item.IsCheked)
                        {
                            objMapDatabase.InstrumentKey = item.InstrumentKey;
                            objMapDatabase.DatabaseKey = item.DatabaseKey;
                            _context.Master_Database_Instruments.Add(objMapDatabase);
                            _context.SaveChanges();
                        }

                        result = objMapDatabase.DatabaseInstrumentKey;
                    }
                }
                catch (Exception)
                {
                    result = -1;
                }
            }
            return result;

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

    public interface IInstrumentRepository : IDisposable
    {
        IEnumerable<InstrumentViewModel> GetAll();
        IEnumerable<InstrumentViewModel> GetInstrumentByDatabaseKey(int databasekey);
        InstrumentViewModel GetByKey(int key);
        int Add(InstrumentViewModel entityVM);
        int Update(InstrumentViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
        IEnumerable<DatabaseInstrumentViewModel> GetDatabaseByInstrumentKey(int instrumentKey);

        int AddMapDatabaseInstrument(List<DbInstViewModel> entityVM);
    }

}