using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class MS2MassSpectraRepository : IMS2MassSpectraRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        GeneralClass gls = new GeneralClass();
        private bool disposed = false;
        public MS2MassSpectraRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<MS2MassSpectraViewModel> GetAll()
        {
            return _context.Master_MS2MassSpectras.Select(d => new MS2MassSpectraViewModel
            {
                MS2MassSpectraKey = d.MS2MassSpectraKey,
                InstrumentKey = d.InstrumentKey,
                Polarity = d.Polarity,
                ParentIon = d.ParentIon,
                AnnotationKey = d.AnnotationKey,
                CompoundKey = d.CompoundKey,
                SpectraDescription = d.SpectraDescription,
                MS2DataFile = d.MS2DataFile,
                MS2RefFile = d.MS2RefFile,
                MS2SpectraFile = d.MS2SpectraFile,
                CreatedBy = d.CreatedBy,
                CreatedOn = d.CreatedOn,
                LastModifiedBy = d.LastModifiedBy,
                LastModifiedOn = d.LastModifiedOn,

            }).OrderByDescending(d => d.MS2MassSpectraKey).ToList();
        }

        public IEnumerable<MS2MassSpectraViewModel> GetAllByInstrumentKey(int instKey, int compKey)
        {
            var lstEntity = (from ms in _context.Master_MS2MassSpectras
                             where ms.InstrumentKey == instKey && ms.CompoundKey == compKey
                             join an in _context.Master_Annotations on ms.AnnotationKey equals an.AnnotationKey
                             select new MS2MassSpectraViewModel
                             {
                                 MS2MassSpectraKey = ms.MS2MassSpectraKey,
                                 InstrumentKey = ms.InstrumentKey,
                                 Polarity = ms.Polarity,
                                 ParentIon = ms.ParentIon,
                                 AnnotationKey = ms.AnnotationKey,
                                 CompoundKey = ms.CompoundKey,
                                 SpectraDescription = ms.SpectraDescription,
                                 MS2DataFile = ms.MS2DataFile,
                                 MS2RefFile = ms.MS2RefFile,
                                 MS2SpectraFile = ms.MS2SpectraFile,
                                 CreatedBy = ms.CreatedBy,
                                 CreatedOn = ms.CreatedOn,
                                 LastModifiedBy = ms.LastModifiedBy,
                                 LastModifiedOn = ms.LastModifiedOn,
                                 AnnotationName = an.AnnotationName,
                                 
                             }).OrderByDescending(d => d.MS2MassSpectraKey).ToList();

            lstEntity.ForEach(y => y.PolarityName = gls.GetPolarity().Where(d => d.Value == y.Polarity.ToString()).FirstOrDefault().Text);
            return lstEntity;
        }

        public MS2MassSpectraViewModel GetByKey(int key)
        {
            return _context.Master_MS2MassSpectras.Where(x => x.MS2MassSpectraKey == key)
                .Select(d => new MS2MassSpectraViewModel
                {
                    MS2MassSpectraKey = d.MS2MassSpectraKey,
                    InstrumentKey = d.InstrumentKey,
                    Polarity = d.Polarity,
                    ParentIon = d.ParentIon,
                    AnnotationKey = d.AnnotationKey,
                    CompoundKey = d.CompoundKey,
                    SpectraDescription = d.SpectraDescription,
                    MS2DataFile = d.MS2DataFile,
                    MS2RefFile = d.MS2RefFile,
                    MS2SpectraFile = d.MS2SpectraFile,
                    CreatedBy = d.CreatedBy,
                    CreatedOn = d.CreatedOn,
                    LastModifiedBy = d.LastModifiedBy,
                    LastModifiedOn = d.LastModifiedOn,

                }).FirstOrDefault();
        }

        public IEnumerable<MS2MassSpectraViewModel> GetByCompoundKey(int compoundKey)
        {
            return _context.Master_MS2MassSpectras.Where(x => x.CompoundKey == compoundKey)
                .Select(d => new MS2MassSpectraViewModel
                {
                    MS2MassSpectraKey = d.MS2MassSpectraKey,
                    InstrumentKey = d.InstrumentKey,
                    Polarity = d.Polarity,
                    ParentIon = d.ParentIon,
                    AnnotationKey = d.AnnotationKey,
                    CompoundKey = d.CompoundKey,
                    SpectraDescription = d.SpectraDescription,
                    MS2DataFile = d.MS2DataFile,
                    MS2RefFile = d.MS2RefFile,
                    MS2SpectraFile = d.MS2SpectraFile,
                }).ToList();
        }

        public int Add(MS2MassSpectraViewModel entityVM)
        {
            int result = -1;
            if (entityVM.InstrumentKey > 0 && entityVM.CompoundKey > 0 && entityVM.Polarity > 0 && entityVM.AnnotationKey > 0 && entityVM.ParentIon != null)
            {
                Master_MS2MassSpectra objEntity = new Master_MS2MassSpectra();
                objEntity.Polarity = entityVM.Polarity;
                objEntity.AnnotationKey = entityVM.AnnotationKey;
                objEntity.InstrumentKey = entityVM.InstrumentKey;
                objEntity.ParentIon = entityVM.ParentIon;
                objEntity.CompoundKey = entityVM.CompoundKey;
                objEntity.SpectraDescription = entityVM.SpectraDescription;
                objEntity.MS2DataFile = entityVM.MS2DataFile;
                objEntity.MS2RefFile = entityVM.MS2RefFile;
                objEntity.MS2SpectraFile = entityVM.MS2SpectraFile;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.Date.AddMinutes(330);

                _context.Master_MS2MassSpectras.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.CompoundKey;
                Int16 massSpectraKey = objEntity.MS2MassSpectraKey;
                if(result > 0)
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

        public int Update(MS2MassSpectraViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_MS2MassSpectra objEntity = _context.Master_MS2MassSpectras.Find(entityVM.MS2MassSpectraKey);
                if (objEntity != null)
                {
                    objEntity.Polarity = entityVM.Polarity;
                    objEntity.AnnotationKey = entityVM.AnnotationKey;
                    objEntity.InstrumentKey = entityVM.InstrumentKey;
                    objEntity.ParentIon = entityVM.ParentIon;
                    objEntity.CompoundKey = entityVM.CompoundKey;
                    objEntity.SpectraDescription = entityVM.SpectraDescription;
                    objEntity.MS2DataFile = entityVM.MS2DataFile;
                    objEntity.MS2RefFile = entityVM.MS2RefFile;
                    objEntity.MS2SpectraFile = entityVM.MS2SpectraFile;
                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);
                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.MS2MassSpectraKey;
            }
            return result;
        }

        public void Delete(int key)
        {
            Master_MS2MassSpectra objEntity = _context.Master_MS2MassSpectras.Find(key);
            _context.Master_MS2MassSpectras.Remove(objEntity);
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
    public interface IMS2MassSpectraRepository : IDisposable
    {
        IEnumerable<MS2MassSpectraViewModel> GetAll();
        IEnumerable<MS2MassSpectraViewModel> GetAllByInstrumentKey(int instKey, int compKey);
        IEnumerable<MS2MassSpectraViewModel> GetByCompoundKey(int compoundKey);
        MS2MassSpectraViewModel GetByKey(int key);
        int Add(MS2MassSpectraViewModel entityVM);
        int Update(MS2MassSpectraViewModel entityVM);
        void Delete(int key);
    } 

}