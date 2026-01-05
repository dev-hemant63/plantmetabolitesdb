using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class MS3MassSpectraRepository : IMS3MassSpectraRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        private bool disposed = false;
        public MS3MassSpectraRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<MS3MassSpectraViewModel> GetAll()
        {
            return _context.Master_MS3MassSpectras.Select(d => new MS3MassSpectraViewModel
            {
                MS3MassSpectraKey = d.MS3MassSpectraKey,
                InstrumentKey = d.InstrumentKey,
                Polarity = d.Polarity,
                ParentIon = d.ParentIon,
                AnnotationKey = d.AnnotationKey,
                CompoundKey = d.CompoundKey,
                SpectraDescription = d.SpectraDescription,
                MS3DataFile = d.MS3DataFile,
                MS3RefFile = d.MS3RefFile,
                MS3SpectraFile = d.MS3SpectraFile,
                CreatedBy = d.CreatedBy,
                CreatedOn = d.CreatedOn,
                LastModifiedBy = d.LastModifiedBy,
                LastModifiedOn = d.LastModifiedOn,

            }).OrderByDescending(d => d.MS3MassSpectraKey).ToList();
        }

        public IEnumerable<MS3MassSpectraViewModel> GetAllByInstrumentKey(int instKey, int compKey)
        {
            var lstEntity = (from ms in _context.Master_MS3MassSpectras
                             where ms.InstrumentKey == instKey && ms.CompoundKey == compKey
                             join an in _context.Master_Annotations on ms.AnnotationKey equals an.AnnotationKey
                             select new MS3MassSpectraViewModel
                             {
                                 MS3MassSpectraKey = ms.MS3MassSpectraKey,
                                 InstrumentKey = ms.InstrumentKey,
                                 Polarity = ms.Polarity,
                                 ParentIon = ms.ParentIon,
                                 AnnotationKey = ms.AnnotationKey,
                                 CompoundKey = ms.CompoundKey,
                                 SpectraDescription = ms.SpectraDescription,
                                 MS3DataFile = ms.MS3DataFile,
                                 MS3RefFile = ms.MS3RefFile,
                                 MS3SpectraFile = ms.MS3SpectraFile,
                                 CreatedBy = ms.CreatedBy,
                                 CreatedOn = ms.CreatedOn,
                                 LastModifiedBy = ms.LastModifiedBy,
                                 LastModifiedOn = ms.LastModifiedOn,
                                 AnnotationName = an.AnnotationName,

                             }).OrderByDescending(d => d.MS3MassSpectraKey).ToList();

            lstEntity.ForEach(y => y.PolarityName = gls.GetPolarity().Where(d => d.Value == y.Polarity.ToString()).FirstOrDefault().Text);
            return lstEntity;
        }

        public IEnumerable<MS3MassSpectraViewModel> GetByCompoundKey(int compoundKey)
        {
            return _context.Master_MS3MassSpectras.Where(x => x.CompoundKey == compoundKey)
                .Select(d => new MS3MassSpectraViewModel
                {
                    MS3MassSpectraKey = d.MS3MassSpectraKey,
                    InstrumentKey = d.InstrumentKey,
                    Polarity = d.Polarity,
                    ParentIon = d.ParentIon,
                    AnnotationKey = d.AnnotationKey,
                    CompoundKey = d.CompoundKey,
                    SpectraDescription = d.SpectraDescription,
                    MS3DataFile = d.MS3DataFile,
                    MS3RefFile = d.MS3RefFile,
                    MS3SpectraFile = d.MS3SpectraFile,
                }).ToList();
        }

        public MS3MassSpectraViewModel GetByKey(int key)
        {
            return _context.Master_MS3MassSpectras.Where(x => x.MS3MassSpectraKey == key)
                .Select(d => new MS3MassSpectraViewModel
                {
                    MS3MassSpectraKey = d.MS3MassSpectraKey,
                    InstrumentKey = d.InstrumentKey,
                    Polarity = d.Polarity,
                    ParentIon = d.ParentIon,
                    AnnotationKey = d.AnnotationKey,
                    CompoundKey = d.CompoundKey,
                    SpectraDescription = d.SpectraDescription,
                    MS3DataFile = d.MS3DataFile,
                    MS3RefFile = d.MS3RefFile,
                    MS3SpectraFile = d.MS3SpectraFile,
                    CreatedBy = d.CreatedBy,
                    CreatedOn = d.CreatedOn,
                    LastModifiedBy = d.LastModifiedBy,
                    LastModifiedOn = d.LastModifiedOn,

                }).FirstOrDefault();
        }

        public int Add(MS3MassSpectraViewModel entityVM)
        {
            int result = -1;
            if (entityVM.InstrumentKey > 0 && entityVM.CompoundKey > 0 && entityVM.Polarity > 0 && entityVM.AnnotationKey > 0 && entityVM.ParentIon != null)
            {
                Master_MS3MassSpectra objEntity = new Master_MS3MassSpectra();
                objEntity.Polarity = entityVM.Polarity;
                objEntity.AnnotationKey = entityVM.AnnotationKey;
                objEntity.InstrumentKey = entityVM.InstrumentKey;
                objEntity.ParentIon = entityVM.ParentIon;
                objEntity.CompoundKey = entityVM.CompoundKey;
                objEntity.SpectraDescription = entityVM.SpectraDescription;
                objEntity.MS3DataFile = entityVM.MS3DataFile;
                objEntity.MS3RefFile = entityVM.MS3RefFile;
                objEntity.MS3SpectraFile = entityVM.MS3SpectraFile;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.Date.AddMinutes(330);

                _context.Master_MS3MassSpectras.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.CompoundKey;
                Int16 massSpectraKey = objEntity.MS3MassSpectraKey;
                if (result > 0)
                {
                    foreach (var entityMSDV in entityVM.lstMassSpectra_DataValues)
                    {
                        MassSpectra_DataValues obj = new MassSpectra_DataValues();
                        obj.MassSpectraKey = massSpectraKey;
                        obj.MassSpectraType = entityMSDV.MassSpectraType;
                        obj.absolute = entityMSDV.absolute;
                        obj.relative = entityMSDV.relative;
                        obj.relative_int = entityMSDV.relative_int;
                        obj.mz = entityMSDV.mz;
                        obj.mz_int = entityMSDV.mz_int;
                        obj.absolute_int = entityMSDV.absolute_int;
                        _context.MassSpectra_DataValuess.Add(obj);
                        _context.SaveChanges();
                    }
                }
            }
            return result;
        }

        public int Update(MS3MassSpectraViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_MS3MassSpectra objEntity = _context.Master_MS3MassSpectras.Find(entityVM.MS3MassSpectraKey);
                if (objEntity != null)
                {
                    objEntity.Polarity = entityVM.Polarity;
                    objEntity.AnnotationKey = entityVM.AnnotationKey;
                    objEntity.InstrumentKey = entityVM.InstrumentKey;
                    objEntity.ParentIon = entityVM.ParentIon;
                    objEntity.CompoundKey = entityVM.CompoundKey;
                    objEntity.SpectraDescription = entityVM.SpectraDescription;
                    objEntity.MS3DataFile = entityVM.MS3DataFile;
                    objEntity.MS3RefFile = entityVM.MS3RefFile;
                    objEntity.MS3SpectraFile = entityVM.MS3SpectraFile;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.MS3MassSpectraKey;
            }
            return result;
        }
        public void Delete(int key)
        {
            Master_MS3MassSpectra objEntity = _context.Master_MS3MassSpectras.Find(key);
            _context.Master_MS3MassSpectras.Remove(objEntity);
            _context.SaveChanges();
        }
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
    public interface IMS3MassSpectraRepository : IDisposable
    {
        IEnumerable<MS3MassSpectraViewModel> GetAll();
        IEnumerable<MS3MassSpectraViewModel> GetAllByInstrumentKey(int instKey, int compKey);
        IEnumerable<MS3MassSpectraViewModel> GetByCompoundKey(int compoundKey);
        MS3MassSpectraViewModel GetByKey(int key);
        int Add(MS3MassSpectraViewModel entityVM);
        int Update(MS3MassSpectraViewModel entityVM);
        void Delete(int key);
    }

}