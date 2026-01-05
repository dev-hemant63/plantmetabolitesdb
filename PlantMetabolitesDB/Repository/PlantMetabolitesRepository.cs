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
        PlantMetabolitesVM GetById(int id);
        int Add(PlantMetabolitesVM entity);
        int Update(PlantMetabolitesVM entity);
        int Delete(int PlantMetabolitesKey);


        int AddMS1MassSpectra(MasterMS1MassSpectra entity);
    }
    public class PlantMetabolitesRepository : IPlantMetabolitesRepository
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
                    MajorConstituents = p.M07_Constituents.FirstOrDefault().Constituents,
                    PlantFamilyName = p.Master_Databases.DatabaseName
                }).OrderByDescending(x => x.PlantMetabolitesKey)
                .ToList();

            return list;
        }
        public PlantMetabolitesVM GetById(int id)
        {
            var data = _context.PlantMetabolites
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
                    MajorConstituents = p.M07_Constituents.FirstOrDefault().Constituents,
                    PlantFamilyName = p.Master_Databases.DatabaseName
                }).Where(x => x.PlantMetabolitesKey == id).FirstOrDefault();
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
                    PlantFamilyKey = Convert.ToInt16(entity.PlantFamilyKey),
                    DataSheetFile = entity.DataSheetFile,
                    IsActive = entity.IsActive,
                    CompiledBy = entity.CompiledBy?.ToString(),
                    Synonyms = entity.Synonyms,
                    CreatedBy = null,
                    CreatedOn = DateTime.Now,
                    Figure = entity.Figure
                };

                _context.PlantMetabolites.Add(plantMetabolite);
                _context.SaveChanges();

                int newId = plantMetabolite.PlantMetabolitesKey;

                if (!string.IsNullOrEmpty(entity.VernacularName))
                {
                    foreach (var name in entity.VernacularName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M02_VernacularName.Add(new M02_VernacularName
                        {
                            M01ID = newId,
                            VernacularName = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.SpecificBiologicalActivity))
                {
                    foreach (var name in entity.SpecificBiologicalActivity.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M03_BiologicalActivity.Add(new M03_BiologicalActivity
                        {
                            M01ID = newId,
                            BiologicalActivity = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.Distribution))
                {
                    foreach (var name in entity.Distribution.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M04_Distribution.Add(new M04_Distribution
                        {
                            M01ID = newId,
                            Distribution = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.EthnobotanicalInformation))
                {
                    foreach (var name in entity.EthnobotanicalInformation.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M05_EthnobotanicalInfo.Add(new M05_EthnobotanicalInfo
                        {
                            M01ID = newId,
                            EthnobotanicalInfo = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.ClassOfCompounds))
                {
                    foreach (var name in entity.ClassOfCompounds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M06_CompuondClass.Add(new M06_CompuondClass
                        {
                            M01ID = newId,
                            CompuondClass = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.MajorConstituents))
                {
                    foreach (var name in entity.MajorConstituents.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M07_Constituents.Add(new M07_Constituents
                        {
                            M01ID = newId,
                            Constituents = name.Trim(),
                            IsActive = true
                        });
                    }
                }


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
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var plantMetabolite = _context.PlantMetabolites
                    .FirstOrDefault(p => p.PlantMetabolitesKey == entity.PlantMetabolitesKey);

                if (plantMetabolite == null)
                    throw new Exception("Plant Metabolite not found.");

                plantMetabolite.PlantSpeciesName = entity.PlantSpeciesName;
                plantMetabolite.TaxonomistName = entity.TaxonomistName;
                plantMetabolite.PlantFamilyKey = Convert.ToInt16(entity.PlantFamilyKey);
                plantMetabolite.DataSheetFile = entity.DataSheetFile;
                plantMetabolite.IsActive = entity.IsActive;
                plantMetabolite.CompiledBy = entity.CompiledBy?.ToString();
                plantMetabolite.Synonyms = entity.Synonyms;
                plantMetabolite.Figure = entity.Figure;

                _context.M02_VernacularName.RemoveRange(
                    _context.M02_VernacularName.Where(v => v.M01ID == entity.PlantMetabolitesKey));
                _context.M03_BiologicalActivity.RemoveRange(
                    _context.M03_BiologicalActivity.Where(b => b.M01ID == entity.PlantMetabolitesKey));
                _context.M04_Distribution.RemoveRange(
                    _context.M04_Distribution.Where(d => d.M01ID == entity.PlantMetabolitesKey));
                _context.M05_EthnobotanicalInfo.RemoveRange(
                    _context.M05_EthnobotanicalInfo.Where(e => e.M01ID == entity.PlantMetabolitesKey));
                _context.M06_CompuondClass.RemoveRange(
                    _context.M06_CompuondClass.Where(c => c.M01ID == entity.PlantMetabolitesKey));
                _context.M07_Constituents.RemoveRange(
                    _context.M07_Constituents.Where(c => c.M01ID == entity.PlantMetabolitesKey));

                _context.SaveChanges();

                int id = entity.PlantMetabolitesKey;

                if (!string.IsNullOrEmpty(entity.VernacularName))
                {
                    foreach (var name in entity.VernacularName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M02_VernacularName.Add(new M02_VernacularName
                        {
                            M01ID = id,
                            VernacularName = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.SpecificBiologicalActivity))
                {
                    foreach (var name in entity.SpecificBiologicalActivity.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M03_BiologicalActivity.Add(new M03_BiologicalActivity
                        {
                            M01ID = id,
                            BiologicalActivity = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.Distribution))
                {
                    foreach (var name in entity.Distribution.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M04_Distribution.Add(new M04_Distribution
                        {
                            M01ID = id,
                            Distribution = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.EthnobotanicalInformation))
                {
                    foreach (var name in entity.EthnobotanicalInformation.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M05_EthnobotanicalInfo.Add(new M05_EthnobotanicalInfo
                        {
                            M01ID = id,
                            EthnobotanicalInfo = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.ClassOfCompounds))
                {
                    foreach (var name in entity.ClassOfCompounds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M06_CompuondClass.Add(new M06_CompuondClass
                        {
                            M01ID = id,
                            CompuondClass = name.Trim(),
                            IsActive = true
                        });
                    }
                }

                if (!string.IsNullOrEmpty(entity.MajorConstituents))
                {
                    foreach (var name in entity.MajorConstituents.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _context.M07_Constituents.Add(new M07_Constituents
                        {
                            M01ID = id,
                            Constituents = name.Trim(),
                            IsActive = true
                        });
                    }
                }

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
        public int Delete(int PlantMetabolitesKey)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var entity = _context.PlantMetabolites
                .Include(p => p.M02_VernacularName)
                .Include(p => p.M03_BiologicalActivity)
                .Include(p => p.M04_Distribution)
                .Include(p => p.M05_EthnobotanicalInfo)
                .Include(p => p.M06_CompuondClass)
                .Include(p => p.M07_Constituents).Where(x => x.PlantMetabolitesKey == PlantMetabolitesKey).FirstOrDefault();

                _context.PlantMetabolites.Remove(entity);

                _context.M02_VernacularName.RemoveRange(
                   _context.M02_VernacularName.Where(v => v.M01ID == entity.PlantMetabolitesKey));
                _context.M03_BiologicalActivity.RemoveRange(
                    _context.M03_BiologicalActivity.Where(b => b.M01ID == entity.PlantMetabolitesKey));
                _context.M04_Distribution.RemoveRange(
                    _context.M04_Distribution.Where(d => d.M01ID == entity.PlantMetabolitesKey));
                _context.M05_EthnobotanicalInfo.RemoveRange(
                    _context.M05_EthnobotanicalInfo.Where(e => e.M01ID == entity.PlantMetabolitesKey));
                _context.M06_CompuondClass.RemoveRange(
                    _context.M06_CompuondClass.Where(c => c.M01ID == entity.PlantMetabolitesKey));
                _context.M07_Constituents.RemoveRange(
                    _context.M07_Constituents.Where(c => c.M01ID == entity.PlantMetabolitesKey));


                int result = _context.SaveChanges();
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }



        public int AddMS1MassSpectra(MasterMS1MassSpectra entity)
        {
            _context.MasterMS1MassSpectra.Add(entity);
            return _context.SaveChanges();
        }
    }
}