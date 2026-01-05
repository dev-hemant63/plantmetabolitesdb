using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Xml.Linq;
using PlantMetabolitesDB.Models;
using PlantMetabolitesDB.ViewModel;
using static PlantMetabolitesDB.Models.GeneralClass;

namespace PlantMetabolitesDB.Repository
{
    public class CompoundRepository : ICompoundRepository
    {
        private readonly PlantMetabolitesDBContext _context;

        private bool disposed = false;
        public CompoundRepository(PlantMetabolitesDBContext context)
        {
            this._context = context;
        }

        public IEnumerable<CompoundViewModel> GetAll()
        {
            return _context.Master_Compounds.Select(d => new CompoundViewModel
            {
                CompoundKey = d.CompoundKey,
                CASNo = d.CASNo,
                ChemicalStructureFile = d.ChemicalStructureFile,
                CompiledBy = d.CompiledBy,
                CompoundName = d.CompoundName,
                CreatedBy = d.CreatedBy,
                CreatedOn = d.CreatedOn,
                IUPACName = d.IUPACName,
                DatabaseKey = d.DatabaseKey,
                IsActive = d.IsActive,
                DataSheetFile = d.DataSheetFile,
                Formula = d.Formula,
                MSnITFragmentationFile = d.MSnITFragmentationFile,
                SchemeofFragmentationFile = d.SchemeofFragmentationFile,
                LastModifiedBy = d.LastModifiedBy,
                LastModifiedOn = d.LastModifiedOn,
                MolecularWeight = d.MolecularWeight,
                OtherNames = d.OtherNames,
                MolecularWeight_Input = d.MolecularWeight_Input,
                Smiles = d.Smiles,
                DatabaseName = d.Databases.DatabaseName,
                StatusName = d.IsActive == true ? "Active" : "InActive"
            }).ToList();
        }

