using BotDetect.C5;
using PlantMetabolitesDB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using TandemDB.Models;
using TandemDB.ViewModel;

namespace TandemDB.Repository
{
    public interface IPlantMetabolitesRepository
    {
        IEnumerable<PlantMetabolitesVM> GetAll();
        PlantMetabolites GetById(int id);

        int Add(PlantMetabolitesVM entity);
        int Update(PlantMetabolitesVM entity);
    }
    public class PlantMetabolitesRepository: IPlantMetabolitesRepository
    {
        private readonly PlantMetabolitesDBContext _context;
        public PlantMetabolitesRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<PlantMetabolitesVM> GetAll()
        {
            var list = _context.PlantMetabolites
                .Include(p => p.M02_VernacularName)
                .Include(p => p.M03_BiologicalActivity)
                .Include(p => p.M04_Distribution)
                .Include(p => p.M05_EthnobotanicalInfo)
                .Include(p => p.M06_CompuondClass)
                .Include(p => p.M07_Constituents)
                .Select(p => new PlantMetabolitesVM
                {
                    PlantMetabolitesKey = p.PlantMetabolitesKey,
                    PlantSpeciesName = p.PlantSpeciesName,
                    TaxonomistName = p.TaxonomistName,
                    PlantFamilyKey = p.PlantFamilyKey.ToString(),
                    DataSheetFile = p.DataSheetFile,
                    IsActive = p.IsActive,
                    CompiledBy = p.CompiledBy,
                    Synonyms = p.Synonyms,
                    VernacularName = p.M02_VernacularName.FirstOrDefault().VernacularName,
                    SpecificBiologicalActivity = p.M03_BiologicalActivity.FirstOrDefault().BiologicalActivity,
                    Distribution = p.M04_Distribution.FirstOrDefault().Distribution,
                    EthnobotanicalInformation = p.M05_EthnobotanicalInfo.FirstOrDefault().EthnobotanicalInfo,
                    ClassOfCompounds = p.M06_CompuondClass.FirstOrDefault().CompuondClass,
                    MajorConstituents = p.M07_Constituents.FirstOrDefault().Constituents
                })
                .ToList();

            return list;
        }

        public PlantMetabolites GetById(int id)
        {
            var data = _context.PlantMetabolites
                       .Include(p => p.M02_VernacularName)
                       .Include(p => p.M03_BiologicalActivity)
                       .Include(p => p.M04_Distribution)
                       .Include(p => p.M05_EthnobotanicalInfo)
                       .Include(p => p.M06_CompuondClass)
                       .Include(p => p.M07_Constituents)
                       .Where(x=>x.PlantMetabolitesKey == id).FirstOrDefault();
            return data;
        }
        public int Add(PlantMetabolitesVM entity)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var plantMetabolite = new PlantMetabolites
                {
                    PlantSpeciesName = entity.PlantSpeciesName,
                    TaxonomistName = entity.TaxonomistName,
                    PlantFamilyKey = int.TryParse(entity.PlantFamilyKey, out int familyKey) ? familyKey : 0,
                    DataSheetFile = entity.DataSheetFile,
                    IsActive = entity.IsActive,
                    CompiledBy = entity.CompiledBy?.ToString(),
                    Synonyms = entity.Synonyms,
                    CreatedBy = null,
                    CreatedOn = DateTime.Now
                };

                _context.PlantMetabolites.Add(plantMetabolite);
                _context.SaveChanges();

                int newId = plantMetabolite.PlantMetabolitesKey;

                _context.M02_VernacularName.Add(new M02_VernacularName
                {
                    M01ID = newId,
                    VernacularName = entity.VernacularName,
                    IsActive = true
                });

                _context.M03_BiologicalActivity.Add(new M03_BiologicalActivity
                {
                    M01ID = newId,
                    BiologicalActivity = entity.SpecificBiologicalActivity,
                    IsActive = true
                });

                _context.M04_Distribution.Add(new M04_Distribution
                {
                    M01ID = newId,
                    Distribution = entity.Distribution,
                    IsActive = true
                });

                _context.M05_EthnobotanicalInfo.Add(new M05_EthnobotanicalInfo
                {
                    M01ID = newId,
                    EthnobotanicalInfo = entity.EthnobotanicalInformation,
                    IsActive = true
                });

                _context.M06_CompuondClass.Add(new M06_CompuondClass
                {
                    M01ID = newId,
                    CompuondClass = entity.ClassOfCompounds,
                    IsActive = true
                });

                _context.M07_Constituents.Add(new M07_Constituents
                {
                    M01ID = newId,
                    Constituents = entity.MajorConstituents,
                    IsActive = true
                });

                int result = _context.SaveChanges();

                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public int Update(PlantMetabolitesVM entity)
        {
            var existing = _context.PlantMetabolites.Find(entity.PlantMetabolitesKey);
            if (existing == null)
            {
                throw new Exception("Entity not found.");
            }

            _context.Entry(existing).CurrentValues.SetValues(entity);

            return _context.SaveChanges();
        }

    }
}