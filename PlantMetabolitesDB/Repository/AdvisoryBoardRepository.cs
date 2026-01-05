using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class AdvisoryBoardRepository: IAdvisoryBoardRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        public AdvisoryBoardRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<AdvisoryBoardViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_AdvisioryBoards
                             select new AdvisoryBoardViewModel
                             {
                                 AdvisioryBoradKey = dt.AdvisioryBoradKey,
                                 Name = dt.Name,
                                 DisplayOrder = dt.DisplayOrder,
                                 Affiliation = dt.Affiliation,
                                 Designation = dt.Designation,
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

        public AdvisoryBoardViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_AdvisioryBoards                         
                             where dt.AdvisioryBoradKey == key
                             select new AdvisoryBoardViewModel
                             {
                                 AdvisioryBoradKey = dt.AdvisioryBoradKey,
                                 Name = dt.Name,
                                 DisplayOrder = dt.DisplayOrder,
                                 Affiliation = dt.Affiliation,
                                 Designation = dt.Designation,
                                 CountryKey = dt.CountryKey,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 CountryName = dt.Country.CountryName,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).SingleOrDefault();

            return objEntity;
        }

        public int Add(AdvisoryBoardViewModel entityVM)
        {
            int result = -1;
            if (entityVM.Name != null)
            {
                Master_AdvisioryBoard objEntity = new Master_AdvisioryBoard();
                objEntity.Name = entityVM.Name;
                objEntity.DisplayOrder = entityVM.DisplayOrder;
                objEntity.Affiliation = entityVM.Affiliation;
                objEntity.Designation = entityVM.Designation;
                objEntity.CountryKey = entityVM.CountryKey;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_AdvisioryBoards.Add(objEntity);
                _context.SaveChanges();

                result = objEntity.AdvisioryBoradKey;
            }
            return result;

        }

        public int Update(AdvisoryBoardViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_AdvisioryBoard objEntity = _context.Master_AdvisioryBoards.Find(entityVM.AdvisioryBoradKey);
                if (objEntity != null)
                {
                    objEntity.Name = entityVM.Name;
                    objEntity.DisplayOrder = entityVM.DisplayOrder;
                    objEntity.Affiliation = entityVM.Affiliation;
                    objEntity.Designation = entityVM.Designation;
                    objEntity.CountryKey = entityVM.CountryKey;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.AdvisioryBoradKey;
            }
            return result;
        }

        public void Delete(int key)
        {
            Master_AdvisioryBoard objEntity = _context.Master_AdvisioryBoards.Find(key);
            _context.Master_AdvisioryBoards.Remove(objEntity);
            _context.SaveChanges();
        }

        public void ActivateDeActivate(int key)
        {
            Master_AdvisioryBoard objEntity = _context.Master_AdvisioryBoards.Find(key);
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

    public interface IAdvisoryBoardRepository : IDisposable
    {
        IEnumerable<AdvisoryBoardViewModel> GetAll();
        AdvisoryBoardViewModel GetByKey(int key);
        int Add(AdvisoryBoardViewModel entityVM);
        int Update(AdvisoryBoardViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
    }
}
