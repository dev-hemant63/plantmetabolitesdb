using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class AductMassSpectraRepository:IAductMassSpectraRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        private bool disposed = false;
        public AductMassSpectraRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<AductMassSpectraViewModel> GetAll()
        {
            return _context.Master_AductMassSpectras.Select(d => new AductMassSpectraViewModel
            {
                AductMassSpectraKey = d.AductMassSpectraKey,
                InstrumentKey = d.InstrumentKey,
                Polarity = d.Polarity,
                ParentIon = d.ParentIon,
                AnnotationKey = d.AnnotationKey,
                CompoundKey = d.CompoundKey,
                SpectraDescription = d.SpectraDescription,
                AductDataFile = d.AductDataFile,
                AductRefFile = d.AductRefFile,
                AductSpectraFile = d.AductSpectraFile,
                CreatedBy = d.CreatedBy,
                CreatedOn = d.CreatedOn,
                LastModifiedBy = d.LastModifiedBy,
                LastModifiedOn = d.LastModifiedOn,

            }).OrderByDescending(d => d.AductMassSpectraKey).ToList();
        }

        public IEnumerable<AductMassSpectraViewModel> GetAllByInstrumentKey(int instKey, int compKey)
        {
            var lstEntity = (from ms in _context.Master_AductMassSpectras
                             where ms.InstrumentKey == instKey && ms.CompoundKey == compKey
                             join an in _context.Master_Annotations on ms.AnnotationKey equals an.AnnotationKey
                             select new AductMassSpectraViewModel
                             {
                                 AductMassSpectraKey = ms.AductMassSpectraKey,
                                 InstrumentKey = ms.InstrumentKey,
                                 Polarity = ms.Polarity,
                                 ParentIon = ms.ParentIon,
                                 AnnotationKey = ms.AnnotationKey,
                                 CompoundKey = ms.CompoundKey,
                                 SpectraDescription = ms.SpectraDescription,
                                 AductDataFile = ms.AductDataFile,
                                 AductRefFile = ms.AductRefFile,
                                 AductSpectraFile = ms.AductSpectraFile,
                                 CreatedBy = ms.CreatedBy,
                                 CreatedOn = ms.CreatedOn,
                                 LastModifiedBy = ms.LastModifiedBy,
                                 LastModifiedOn = ms.LastModifiedOn,
                                 AnnotationName = an.AnnotationName,

                             }).OrderByDescending(d => d.AductMassSpectraKey).ToList();

            lstEntity.ForEach(y => y.PolarityName = gls.GetPolarity().Where(d => d.Value == y.Polarity.ToString()).FirstOrDefault().Text);
            return lstEntity;
        }
        public IEnumerable<AductMassSpectraViewModel> GetByCompoundKey(int compoundKey)
        {
            return _context.Master_AductMassSpectras.Where(x => x.CompoundKey == compoundKey)
                .Select(d => new AductMassSpectraViewModel
                {
                    AductMassSpectraKey = d.AductMassSpectraKey,
                    InstrumentKey = d.InstrumentKey,
                    Polarity = d.Polarity,
                    ParentIon = d.ParentIon,
                    AnnotationKey = d.AnnotationKey,
                    CompoundKey = d.CompoundKey,
                    SpectraDescription = d.SpectraDescription,
                    AductDataFile = d.AductDataFile,
                    AductRefFile = d.AductRefFile,
                    AductSpectraFile = d.AductSpectraFile,
                }).ToList();
        }

        public AductMassSpectraViewModel GetByKey(int key)
        {
            return _context.Master_AductMassSpectras.Where(x => x.AductMassSpectraKey == key)
                .Select(d => new AductMassSpectraViewModel
                {
                    AductMassSpectraKey = d.AductMassSpectraKey,
                    InstrumentKey = d.InstrumentKey,
                    Polarity = d.Polarity,
                    ParentIon = d.ParentIon,
                    AnnotationKey = d.AnnotationKey,
                    CompoundKey = d.CompoundKey,
                    SpectraDescription = d.SpectraDescription,
                    AductDataFile = d.AductDataFile,
                    AductRefFile = d.AductRefFile,
                    AductSpectraFile = d.AductSpectraFile,
                    CreatedBy = d.CreatedBy,
                    CreatedOn = d.CreatedOn,
                    LastModifiedBy = d.LastModifiedBy,
                    LastModifiedOn = d.LastModifiedOn,

                }).FirstOrDefault();
        }

        public int Add(AductMassSpectraViewModel entityVM)
        {
            int result = -1;
            if (entityVM.InstrumentKey > 0 && entityVM.CompoundKey > 0 && entityVM.Polarity > 0 && entityVM.AnnotationKey > 0 && entityVM.ParentIon != null)
            {
                Master_AductMassSpectra objEntity = new Master_AductMassSpectra();
                objEntity.Polarity = entityVM.Polarity;
                objEntity.AnnotationKey = entityVM.AnnotationKey;
                objEntity.InstrumentKey = entityVM.InstrumentKey;
                objEntity.ParentIon = entityVM.ParentIon;
                objEntity.CompoundKey = entityVM.CompoundKey;
                objEntity.SpectraDescription = entityVM.SpectraDescription;
                objEntity.AductDataFile = entityVM.AductDataFile;
                objEntity.AductRefFile = entityVM.AductRefFile;
                objEntity.AductSpectraFile = entityVM.AductSpectraFile;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.Date.AddMinutes(330);

                _context.Master_AductMassSpectras.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.CompoundKey;
                Int16 massSpectraKey = objEntity.AductMassSpectraKey;
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

        public int Update(AductMassSpectraViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_AductMassSpectra objEntity = _context.Master_AductMassSpectras.Find(entityVM.AductMassSpectraKey);
                if (objEntity != null)
                {
                    objEntity.Polarity = entityVM.Polarity;
                    objEntity.AnnotationKey = entityVM.AnnotationKey;
                    objEntity.InstrumentKey = entityVM.InstrumentKey;
                    objEntity.ParentIon = entityVM.ParentIon;
                    objEntity.CompoundKey = entityVM.CompoundKey;
                    objEntity.SpectraDescription = entityVM.SpectraDescription;
                    objEntity.AductDataFile = entityVM.AductDataFile;
                    objEntity.AductRefFile = entityVM.AductRefFile;
                    objEntity.AductSpectraFile = entityVM.AductSpectraFile;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.AductMassSpectraKey;
            }
            return result;
        }
        public void Delete(int key)
        {
            Master_AductMassSpectra objEntity = _context.Master_AductMassSpectras.Find(key);
            _context.Master_AductMassSpectras.Remove(objEntity);
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

    public interface IAductMassSpectraRepository:IDisposable
    {
        IEnumerable<AductMassSpectraViewModel> GetAll();
        IEnumerable<AductMassSpectraViewModel> GetAllByInstrumentKey(int instKey, int compKey);
        IEnumerable<AductMassSpectraViewModel> GetByCompoundKey(int compoundKey);
        AductMassSpectraViewModel GetByKey(int key);
        int Add(AductMassSpectraViewModel entityVM);
        int Update(AductMassSpectraViewModel entityVM);
        void Delete(int key);
    }
}