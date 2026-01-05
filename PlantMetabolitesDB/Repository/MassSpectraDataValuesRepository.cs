using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class MassSpectraDataValuesRepository : IMassSpectraDataValuesRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        private bool disposed = false;
        public MassSpectraDataValuesRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<MassSpectraDataValuesViewModel> GetMassSpectraDataValuesByKey(int massSpectraKey)
        {
            var result= _context.MassSpectra_DataValuess.Where(x => x.MassSpectraKey == massSpectraKey)
                .Select(d => new MassSpectraDataValuesViewModel
                {
                    MassSpectraKey = d.MassSpectraKey,
                    mz = d.mz,
                    relative = d.relative,
                    mz_int = d.mz_int,
                    relative_int = d.relative_int

                }).ToList();

            return result;
        }

        public IEnumerable<MassSpectraDataValuesViewModel> GetMassSpectraDataValuesByKey(int massSpectraKey, int massspectraType)
        {
            var result= _context.MassSpectra_DataValuess.Where(x => x.MassSpectraKey == massSpectraKey && x.MassSpectraType == massspectraType)
                .Select(d => new MassSpectraDataValuesViewModel
                {
                    MassSpectraKey = d.MassSpectraKey,
                    mz = d.mz,
                    relative = d.relative,
                    mz_int = d.mz_int,
                    relative_int = d.relative_int

                }).ToList();

            return result;
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

    public interface IMassSpectraDataValuesRepository : IDisposable
    {
        IEnumerable<MassSpectraDataValuesViewModel> GetMassSpectraDataValuesByKey(int massSpectraKey);
        IEnumerable<MassSpectraDataValuesViewModel> GetMassSpectraDataValuesByKey(int massSpectraKey, int massspectraType);
    }
}