        public CompoundViewModel GetByKey(int key)
        {
            return _context.Master_Compounds.Where(d => d.CompoundKey == key).Select(d => new CompoundViewModel
            {
                CompoundKey = d.CompoundKey,
                CASNo = d.CASNo,
                ChemicalStructureFile = d.ChemicalStructureFile,
                CompiledBy = d.CompiledBy,
                CompoundName = d.CompoundName,
                CreatedBy = d.CreatedBy,
                CreatedOn = d.CreatedOn,
                IUPACName = d.IUPACName,
                DatabaseKey = d.DatabaseKey,
                IsActive = d.IsActive,
                DataSheetFile = d.DataSheetFile,
                Formula = d.Formula,
                MSnITFragmentationFile = d.MSnITFragmentationFile,
                SchemeofFragmentationFile = d.SchemeofFragmentationFile,
                LastModifiedBy = d.LastModifiedBy,
                LastModifiedOn = d.LastModifiedOn,
                MolecularWeight = d.MolecularWeight,
                OtherNames = d.OtherNames,
                MolecularWeight_Input = d.MolecularWeight_Input,
                Smiles = d.Smiles,
                DatabaseName = d.Databases.DatabaseName,
                //MS2Count = _context.Master_MS2MassSpectras.Where(c => c.CompoundKey == d.CompoundKey).Count(),
                //MS3Count = _context.Master_MS3MassSpectras.Where(c => c.CompoundKey == d.CompoundKey).Count(),
                //AductCount = _context.Master_AductMassSpectras.Where(c => c.CompoundKey == d.CompoundKey).Count(),
                MS2Count = _context.Master_MS2MassSpectras.Count(c => c.CompoundKey == d.CompoundKey),
                MS3Count = _context.Master_MS3MassSpectras.Count(c => c.CompoundKey == d.CompoundKey),
                AductCount = _context.Master_AductMassSpectras.Count(c => c.CompoundKey == d.CompoundKey),
                StatusName = d.IsActive == true ? "Active" : "InActive"
            }).FirstOrDefault();
        }
        public int Add(CompoundViewModel entityVM)
        {
            int result = -1;
            if (entityVM.CompoundName != null)
            {
                Master_Compound objEntity = new Master_Compound();
                objEntity.CASNo = entityVM.CASNo;
                objEntity.ChemicalStructureFile = entityVM.ChemicalStructureFile;
                objEntity.CompiledBy = entityVM.CompiledBy;
                objEntity.CompoundName = entityVM.CompoundName;
                objEntity.IUPACName = entityVM.IUPACName;
                objEntity.DatabaseKey = entityVM.DatabaseKey;
                objEntity.DataSheetFile = entityVM.DataSheetFile;
                objEntity.Formula = entityVM.Formula;
                objEntity.MSnITFragmentationFile = entityVM.MSnITFragmentationFile;
                objEntity.SchemeofFragmentationFile = entityVM.SchemeofFragmentationFile;
                objEntity.MolecularWeight = Convert.ToInt16(0);
                objEntity.OtherNames = entityVM.OtherNames;
                objEntity.MolecularWeight_Input = entityVM.MolecularWeight_Input;
                objEntity.Smiles = entityVM.Smiles;
                objEntity.IsActive = true;
                objEntity.CreatedBy = entityVM.CreatedBy;
                objEntity.CreatedOn = DateTime.UtcNow.Date.AddMinutes(330);

                _context.Master_Compounds.Add(objEntity);
                _context.SaveChanges();
                result = objEntity.CompoundKey;
            }
            return result;

        }
        public int Update(CompoundViewModel entityVM)
        {
            int result = -1;
            if (entityVM != null)
            {
                Master_Compound objEntity = _context.Master_Compounds.Find(entityVM.CompoundKey);
                if (objEntity != null)
                {
                    objEntity.CASNo = entityVM.CASNo;
                    objEntity.ChemicalStructureFile = entityVM.ChemicalStructureFile;
                    objEntity.CompiledBy = entityVM.CompiledBy;
                    objEntity.CompoundName = entityVM.CompoundName;
                    objEntity.IUPACName = entityVM.IUPACName;
                    objEntity.DatabaseKey = entityVM.DatabaseKey;
                    objEntity.DataSheetFile = entityVM.DataSheetFile;
                    objEntity.Formula = entityVM.Formula;
                    objEntity.MSnITFragmentationFile = entityVM.MSnITFragmentationFile;
                    objEntity.SchemeofFragmentationFile = entityVM.SchemeofFragmentationFile;
                    //objEntity.MolecularWeight = entityVM.MolecularWeight_Input == "" ? Convert.ToInt16(0) : Convert.ToInt16(entityVM.MolecularWeight_Input);
                    objEntity.OtherNames = entityVM.OtherNames;
                    objEntity.MolecularWeight_Input = entityVM.MolecularWeight_Input;
                    objEntity.Smiles = entityVM.Smiles;

                    objEntity.LastModifiedBy = entityVM.LastModifiedBy;
                    objEntity.LastModifiedOn = DateTime.UtcNow.AddMinutes(330);

                    _context.Entry(objEntity).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                result = objEntity.CompoundKey;
            }
            return result;
        }
        public void Delete(int key)
        {
            Master_Compound objEntity = _context.Master_Compounds.Find(key);
            _context.Master_Compounds.Remove(objEntity);
            _context.SaveChanges();
        }
        public void ActivateDeActivate(int key)
        {
            Master_Compound objEntity = _context.Master_Compounds.Find(key);
            if (objEntity != null)
            {
                objEntity.IsActive = objEntity.IsActive ? false : true;
                _context.Entry(objEntity).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public StringBuilder LoadXML(String compoundName)
        {
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
            System.Text.StringBuilder sbInternal = new System.Text.StringBuilder();

            System.Text.StringBuilder sbIdList = new System.Text.StringBuilder();
            String tempStr = "", authorList = "";
            Int16 cntArticle = 1;
            XDocument xdoc = XDocument.Load("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&term=" + HttpContext.Current.Server.UrlEncode(compoundName) + "[Title]");
            //XDocument xdoc = XDocument.Load(@"C:\Users\ATUL\Desktop\CDRI\XML-DATA\1.xml");
            foreach (XElement element in xdoc.Descendants("Id"))
                sbIdList.Append(element + ",");

            if (sbIdList.Length != 0)
            {
                sbIdList.Remove(sbIdList.Length - 1, 1);

                XDocument xdoc2 = XDocument.Load("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=" + sbIdList.ToString() + "&retmode=xml");
                //XDocument xdoc2 = XDocument.Load(@"C:\Users\ATUL\Desktop\CDRI\XML-DATA\2.xml");

                foreach (XElement elePubmedArticle in xdoc2.Descendants("PubmedArticle"))
                {
                    XElement eleArticle = elePubmedArticle.Element("MedlineCitation").Element("Article");

                    String strPMID = elePubmedArticle.Element("MedlineCitation").Element("PMID").Value;
                    sbInternal.Append("<tr><td valign='top'>" + (cntArticle++) + ".</td><td><b><a href='https://www.ncbi.nlm.nih.gov/pubmed/" + strPMID + "' target='_blank' title='Article Details'>" + eleArticle.Element("ArticleTitle").Value + "</a><b><td></tr>");

                    // AuthorList
                    authorList = "";
                    tempStr = "";

                    XElement eleAuthorList = eleArticle.Element("AuthorList");
                    authorList = ("<tr><td></td><td><div class='' style='width: 100%; margin-top: 3px;'><div style='padding:2px 7px 2px 7px;background-color:#e5e5e5'>");
                    authorList += ("<font style='font-size: 13px;'>");
                    foreach (XElement elementAuthor in eleArticle.Descendants("AuthorList").Descendants("Author"))
                    {
                        if (elementAuthor.Element("LastName") != null && elementAuthor.Element("Initials") != null)
                        {
                            tempStr += elementAuthor.Element("LastName").Value;
                            tempStr += " " + elementAuthor.Element("Initials").Value + ", ";
                        }

                    }
                    if (!String.IsNullOrEmpty(tempStr))
                        authorList += ("<i><b>Author(s)</b> : " + tempStr.Substring(0, tempStr.Length - 2) + "</i>");
                    // AuthorList


                    sbInternal.Append(authorList);

                    XElement eleMedlineJournalInfo = elePubmedArticle.Element("MedlineCitation").Element("MedlineJournalInfo");
                    if (eleMedlineJournalInfo != null)
                        sbInternal.Append("<br><i><b>Journal Info : </b>" + eleMedlineJournalInfo.Element("MedlineTA") + "</i>");

                    XElement eleDateCreated = elePubmedArticle.Element("MedlineCitation").Element("DateCreated");
                    if (eleDateCreated != null)
                        sbInternal.Append(" <i><b>" + ((MonthName)Convert.ToInt16(eleDateCreated.Element("Month").Value)).ToString() + " " + eleDateCreated.Element("Day") + ", " + eleDateCreated.Element("Year") + "</b></i>");


                    if (eleArticle.Element("Affiliation") != null)
                        sbInternal.Append("<br><i><b>Affiliation</b> : " + eleArticle.Element("Affiliation") + "</i>");

                    sbInternal.Append("</font>");
                    sbInternal.Append("</div></div></td></tr>");
                    sbInternal.Append("<tr><td height='10px' colspan='2'></td></tr>");
                }
                //lblRef.Text = sbInternal.ToString();
                //if (sbInternal.Length != 0)
                //    rwReferences.Visible = true;\
            }

            return sbInternal;
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

    public interface ICompoundRepository : IDisposable
    {
        IEnumerable<CompoundViewModel> GetAll();
        CompoundViewModel GetByKey(int key);
        int Add(CompoundViewModel entityVM);
        int Update(CompoundViewModel entityVM);
        void Delete(int key);
        void ActivateDeActivate(int key);
        StringBuilder LoadXML(String compoundName);
    }
}