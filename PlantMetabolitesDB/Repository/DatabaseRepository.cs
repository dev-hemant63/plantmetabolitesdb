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
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        public DatabaseRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<DatabaseViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_Databases
                             select new DatabaseViewModel
                             {
                                 DatabaseKey = (Int16)dt.DatabaseKey,
                                 DatabaseName = dt.DatabaseName,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();

            return lstEntity;
        }
        public DatabaseViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_Databases
                               where dt.DatabaseKey == key
                               select new DatabaseViewModel
                               {
                                   DatabaseKey = (Int16)dt.DatabaseKey,
                                   DatabaseName = dt.DatabaseName,
                                   CreatedBy = dt.CreatedBy,
                                   CreatedOn = dt.CreatedOn,
                                   IsActive = dt.IsActive,
                                   LastModifiedBy = dt.LastModifiedBy,
                                   LastModifiedOn = dt.LastModifiedOn,
                                   StatusName = dt.IsActive == true ? "Active" : "InActive"
                               }).SingleOrDefault();

            return objEntity;
        }

        public int Add(DatabaseViewModel entityVM)
        {
            int result = -1;
            if (entityVM.DatabaseName != null)
            {
                Master_Database objEntity = new Master_Database();
                objEntity.DatabaseName = entityVM.DatabaseName;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_Databases.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.DatabaseKey;
            }
            return result;

        }
        public int Update(DatabaseViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_Database objEntity = _context.Master_Databases.Find(entityVM.DatabaseKey);
                if (objEntity != null)
                {
                    objEntity.DatabaseName = entityVM.DatabaseName;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.DatabaseKey;
            }
            return result;
        }
        public void Delete(int key)
        {
            Master_Database objEntity = _context.Master_Databases.Find(key);
            _context.Master_Databases.Remove(objEntity);
            _context.SaveChanges();
        }
        public void ActivateDeActivate(int key)
        {
            Master_Database objEntity = _context.Master_Databases.Find(key);
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

    public interface IDatabaseRepository : IDisposable
    {
        IEnumerable<DatabaseViewModel> GetAll();
        DatabaseViewModel GetByKey(int key);
        int Add(DatabaseViewModel entityVM);
        int Update(DatabaseViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
    }

}