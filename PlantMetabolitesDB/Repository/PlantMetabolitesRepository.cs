using BotDetect.C5;
using PlantMetabolitesDB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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


        int AddMS1MassSpectra(Master_MS1MassSpectra entity);
        int UpdateMS1MassSpectra(Master_MS1MassSpectra entity);
        IEnumerable<Master_MS1MassSpectra> GetMS1MassSpectraList();
        int DeleteMS1Record(int MS1MassSpectraKey);
        Master_MS1MassSpectra GetMS1MassSpectraById(int MS1MassSpectraKey);
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
                _context.Master_MS1MassSpectra.RemoveRange(
                   _context.Master_MS1MassSpectra.Where(c => c.PlantMetabolitesKey == entity.PlantMetabolitesKey));


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



        public int AddMS1MassSpectra(Master_MS1MassSpectra entity)
        {
            _context.Master_MS1MassSpectra.Add(entity);
            try
            {
                return _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.InnerException?.Message;
                return 0;
            }
        }
        public int UpdateMS1MassSpectra(Master_MS1MassSpectra entity)
        {
            if (entity.MS1MassSpectraKey == 0)
            {
                return 0;
            }
            
            try
            {
                var existing_entity = _context.Master_MS1MassSpectra
                  .Include(m => m.Master_Annotation)
                  .Include(m => m.PlantMetabolites)
                  .Where(x => x.MS1MassSpectraKey == entity.MS1MassSpectraKey)
                  .FirstOrDefault();

                existing_entity.InstrumentKey = entity.InstrumentKey;
                existing_entity.Polarity = entity.Polarity;
                existing_entity.AnnotationKey = entity.AnnotationKey;
                existing_entity.PartsOfPlant = entity.PartsOfPlant;
                existing_entity.SampleType = entity.SampleType;
                existing_entity.TaxonomistName = entity.TaxonomistName;
                existing_entity.VoucherNo = entity.VoucherNo;
                existing_entity.HerbariumDepositedAt = entity.HerbariumDepositedAt;
                existing_entity.DateOfCollection = entity.DateOfCollection;
                existing_entity.GeoLocation = entity.GeoLocation;
                existing_entity.SpectrumAveraging = entity.SpectrumAveraging;
                existing_entity.HPLCUPLCMethodeFilePath = string.IsNullOrEmpty(entity.HPLCUPLCMethodeFilePath) ? existing_entity.HPLCUPLCMethodeFilePath : entity.HPLCUPLCMethodeFilePath;
                existing_entity.ExtractionMethodeFilePath = string.IsNullOrEmpty(entity.ExtractionMethodeFilePath) ? existing_entity.ExtractionMethodeFilePath : entity.ExtractionMethodeFilePath;
                existing_entity.FingerprintFilePath = string.IsNullOrEmpty(entity.FingerprintFilePath) ? existing_entity.FingerprintFilePath : entity.FingerprintFilePath;
                existing_entity.MS1RefFilePath = string.IsNullOrEmpty(entity.MS1RefFilePath) ? existing_entity.MS1RefFilePath : entity.MS1RefFilePath;
                existing_entity.MS1RefFilePath = string.IsNullOrEmpty(entity.MS1RefFilePath) ? existing_entity.MS1RefFilePath : entity.MS1RefFilePath;
                existing_entity.LastModifiedBy = entity.LastModifiedBy;
                existing_entity.LastModifiedOn = entity.LastModifiedOn;

                return _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.InnerException?.Message;
                return 0;
            }
        }
        public IEnumerable<Master_MS1MassSpectra> GetMS1MassSpectraList()
        {
            try
            {
                var list = _context.Master_MS1MassSpectra
                  .Include(m => m.Master_Annotation)
                  .Include(m => m.PlantMetabolites)
                  .ToList();
                return list;
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.InnerException?.Message;
                return default(IEnumerable<Master_MS1MassSpectra>);
            }
        }
        public int DeleteMS1Record(int MS1MassSpectraKey)
        {
            var entity = _context.Master_MS1MassSpectra.Where(x => x.MS1MassSpectraKey == MS1MassSpectraKey).FirstOrDefault();
            _context.Master_MS1MassSpectra.Remove(entity);
            int result = _context.SaveChanges();
            return result;
        }
        public Master_MS1MassSpectra GetMS1MassSpectraById(int MS1MassSpectraKey)
        {
            try
            {
                var data = _context.Master_MS1MassSpectra
                  .Include(m => m.Master_Annotation)
                  .Include(m => m.PlantMetabolites)
                  .Where(x => x.MS1MassSpectraKey == MS1MassSpectraKey)
                  .FirstOrDefault();

                return data;
            }
            catch (Exception ex)
            {
                return default(Master_MS1MassSpectra);
            }
        }
    }
}