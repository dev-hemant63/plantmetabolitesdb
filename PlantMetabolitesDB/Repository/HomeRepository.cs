using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly PlantMetabolitesDBContext _context;

        private bool disposed = false;
        public HomeRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<AvailableDatabaseViewModel> GetAvailableDatabase()
        {
            var result = (from d in _context.Master_Databases.Where(d => d.IsActive == true)
                          join s in _context.Master_Compounds.Where(d => d.IsActive == true)
                          on d.DatabaseKey equals s.DatabaseKey into g
                          select new AvailableDatabaseViewModel
                          {
                              DatabaseName = d.DatabaseName.Trim(),
                              Count = g.Count()
                          }).ToList();

            return result.OrderBy(d => d.DatabaseName);
        }

        public IEnumerable<NewsTickerViewModel> GetNewsTicker()
        {
            var result = (from d in _context.Master_Tickers
                          where d.IsActive == true
                          select new NewsTickerViewModel
                          {
                              TickerKey = d.TickerKey,
                              Title = d.Title,
                              FileName = d.UploadedFileName
                          }).ToList();

            return result.OrderByDescending(d => d.TickerKey);
        }

        public IEnumerable<DownloadViewModel> GetDownloads()
        {
            var result = (from d in _context.Master_LatestUpdatess
                          where d.IsActive == true
                          select new DownloadViewModel
                          {
                              DownloadKey = d.LatestUpdatesKey,
                              Title = d.Title,
                              FileName = d.UploadedFileName
                          }).ToList();

            return result.OrderByDescending(d => d.DownloadKey).Take(3);
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

    public interface IHomeRepository : IDisposable
    {
        IEnumerable<AvailableDatabaseViewModel> GetAvailableDatabase();
        IEnumerable<NewsTickerViewModel> GetNewsTicker();
        IEnumerable<DownloadViewModel> GetDownloads();
    }
}