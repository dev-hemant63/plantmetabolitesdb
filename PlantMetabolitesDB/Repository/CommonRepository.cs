using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class CommonRepository: ICommonRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        public CommonRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<Master_Country> GetAllCountry()
        {
            return _context.Master_Countrys.OrderBy(d=>d.CountryName).ToList();
                        
        }

        public CompoundStats CompoundStats()
        {
            CompoundStats model=new CompoundStats();
            model.CompoundCount = _context.Master_Compounds.Where(d => d.IsActive == true).Count();
            model.MS2Count = _context.Master_MS2MassSpectras.Count();
            model.MS3Count = _context.Master_MS3MassSpectras.Count();
            model.AductCount = _context.Master_AductMassSpectras.Count();

            return model;

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

    public interface ICommonRepository : IDisposable
    {
        IEnumerable<Master_Country> GetAllCountry();
        CompoundStats CompoundStats();
      
    }

}