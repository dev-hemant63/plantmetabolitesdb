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
    public class AnnotationRepository : IAnnotationRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        public AnnotationRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<AnnotationViewModel> GetAll()
        {
            var lstEntity = (from dt in _context.Master_Annotations
                             select new AnnotationViewModel
                             {
                                 AnnotationKey = dt.AnnotationKey,
                                 AnnotationName = dt.AnnotationName,
                                 Polarity = dt.Polarity,
                                 AnnotationType = dt.AnnotationType,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).ToList();

            lstEntity.ForEach(y => y.PolarityName = gls.GetPolarity().Where(d => d.Value == y.Polarity.ToString()).FirstOrDefault().Text);
            lstEntity.ForEach(y => y.AnnotationTypeName = gls.GetAnnotationType().Where(d => d.Value == y.AnnotationType.ToString()).FirstOrDefault().Text);

            return lstEntity;
        }

        public IEnumerable<AnnotationViewModel> GetAnnotationByPolarity(int polarity)
        {
            var lstEntity = (from dt in _context.Master_Annotations
                             where dt.Polarity == polarity && dt.IsActive == true
                             select new AnnotationViewModel
                             {
                                 AnnotationKey = dt.AnnotationKey,
                                 AnnotationName = dt.AnnotationName,
                             }).ToList();

            //lstEntity.ForEach(y => y.PolarityName = gls.GetPolarity().Where(d => d.Value == y.Polarity.ToString()).FirstOrDefault().Text);
            //lstEntity.ForEach(y => y.AnnotationTypeName = gls.GetAnnotationType().Where(d => d.Value == y.AnnotationType.ToString()).FirstOrDefault().Text);

            return lstEntity;
        }

        public AnnotationViewModel GetByKey(int key)
        {
            var objEntity = (from dt in _context.Master_Annotations
                             where dt.AnnotationKey == key
                             select new AnnotationViewModel
                             {
                                 AnnotationKey = dt.AnnotationKey,
                                 AnnotationName = dt.AnnotationName,
                                 Polarity = dt.Polarity,
                                 AnnotationType = dt.AnnotationType,
                                 CreatedBy = dt.CreatedBy,
                                 CreatedOn = dt.CreatedOn,
                                 IsActive = dt.IsActive,
                                 LastModifiedBy = dt.LastModifiedBy,
                                 LastModifiedOn = dt.LastModifiedOn,
                                 StatusName = dt.IsActive == true ? "Active" : "InActive"
                             }).SingleOrDefault();

            objEntity.PolarityName = gls.GetPolarity().Where(d => d.Value == objEntity.Polarity.ToString()).FirstOrDefault().Text;
            objEntity.AnnotationTypeName = gls.GetAnnotationType().Where(d => d.Value == objEntity.AnnotationType.ToString()).FirstOrDefault().Text;

            return objEntity;
        }

        public int Add(AnnotationViewModel entityVM)
        {
            int result = -1;
            if (entityVM.AnnotationName != null)
            {
                Master_Annotation objEntity = new Master_Annotation();
                objEntity.AnnotationName = entityVM.AnnotationName;
                objEntity.Polarity = entityVM.Polarity;
                objEntity.AnnotationType = entityVM.AnnotationType;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.AddMinutes(330);
                objEntity.IsActive = true;

                _context.Master_Annotations.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.AnnotationKey;
            }
            return result;

        }
        public int Update(AnnotationViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_Annotation objEntity = _context.Master_Annotations.Find(entityVM.AnnotationKey);
                if (objEntity != null)
                {
                    objEntity.AnnotationName = entityVM.AnnotationName;
                    objEntity.Polarity = entityVM.Polarity;
                    objEntity.AnnotationType = entityVM.AnnotationType;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.AnnotationKey;
            }
            return result;
        }
        public void Delete(int key)
        {
            Master_Annotation objEntity = _context.Master_Annotations.Find(key);
            _context.Master_Annotations.Remove(objEntity);
            _context.SaveChanges();

        }

        public void ActivateDeActivate(int key)
        {
            Master_Annotation objEntity = _context.Master_Annotations.Find(key);
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

    public interface IAnnotationRepository : IDisposable
    {
        IEnumerable<AnnotationViewModel> GetAll();
        IEnumerable<AnnotationViewModel> GetAnnotationByPolarity(int polarity);
        AnnotationViewModel GetByKey(int key);
        int Add(AnnotationViewModel entityVM);
        int Update(AnnotationViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
    }

}