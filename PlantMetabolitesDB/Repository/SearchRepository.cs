using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;

namespace PlantMetabolitesDB.Repository
{
    public class SearchRepository : ISearchRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        private bool disposed = false;
        public SearchRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<GeneralSearchResultViewModel> GetGeneralSearchResult(GeneralSearchViewModel entity)
        {

            IEnumerable<GeneralSearchResultViewModel> lstEntity = new List<GeneralSearchResultViewModel>();
            if (entity.CompoundName != null)
            {
                lstEntity = (from c in _context.Master_Compounds
                              where c.IsActive == true && c.CompoundName.Contains(entity.CompoundName)
                              select new GeneralSearchResultViewModel
                              {
                                  CompoundKey = c.CompoundKey,
                                  CompoundName = c.CompoundName,
                                  MolecularWeight = c.MolecularWeight_Input,
                                  Formula = c.Formula
                              });
                
            }
            else if (entity.CASNo != null)
            {
                lstEntity = (from c in _context.Master_Compounds
                             where c.IsActive == true && c.CASNo.Contains(entity.CASNo)
                             select new GeneralSearchResultViewModel
                             {
                                 CompoundKey = c.CompoundKey,
                                 CompoundName = c.CompoundName,
                                 MolecularWeight = c.MolecularWeight_Input,
                                 Formula = c.Formula
                             });
            }
            else if (entity.MolecularWeight != null)
            {
                lstEntity = (from c in _context.Master_Compounds
                             where c.IsActive == true && c.MolecularWeight_Input.Contains(entity.MolecularWeight)
                             select new GeneralSearchResultViewModel
                             {
                                 CompoundKey = c.CompoundKey,
                                 CompoundName = c.CompoundName,
                                 MolecularWeight = c.MolecularWeight_Input,
                                 Formula = c.Formula
                             });
            }
            else if (entity.Formula != null)
            {
                lstEntity = (from c in _context.Master_Compounds
                             where c.IsActive == true && c.Formula.Contains(entity.Formula)
                             select new GeneralSearchResultViewModel
                             {
                                 CompoundKey = c.CompoundKey,
                                 CompoundName = c.CompoundName,
                                 MolecularWeight = c.MolecularWeight_Input,
                                 Formula = c.Formula
                             });
            }

            return lstEntity;
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

    public interface ISearchRepository : IDisposable
    {
        IEnumerable<GeneralSearchResultViewModel> GetGeneralSearchResult(GeneralSearchViewModel entity);
    }
}