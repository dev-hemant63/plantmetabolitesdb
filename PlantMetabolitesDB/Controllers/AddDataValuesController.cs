using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Interop;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.Repository;

namespace PlantMetabolitesDB.Controllers
{
    public class AddDataValuesController : Controller
    {
        private IMS2MassSpectraRepository _ms2MassSpectraRepository;
        private IMS3MassSpectraRepository _ms3MassSpectraRepository;
        private IAductMassSpectraRepository _aductMassSpectraRepository;
        private IMassSpectraDataValuesRepository _massSpectraDataValuesRepository;
        private PlantMetabolitesDBContext _context;

        public AddDataValuesController()
        {
            
            _ms2MassSpectraRepository = new MS2MassSpectraRepository(new PlantMetabolitesDBContext());
            _ms3MassSpectraRepository = new MS3MassSpectraRepository(new PlantMetabolitesDBContext());
            _aductMassSpectraRepository = new AductMassSpectraRepository(new PlantMetabolitesDBContext());
            _massSpectraDataValuesRepository = new MassSpectraDataValuesRepository(new PlantMetabolitesDBContext());
        }
        public AddDataValuesController(
            IMS2MassSpectraRepository ms2MassSpectraRepository,
            IMS3MassSpectraRepository ms3MassSpectraRepository,
            IAductMassSpectraRepository aductMassSpectraRepository,
            IMassSpectraDataValuesRepository massSpectraDataValuesRepository)
        {
            
            _ms2MassSpectraRepository = ms2MassSpectraRepository;
            _ms3MassSpectraRepository = ms3MassSpectraRepository;
            _aductMassSpectraRepository = aductMassSpectraRepository;
            _massSpectraDataValuesRepository = massSpectraDataValuesRepository;
            
        }

        // GET: AddDataValues
        public ActionResult Index()
        {
            _context = new PlantMetabolitesDBContext();
            //var ms2 = _ms2MassSpectraRepository.GetAll().OrderBy(d => d.MS2MassSpectraKey);
            var ms3 = _ms3MassSpectraRepository.GetAll().OrderBy(d => d.MS3MassSpectraKey); 
            //var aduct = _aductMassSpectraRepository.GetAll().OrderBy(d => d.AductMassSpectraKey);

            foreach (var item in ms3)
            {
                string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS3/DataFile"), item.MS3DataFile);
                Int16 massSpectraKey = item.MS3MassSpectraKey;

                List<MassSpectra_DataValues> lstMassSpectra_DataValues = new List<MassSpectra_DataValues>();
                lstMassSpectra_DataValues = GeneralClass.GetMassSpectra_DataValues(filePath, 3);

                foreach (var entityMSDV in lstMassSpectra_DataValues)
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

            //foreach (var item in aduct)
            //{
            //    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/Aduct/DataFile"), item.AductDataFile);
            //    Int16 massSpectraKey = item.AductMassSpectraKey;

            //    List<MassSpectra_DataValues> lstMassSpectra_DataValues = new List<MassSpectra_DataValues>();
            //    lstMassSpectra_DataValues = GeneralClass.GetMassSpectra_DataValues(filePath, 2);

            //    foreach (var entityMSDV in lstMassSpectra_DataValues)
            //    {
            //        MassSpectra_DataValues obj = new MassSpectra_DataValues();
            //        obj.MassSpectraKey = massSpectraKey;
            //        obj.MassSpectraType = entityMSDV.MassSpectraType;
            //        obj.absolute = entityMSDV.absolute;
            //        obj.relative = entityMSDV.relative;
            //        obj.relative_int = entityMSDV.relative_int;
            //        obj.mz = entityMSDV.mz;
            //        obj.mz_int = entityMSDV.mz_int;
            //        obj.absolute_int = entityMSDV.absolute_int;
            //        _context.MassSpectra_DataValuess.Add(obj);
            //        _context.SaveChanges();
            //    }
            //}

            //foreach (var item in ms2)
            //{
            //    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/MS2/DataFile"), item.MS2DataFile);
            //    Int16 massSpectraKey = item.MS2MassSpectraKey;

            //    List<MassSpectra_DataValues> lstMassSpectra_DataValues = new List<MassSpectra_DataValues>();
            //    lstMassSpectra_DataValues = GeneralClass.GetMassSpectra_DataValues(filePath, 1);

            //    foreach (var entityMSDV in lstMassSpectra_DataValues)
            //    {
            //        MassSpectra_DataValues obj = new MassSpectra_DataValues();
            //        obj.MassSpectraKey = massSpectraKey;
            //        obj.MassSpectraType = entityMSDV.MassSpectraType;
            //        obj.absolute = entityMSDV.absolute;
            //        obj.relative = entityMSDV.relative;
            //        obj.relative_int = entityMSDV.relative_int;
            //        obj.mz = entityMSDV.mz;
            //        obj.mz_int = entityMSDV.mz_int;
            //        obj.absolute_int = entityMSDV.absolute_int;
            //        _context.MassSpectra_DataValuess.Add(obj);
            //        _context.SaveChanges();
            //    }
            //}


            return View();
        }
    }
}