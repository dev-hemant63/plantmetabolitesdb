using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PlantMetabolitesDB.ViewModel;
using PlantMetabolitesDB.Models;
using System.Text;
using WebGrease.Activities;
using System.Runtime.InteropServices.ComTypes;

namespace PlantMetabolitesDB.Controllers
{
    public class DataFileFormatConvertorController : Controller
    {
        DataInput objDataInput = null;
        List<DataInput> lstDataInput = new List<DataInput>();

        DataInputValues objDataInputValues = null;
        List<DataInputValues> lstDataInputValues = new List<DataInputValues>();
        List<float> tempMZ = new List<float>();
        List<float> tempRI = new List<float>();
        List<float> srtRI = new List<float>();
        List<float> tempshortedMZ = new List<float>();

        List<float> tempMZneg = new List<float>();
        List<float> tempRIneg = new List<float>();
        List<float> srtRIneg = new List<float>();
        List<float> tempshortedMZneg = new List<float>();

        GeneralClass gls = new GeneralClass();

        // GET: DataFileFormatConvertor
        public ActionResult Index()
        {
            var model = new DataFileFormatViewModel();
            model.DataFormats = gls.GetDataFormats();
            model.TotalValues = gls.GetTotalValues();
            return View(model);
        }

        [HttpPost]
        public JsonResult GetDataFile(DataFileFormatViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadedImageFile != null && model.UploadedImageFile.ContentLength > 0)
                {
                    string fileName = model.UploadedImageFile.FileName;
                    string extension = Path.GetExtension(fileName);
                    var uniqueFileName = $@"{Guid.NewGuid()}" + extension + "";
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles/UserDataFiles"), uniqueFileName);
                    model.UploadedImageFile.SaveAs(filePath);

                    Session["TotalValue"] = model.TotalValue;
                    ReadDataFile(filePath, fileName, model.TotalValue);
                    ProcessData();
                    WriteDataToFile();

                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        // Read txt File for Convertion
        private void ReadDataFile2(string fileNameWithPath, string title, int totValue)
        {
            StreamReader reader = null;
            InputFormatType varInputFormatType = InputFormatType.NoFormat;
            String contents;
            int row = 0;

            try
            {
                using (reader = System.IO.File.OpenText(fileNameWithPath))
                {
                    while (reader.Peek() != -1)
                    {
                        contents = reader.ReadLine();
                        if (varInputFormatType == InputFormatType.NoFormat)
                        {
                            if (contents.StartsWith("BEGIN IONS", StringComparison.OrdinalIgnoreCase))
                            {
                                varInputFormatType = InputFormatType.MGF;
                            }
                            else if (contents.StartsWith("SPECTRUM - MS", StringComparison.OrdinalIgnoreCase))
                            {
                                varInputFormatType = InputFormatType.Xcalibur;
                            }
                            else if (contents.EndsWith("+") || contents.EndsWith("-"))
                            {
                                varInputFormatType = InputFormatType.TQD;
                            }
                            else if (contents.Contains("+") || contents.Contains("-"))
                            {
                                varInputFormatType = InputFormatType.AminoAcid;
                            }
                            else if (contents.StartsWith("START IONS", StringComparison.OrdinalIgnoreCase))
                            {
                                varInputFormatType = InputFormatType.tms;
                            }
                        }

                        switch (varInputFormatType)
                        {
                            case InputFormatType.MGF:
                                {
                                    //BEGIN IONS
                                    //TITLE=11I24ZODEC00212240644_130940.1006.1027.1.dta
                                    //CHARGE=1+
                                    //PEPMASS=427.07
                                    //120.17 2945.0
                                    //427.20 11005.0
                                    //427.48 9260.0
                                    //429.46 1152.0
                                    //END IONS

                                    if (contents.StartsWith("BEGIN IONS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (objDataInput != null)
                                        {
                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }

                                        objDataInput = new DataInput();
                                        lstDataInputValues = new List<DataInputValues>();
                                    }
                                    else if (contents.StartsWith("TITLE=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        string[] str = contents.Split(' ', '\t');

                                        for (int i = 0; i < str.Length; i++)
                                        {
                                            if (str[i].StartsWith("RT", StringComparison.OrdinalIgnoreCase))
                                            {
                                                string Temp = "";

                                                for (int j = 3; j < str[i].ToString().Length - 3; j++)
                                                {

                                                    Temp += (str[i].ToString())[j].ToString();
                                                }

                                                objDataInput.Title = "RT " + Convert.ToDouble((Convert.ToDouble(Temp))).ToString("N2");
                                                break;
                                            }
                                        }
                                    }
                                    else if (contents.StartsWith("CHARGE=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        objDataInput.Charge = contents.Substring(contents.IndexOf('=') + 1).Trim();
                                    }
                                    else if (contents.StartsWith("PEPMASS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        objDataInput.PreMass = Convert.ToDouble(contents.Substring(contents.IndexOf('=') + 1).Trim());
                                    }
                                    else if (contents.StartsWith("RTINSECONDS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("SCANS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("END IONS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else
                                    {
                                        //Char[] delimiterChars = { ' ', ',', ':', '\t' };
                                        Char[] delimiterChars = { ' ', '\t', ',' };
                                        String[] words = contents.Split(delimiterChars);
                                        if (words.Length != 0)
                                        {
                                            if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                                            {
                                                //If line contains only 1 value with +/- sign, add appropriate texts & values to parent class                                          
                                                objDataInput.PreMass = Convert.ToDouble(words[0].Substring(0, words[0].IndexOf("+")));
                                                objDataInput.Charge = "1" + words[0].Substring(words[0].IndexOf("+"));
                                                objDataInput.Title = title;
                                                objDataInput.TotalValue = totValue;
                                            }
                                            else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                            {
                                                //Assign the two columns from text file to the child class and add to list one by one
                                                objDataInputValues = new DataInputValues();
                                                objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
                                                objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
                                                if (!lstDataInputValues.Contains(objDataInputValues))
                                                    lstDataInputValues.Add(objDataInputValues);
                                            }
                                            else
                                            {
                                                //End of 1 block - assign list to parent class
                                                if (objDataInput != null)
                                                {
                                                    objDataInput.lstDataInputValues = lstDataInputValues;
                                                    if (!lstDataInput.Contains(objDataInput))
                                                        lstDataInput.Add(objDataInput);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                    }

                                }
                                break;
                            case InputFormatType.TQD:
                                {
                                    //291+
                                    //55	1.020e5
                                    //69	5.442e4
                                    //77	1.659e4
                                    //290	5.079e4
                                    //291	1.824e4

                                    //291+
                                    //55	1.254e4
                                    //299	1.003e4


                                    Char[] delimiterChars = { ' ', '\t', ',' };
                                    String[] words = contents.Split(delimiterChars);
                                    if (words.Length != 0)
                                    {
                                        if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                                        {
                                            //If line contains only 1 value with +/- sign, add appropriate texts & values to parent class  

                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }

                                            objDataInput = new DataInput();
                                            lstDataInputValues = new List<DataInputValues>();
                                            objDataInput.PreMass = Convert.ToDouble(words[0].Substring(0, words[0].IndexOf("+")));
                                            objDataInput.Charge = "1" + words[0].Substring(words[0].IndexOf("+"));
                                            objDataInput.Title = title;
                                            objDataInput.TotalValue = totValue;
                                        }
                                        else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                        {
                                            //Assign the two columns from text file to the child class and add to list one by one
                                            objDataInputValues = new DataInputValues();
                                            objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
                                            objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
                                            if (!lstDataInputValues.Contains(objDataInputValues))
                                                lstDataInputValues.Add(objDataInputValues);
                                        }
                                        else
                                        {
                                            //End of 1 block - assign list to parent class
                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (objDataInput != null)
                                        {
                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }
                                    }
                                }
                                break;
                            case InputFormatType.Xcalibur:
                                {
                                    //SPECTRUM - MS
                                    //3,4,5TRIMETHBENZACID_120321103458.RAW
                                    //- c ESI d Full ms2 211.00@35.00 [ 45.00-225.00]
                                    //Scan #: 101-124
                                    //RT: 1.14-1.57
                                    //AV: 7

                                    //Mass defect: 0.00 @ 1.00, 300.00 @ 1000.00
                                    //Data points: 12
                                    //Mass	Intensity
                                    //137	512
                                    //152	225
                                    //153	10435

                                    if (contents.StartsWith("SPECTRUM - MS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (objDataInput != null)
                                        {
                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }

                                        objDataInput = new DataInput();
                                        lstDataInputValues = new List<DataInputValues>();
                                    }
                                    else if (contents.StartsWith("+") || contents.StartsWith("-"))
                                    {
                                        if (contents.StartsWith("+"))
                                            objDataInput.Charge = "1+";
                                        else
                                            objDataInput.Charge = "1-";

                                        objDataInput.Title = contents.Substring(3).Trim();

                                        String[] words = contents.Split('@');
                                        if (words.Length == 2 && !string.IsNullOrEmpty(words[0]))
                                        {
                                            objDataInput.PreMass = Convert.ToDouble(words[0].Substring(words[0].LastIndexOf(' ') + 1).Trim());
                                        }
                                    }

                                    else
                                    {

                                        Char[] delimiterChars = { '\t' };
                                        String[] words = contents.Split(delimiterChars);
                                        if (words.Length != 0)
                                        {
                                            if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                                            {

                                            }
                                            else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                            {
                                                //Assign the two columns from text file to the child class and add to list one by one
                                                objDataInputValues = new DataInputValues();
                                                objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
                                                objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
                                                if (!lstDataInputValues.Contains(objDataInputValues))
                                                    lstDataInputValues.Add(objDataInputValues);
                                            }

                                        }
                                        else
                                        {
                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                    }

                                }
                                break;
                            case InputFormatType.AminoAcid:
                                {
                                    //146-L-GLUTAMIC ACID
                                    //42	1.314e3
                                    //43	1.016e3
                                    //44	1.027e3
                                    //45	1.927e3

                                    if (contents.Contains("+") || contents.Contains("-"))
                                    {
                                        if (objDataInput != null)
                                        {
                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }

                                        objDataInput = new DataInput();
                                        lstDataInputValues = new List<DataInputValues>();

                                        if (contents.Contains("+"))
                                        {
                                            objDataInput.PreMass = Convert.ToDouble(contents.Substring(0, contents.IndexOf('+')).Trim());
                                            objDataInput.Title = contents.Substring(contents.IndexOf('+') + 1).Trim();
                                            objDataInput.Charge = "1+";
                                            objDataInput.TotalValue = totValue;
                                        }
                                        else
                                        {
                                            objDataInput.PreMass = Convert.ToDouble(contents.Substring(0, contents.IndexOf('-')).Trim());
                                            objDataInput.Title = contents.Substring(contents.IndexOf('-') + 1).Trim();
                                            objDataInput.Charge = "1-";
                                            objDataInput.TotalValue = totValue;
                                        }
                                    }

                                    else
                                    {

                                        Char[] delimiterChars = { '\t' };
                                        String[] words = contents.Split(delimiterChars);
                                        if (words.Length != 0)
                                        {
                                            if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                                            {

                                            }
                                            else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                            {
                                                //Assign the two columns from text file to the child class and add to list one by one
                                                objDataInputValues = new DataInputValues();
                                                objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
                                                objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
                                                if (!lstDataInputValues.Contains(objDataInputValues))
                                                    lstDataInputValues.Add(objDataInputValues);
                                            }
                                            else
                                            {
                                                //End of 1 block - assign list to parent class
                                                if (objDataInput != null)
                                                {
                                                    objDataInput.lstDataInputValues = lstDataInputValues;
                                                    if (!lstDataInput.Contains(objDataInput))
                                                        lstDataInput.Add(objDataInput);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                    }

                                }
                                break;

                        }

                    }
                    //}
                    // For the Row Terminator having no BlankSpace/CR/etc at the end of file
                    {
                        if (objDataInput != null)
                        {
                            objDataInput.lstDataInputValues = lstDataInputValues;
                            if (!lstDataInput.Contains(objDataInput))
                                lstDataInput.Add(objDataInput);
                        }
                    }
                }
                //Release memory
                reader.Dispose();

                //Delete the file uploaded to server
                if (System.IO.File.Exists(fileNameWithPath))
                    System.IO.File.Delete(fileNameWithPath);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error Ocurred in reading file!!!";
                return;
            }
            finally
            {
                reader.Dispose();
            }
        }

        private void ReadDataFile(string fileNameWithPath, string title, int totValue)
        {
            StreamReader reader = null;
            InputFormatType varInputFormatType = InputFormatType.NoFormat;
            String contents;
            int row = 0;

            try
            {
                using (reader = System.IO.File.OpenText(fileNameWithPath))
                {
                    while (reader.Peek() != -1)
                    {
                        contents = reader.ReadLine();
                        if (varInputFormatType == InputFormatType.NoFormat)
                        {
                            if (contents.StartsWith("BEGIN IONS", StringComparison.OrdinalIgnoreCase))
                            {
                                varInputFormatType = InputFormatType.MGF;
                            }
                            else if (contents.StartsWith("SPECTRUM - MS", StringComparison.OrdinalIgnoreCase))
                            {
                                varInputFormatType = InputFormatType.Xcalibur;
                            }
                            else if (contents.EndsWith("+") || contents.EndsWith("-"))
                            {
                                varInputFormatType = InputFormatType.TQD;
                            }
                            else if (contents.Contains("+") || contents.Contains("-"))
                            {
                                varInputFormatType = InputFormatType.AminoAcid;
                            }
                            else if (contents.StartsWith("START IONS", StringComparison.OrdinalIgnoreCase))
                            {
                                varInputFormatType = InputFormatType.tms;
                            }
                        }

                        switch (varInputFormatType)
                        {
                            case InputFormatType.MGF:
                                {
                                    //BEGIN IONS
                                    //TITLE=11I24ZODEC00212240644_130940.1006.1027.1.dta
                                    //CHARGE=1+
                                    //PEPMASS=427.07
                                    //120.17 2945.0
                                    //427.20 11005.0
                                    //427.48 9260.0
                                    //429.46 1152.0
                                    //END IONS

                                    if (contents.StartsWith("BEGIN IONS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (objDataInput != null)
                                        {
                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }

                                        objDataInput = new DataInput();
                                        lstDataInputValues = new List<DataInputValues>();
                                    }
                                    else if (contents.StartsWith("TITLE=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        string[] str = contents.Split(' ', '\t');

                                        for (int i = 0; i < str.Length; i++)
                                        {
                                            if (str[i].StartsWith("RT", StringComparison.OrdinalIgnoreCase))
                                            {
                                                string Temp = "";

                                                for (int j = 3; j < str[i].ToString().Length - 3; j++)
                                                {

                                                    Temp += (str[i].ToString())[j].ToString();
                                                }

                                                objDataInput.Title = "RT " + Convert.ToDouble((Convert.ToDouble(Temp))).ToString("N2");
                                                break;
                                            }
                                        }
                                    }
                                    else if (contents.StartsWith("CHARGE=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        objDataInput.Charge = contents.Substring(contents.IndexOf('=') + 1).Trim();
                                    }
                                    else if (contents.StartsWith("PEPMASS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        objDataInput.PreMass = Convert.ToDouble(contents.Substring(contents.IndexOf('=') + 1).Trim());
                                    }
                                    else if (contents.StartsWith("RTINSECONDS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("SCANS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("END IONS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else
                                    {
                                        //Char[] delimiterChars = { ' ', ',', ':', '\t' };
                                        Char[] delimiterChars = { ' ', '\t', ',' };
                                        String[] words = contents.Split(delimiterChars);
                                        if (words.Length != 0)
                                        {
                                            if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                                            {
                                                //If line contains only 1 value with +/- sign, add appropriate texts & values to parent class                                          
                                                objDataInput.PreMass = Convert.ToDouble(words[0].Substring(0, words[0].IndexOf("+")));
                                                objDataInput.Charge = "1" + words[0].Substring(words[0].IndexOf("+"));
                                                objDataInput.Title = title;
                                                objDataInput.TotalValue = totValue;
                                            }
                                            else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                            {
                                                //Assign the two columns from text file to the child class and add to list one by one
                                                objDataInputValues = new DataInputValues();
                                                objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
                                                objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
                                                if (!lstDataInputValues.Contains(objDataInputValues))
                                                    lstDataInputValues.Add(objDataInputValues);
                                            }
                                            else
                                            {
                                                //End of 1 block - assign list to parent class
                                                if (objDataInput != null)
                                                {
                                                    objDataInput.lstDataInputValues = lstDataInputValues;
                                                    if (!lstDataInput.Contains(objDataInput))
                                                        lstDataInput.Add(objDataInput);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                    }

                                }
                                break;
                            case InputFormatType.TQD:
                                {
                                    //291+
                                    //55	1.020e5
                                    //69	5.442e4
                                    //77	1.659e4
                                    //290	5.079e4
                                    //291	1.824e4

                                    //291+
                                    //55	1.254e4
                                    //299	1.003e4


                                    Char[] delimiterChars = { ' ', '\t', ',' };
                                    String[] words = contents.Split(delimiterChars);
                                    if (words.Length != 0)
                                    {
                                        if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                                        {
                                            //If line contains only 1 value with +/- sign, add appropriate texts & values to parent class  

                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }

                                            objDataInput = new DataInput();
                                            lstDataInputValues = new List<DataInputValues>();
                                            objDataInput.PreMass = Convert.ToDouble(words[0].Substring(0, words[0].IndexOf("+")));
                                            objDataInput.Charge = "1" + words[0].Substring(words[0].IndexOf("+"));
                                            objDataInput.Title = title;
                                            objDataInput.TotalValue = totValue;
                                        }
                                        else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                        {
                                            //Assign the two columns from text file to the child class and add to list one by one
                                            objDataInputValues = new DataInputValues();
                                            objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
                                            objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
                                            if (!lstDataInputValues.Contains(objDataInputValues))
                                                lstDataInputValues.Add(objDataInputValues);
                                        }
                                        else
                                        {
                                            //End of 1 block - assign list to parent class
                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (objDataInput != null)
                                        {
                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }
                                    }
                                }
                                break;
                            case InputFormatType.Xcalibur:
                                {
                                    //SPECTRUM - MS
                                    //3,4,5TRIMETHBENZACID_120321103458.RAW
                                    //- c ESI d Full ms2 211.00@35.00 [ 45.00-225.00]
                                    //Scan #: 101-124
                                    //RT: 1.14-1.57
                                    //AV: 7

                                    //Mass defect: 0.00 @ 1.00, 300.00 @ 1000.00
                                    //Data points: 12
                                    //Mass	Intensity
                                    //137	512
                                    //152	225
                                    //153	10435

                                    if (contents.StartsWith("SPECTRUM - MS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (objDataInput != null)
                                        {
                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }

                                        objDataInput = new DataInput();
                                        objDataInput.TotalValue = totValue;
                                        lstDataInputValues = new List<DataInputValues>();
                                    }
                                    else if (contents.StartsWith("+") || contents.StartsWith("-"))
                                    {
                                        if (contents.StartsWith("+"))
                                            objDataInput.Charge = "1+";
                                        else
                                            objDataInput.Charge = "1-";

                                        objDataInput.Title = contents.Substring(3).Trim();

                                        String[] words = contents.Split('@');
                                        if (words.Length == 2 && !string.IsNullOrEmpty(words[0]))
                                        {
                                            objDataInput.PreMass = Convert.ToDouble(words[0].Substring(words[0].LastIndexOf(' ') + 1).Trim());
                                        }
                                    }

                                    else
                                    {

                                        Char[] delimiterChars = { '\t' };
                                        String[] words = contents.Split(delimiterChars);
                                        if (words.Length != 0)
                                        {
                                            if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                                            {

                                            }
                                            else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                            {
                                                //Assign the two columns from text file to the child class and add to list one by one
                                                objDataInputValues = new DataInputValues();
                                                objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
                                                objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
                                                if (!lstDataInputValues.Contains(objDataInputValues))
                                                    lstDataInputValues.Add(objDataInputValues);
                                            }

                                        }
                                        else
                                        {
                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                    }

                                }
                                break;
                            case InputFormatType.AminoAcid:
                                {
                                    //146-L-GLUTAMIC ACID
                                    //42	1.314e3
                                    //43	1.016e3
                                    //44	1.027e3
                                    //45	1.927e3

                                    if (contents.Contains("+") || contents.Contains("-"))
                                    {
                                        if (objDataInput != null)
                                        {
                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }

                                        objDataInput = new DataInput();
                                        lstDataInputValues = new List<DataInputValues>();
                                        objDataInput.TotalValue = totValue;

                                        if (contents.Contains("+"))
                                        {
                                            objDataInput.PreMass = Convert.ToDouble(contents.Substring(0, contents.IndexOf('+')).Trim());
                                            objDataInput.Title = contents.Substring(contents.IndexOf('+') + 1).Trim();
                                            objDataInput.Charge = "1+";
                                        }
                                        else
                                        {
                                            objDataInput.PreMass = Convert.ToDouble(contents.Substring(0, contents.IndexOf('-')).Trim());
                                            objDataInput.Title = contents.Substring(contents.IndexOf('-') + 1).Trim();
                                            objDataInput.Charge = "1-";
                                        }
                                    }

                                    else
                                    {

                                        Char[] delimiterChars = { '\t' };
                                        String[] words = contents.Split(delimiterChars);
                                        if (words.Length != 0)
                                        {
                                            if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
                                            {

                                            }
                                            else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                            {
                                                //Assign the two columns from text file to the child class and add to list one by one
                                                objDataInputValues = new DataInputValues();
                                                objDataInput.TotalValue = totValue;
                                                objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
                                                objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
                                                if (!lstDataInputValues.Contains(objDataInputValues))
                                                    lstDataInputValues.Add(objDataInputValues);
                                            }
                                            else
                                            {
                                                //End of 1 block - assign list to parent class
                                                if (objDataInput != null)
                                                {
                                                    objDataInput.lstDataInputValues = lstDataInputValues;
                                                    if (!lstDataInput.Contains(objDataInput))
                                                        lstDataInput.Add(objDataInput);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (objDataInput != null)
                                            {
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                    }

                                }
                                break;

                            case InputFormatType.tms:
                                {
                                    //START IONS
                                    //TITLE=RT6.51 CE10EV
                                    //CHARGE=1+
                                    //PREMASS=387
                                    //83	0.27
                                    //85	0.23
                                    //END IONS

                                    if (contents.StartsWith("START IONS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        tempMZ = new List<float>();
                                        tempRI = new List<float>();
                                        srtRI = new List<float>();
                                        tempshortedMZ = new List<float>();

                                        tempMZneg = new List<float>();
                                        tempRIneg = new List<float>();
                                        srtRIneg = new List<float>();
                                        tempshortedMZneg = new List<float>();

                                        if (objDataInput != null)
                                        {

                                            objDataInput.lstDataInputValues = lstDataInputValues;
                                            if (!lstDataInput.Contains(objDataInput))
                                                lstDataInput.Add(objDataInput);
                                        }

                                        objDataInput = new DataInput();
                                        objDataInput.TotalValue = totValue;
                                        lstDataInputValues = new List<DataInputValues>();

                                    }
                                    else if (contents.StartsWith("TITLE=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        objDataInput.Title = contents.Substring(contents.IndexOf('=') + 1).Trim();
                                    }
                                    else if (contents.StartsWith("CHARGE=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        objDataInput.Charge = contents.Substring(contents.IndexOf('=') + 1).Trim();
                                    }
                                    else if (contents.StartsWith("PREMASS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        String val = contents.Substring(contents.IndexOf('=') + 1).Trim();
                                        int n;
                                        bool isNumeric = int.TryParse(val, out n);
                                        if (isNumeric)
                                        {
                                            objDataInput.PreMass = Convert.ToDouble(contents.Substring(contents.IndexOf('=') + 1).Trim());
                                        }
                                        else
                                        {
                                            objDataInput.PreMass = 00000000000000000;
                                        }
                                    }
                                    else if (contents.StartsWith("RTINSECONDS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("SCANS=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Do Nothing
                                    }
                                    else if (contents.StartsWith("END IONS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (objDataInput != null)
                                        {
                                            if (objDataInput.Charge.IndexOf('+') > 0)
                                            {
                                                // Shorting
                                                List<float> usedRI = new List<float>();
                                                tempRI.Sort((a, b) => -1 * a.CompareTo(b));

                                                for (int tli = 0; tli < tempRI.Count; tli++)
                                                {
                                                    float tri = tempRI[tli];
                                                    for (int li = 0; li < tempMZ.Count; li++)
                                                    {
                                                        float ori = srtRI[li];
                                                        if (tri == ori)
                                                        {
                                                            if (tli < Convert.ToInt32(totValue))
                                                            {
                                                                tempshortedMZ.Add((tempMZ[li]));
                                                                objDataInputValues = new DataInputValues();
                                                                objDataInputValues.MZ = Convert.ToDouble(tempMZ[li]);
                                                                objDataInputValues.RI = Convert.ToDouble(String.Format("{0:0.00}", srtRI[li]));
                                                                lstDataInputValues.Add(objDataInputValues);
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }

                                                row = (tempMZ.Count + 1);
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                            else
                                            {
                                                // Shorting
                                                List<float> usedRIneg = new List<float>();

                                                tempRIneg.Sort((a, b) => -1 * a.CompareTo(b));

                                                for (int tli = 0; tli < tempRIneg.Count; tli++)
                                                {
                                                    float tri = tempRIneg[tli];
                                                    for (int li = 0; li < tempMZneg.Count; li++)
                                                    {
                                                        float ori = srtRIneg[li];
                                                        if (tri == ori)
                                                        {
                                                            if (tli < Convert.ToInt32(totValue))
                                                            {
                                                                tempshortedMZneg.Add((tempMZneg[li]));
                                                                objDataInputValues = new DataInputValues();
                                                                objDataInputValues.MZ = Convert.ToDouble(tempMZneg[li]);
                                                                objDataInputValues.RI = Convert.ToDouble(String.Format("{0:0.00}", srtRIneg[li]));
                                                                lstDataInputValues.Add(objDataInputValues);
                                                            }
                                                            break;
                                                        }
                                                    }

                                                }

                                                row = (tempMZneg.Count + 1);
                                                objDataInput.lstDataInputValues = lstDataInputValues;
                                                if (!lstDataInput.Contains(objDataInput))
                                                    lstDataInput.Add(objDataInput);
                                            }
                                        }
                                        // Do Nothing
                                    }
                                    else
                                    {


                                        Char[] delimiterChars = { ' ', '\t', ',' };
                                        String[] words = contents.Split(delimiterChars);

                                        if (words.Length != 0)
                                        {

                                            if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
                                            {

                                                if (objDataInput.Charge.IndexOf('+') > 0)
                                                {

                                                    tempMZ.Add(float.Parse(words[0]));
                                                    tempRI.Add(float.Parse(String.Format("{0:0.00}", words[1])));
                                                    srtRI.Add(float.Parse(String.Format("{0:0.00}", words[1])));

                                                }
                                                else if (objDataInput.Charge.IndexOf('-') > 0)
                                                {

                                                    tempMZneg.Add(float.Parse(words[0]));
                                                    tempRIneg.Add(float.Parse(String.Format("{0:0.00}", words[1])));
                                                    srtRIneg.Add(float.Parse(String.Format("{0:0.00}", words[1])));
                                                }

                                            }
                                            else
                                            {


                                            }
                                        }

                                    }

                                }
                                break;

                        }

                    }
                    //}
                    // For the Row Terminator having no BlankSpace/CR/etc at the end of file
                    {
                        if (objDataInput != null)
                        {
                            objDataInput.lstDataInputValues = lstDataInputValues;
                            if (!lstDataInput.Contains(objDataInput))
                                lstDataInput.Add(objDataInput);
                        }
                    }
                }
                //Release memory
                reader.Dispose();

                //Delete the file uploaded to server
                if (System.IO.File.Exists(fileNameWithPath))
                    System.IO.File.Delete(fileNameWithPath);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error Ocurred in reading file!!!";
                return;
            }
            finally
            {
                reader.Dispose();
            }
        }


        //[NonAction]
        //private void ReadDataFile(string fileNameWithPath, string title, int totValue)
        //{
        //    StreamReader reader = null;
        //    InputFormatType varInputFormatType = InputFormatType.NoFormat;
        //    String contents;
        //    try
        //    {
        //        using (reader = System.IO.File.OpenText(fileNameWithPath))
        //        {
        //            while (reader.Peek() != -1)
        //            {
        //                contents = reader.ReadLine();
        //                if (varInputFormatType == InputFormatType.NoFormat)
        //                {
        //                    if (contents.StartsWith("BEGIN IONS", StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        varInputFormatType = InputFormatType.MGF;
        //                    }
        //                    else if (contents.StartsWith("SPECTRUM - MS", StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        varInputFormatType = InputFormatType.Xcalibur;
        //                    }
        //                    else if (contents.EndsWith("+") || contents.EndsWith("-"))
        //                    {
        //                        varInputFormatType = InputFormatType.TQD;
        //                    }
        //                    else if (contents.Contains("+") || contents.Contains("-"))
        //                    {
        //                        varInputFormatType = InputFormatType.AminoAcid;
        //                    }
        //                }

        //                switch (varInputFormatType)
        //                {
        //                    case InputFormatType.MGF:
        //                        {
        //                            if (contents.StartsWith("BEGIN IONS", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                if (objDataInput != null)
        //                                {
        //                                    objDataInput.lstDataInputValues = lstDataInputValues;
        //                                    if (!lstDataInput.Contains(objDataInput))
        //                                        lstDataInput.Add(objDataInput);
        //                                }

        //                                objDataInput = new DataInput();
        //                                lstDataInputValues = new List<DataInputValues>();
        //                            }
        //                            else if (contents.StartsWith("TITLE=", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                objDataInput.Title = contents.Substring(contents.IndexOf('=') + 1).Trim();
        //                            }
        //                            else if (contents.StartsWith("CHARGE=", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                objDataInput.Charge = contents.Substring(contents.IndexOf('=') + 1).Trim();
        //                            }
        //                            else if (contents.StartsWith("PEPMASS=", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                objDataInput.PreMass = Convert.ToDouble(contents.Substring(contents.IndexOf('=') + 1).Trim());
        //                            }
        //                            else if (contents.StartsWith("END IONS", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                // Do Nothing
        //                            }
        //                            else
        //                            {
        //                                Char[] delimiterChars = { ' ', '\t', ',' };
        //                                String[] words = contents.Split(delimiterChars);
        //                                if (words.Length != 0)
        //                                {
        //                                    if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
        //                                    {
        //                                        objDataInput.PreMass = Convert.ToDouble(words[0].Substring(0, words[0].IndexOf("+")));
        //                                        objDataInput.Charge = "1" + words[0].Substring(words[0].IndexOf("+"));
        //                                        objDataInput.Title = title;
        //                                        objDataInput.TotalValue = totValue;
        //                                    }
        //                                    else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
        //                                    {
        //                                        objDataInputValues = new DataInputValues();
        //                                        objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
        //                                        objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
        //                                        if (!lstDataInputValues.Contains(objDataInputValues))
        //                                            lstDataInputValues.Add(objDataInputValues);
        //                                    }
        //                                    else
        //                                    {
        //                                        if (objDataInput != null)
        //                                        {
        //                                            objDataInput.lstDataInputValues = lstDataInputValues;
        //                                            if (!lstDataInput.Contains(objDataInput))
        //                                                lstDataInput.Add(objDataInput);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (objDataInput != null)
        //                                    {
        //                                        objDataInput.lstDataInputValues = lstDataInputValues;
        //                                        if (!lstDataInput.Contains(objDataInput))
        //                                            lstDataInput.Add(objDataInput);
        //                                    }
        //                                }
        //                            }

        //                        }
        //                        break;
        //                    case InputFormatType.TQD:
        //                        {
        //                            Char[] delimiterChars = { ' ', '\t', ',' };
        //                            String[] words = contents.Split(delimiterChars);
        //                            if (words.Length != 0)
        //                            {
        //                                if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
        //                                {
        //                                    if (objDataInput != null)
        //                                    {
        //                                        objDataInput.lstDataInputValues = lstDataInputValues;
        //                                        if (!lstDataInput.Contains(objDataInput))
        //                                            lstDataInput.Add(objDataInput);
        //                                    }

        //                                    objDataInput = new DataInput();
        //                                    lstDataInputValues = new List<DataInputValues>();
        //                                    objDataInput.PreMass = Convert.ToDouble(words[0].Substring(0, words[0].IndexOf("+")));
        //                                    objDataInput.Charge = "1" + words[0].Substring(words[0].IndexOf("+"));
        //                                    objDataInput.Title = title;
        //                                    objDataInput.TotalValue = totValue;
        //                                }
        //                                else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
        //                                {
        //                                    objDataInputValues = new DataInputValues();

        //                                    objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
        //                                    objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
        //                                    if (!lstDataInputValues.Contains(objDataInputValues))
        //                                        lstDataInputValues.Add(objDataInputValues);
        //                                }
        //                                else
        //                                {
        //                                    if (objDataInput != null)
        //                                    {
        //                                        objDataInput.lstDataInputValues = lstDataInputValues;
        //                                        if (!lstDataInput.Contains(objDataInput))
        //                                            lstDataInput.Add(objDataInput);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (objDataInput != null)
        //                                {
        //                                    objDataInput.lstDataInputValues = lstDataInputValues;
        //                                    if (!lstDataInput.Contains(objDataInput))
        //                                        lstDataInput.Add(objDataInput);
        //                                }
        //                            }
        //                        }
        //                        break;
        //                    case InputFormatType.Xcalibur:
        //                        {

        //                            if (contents.StartsWith("SPECTRUM - MS", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                if (objDataInput != null)
        //                                {
        //                                    objDataInput.lstDataInputValues = lstDataInputValues;
        //                                    if (!lstDataInput.Contains(objDataInput))
        //                                        lstDataInput.Add(objDataInput);
        //                                }

        //                                objDataInput = new DataInput();
        //                                lstDataInputValues = new List<DataInputValues>();
        //                            }
        //                            else if (contents.StartsWith("+") || contents.StartsWith("-"))
        //                            {
        //                                if (contents.StartsWith("+"))
        //                                    objDataInput.Charge = "1+";
        //                                else
        //                                    objDataInput.Charge = "1-";

        //                                objDataInput.Title = contents.Substring(3).Trim();
        //                                objDataInput.TotalValue = totValue;

        //                                String[] words = contents.Split('@');
        //                                if (words.Length == 2 && !string.IsNullOrEmpty(words[0]))
        //                                {
        //                                    objDataInput.PreMass = Convert.ToDouble(words[0].Substring(words[0].LastIndexOf(' ') + 1).Trim());
        //                                }
        //                            }

        //                            else
        //                            {
        //                                Char[] delimiterChars = { '\t' };
        //                                String[] words = contents.Split(delimiterChars);
        //                                if (words.Length != 0)
        //                                {
        //                                    if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
        //                                    {
        //                                    }
        //                                    else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
        //                                    {
        //                                        objDataInputValues = new DataInputValues();
        //                                        objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
        //                                        objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
        //                                        if (!lstDataInputValues.Contains(objDataInputValues))
        //                                            lstDataInputValues.Add(objDataInputValues);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (objDataInput != null)
        //                                    {
        //                                        objDataInput.lstDataInputValues = lstDataInputValues;
        //                                        if (!lstDataInput.Contains(objDataInput))
        //                                            lstDataInput.Add(objDataInput);
        //                                    }
        //                                }
        //                            }

        //                        }
        //                        break;
        //                    case InputFormatType.AminoAcid:
        //                        {
        //                            if (contents.Contains("+") || contents.Contains("-"))
        //                            {
        //                                if (objDataInput != null)
        //                                {
        //                                    objDataInput.lstDataInputValues = lstDataInputValues;
        //                                    if (!lstDataInput.Contains(objDataInput))
        //                                        lstDataInput.Add(objDataInput);
        //                                }

        //                                objDataInput = new DataInput();
        //                                lstDataInputValues = new List<DataInputValues>();

        //                                if (contents.Contains("+"))
        //                                {
        //                                    objDataInput.PreMass = Convert.ToDouble(contents.Substring(0, contents.IndexOf('+')).Trim());
        //                                    objDataInput.Title = contents.Substring(contents.IndexOf('+') + 1).Trim();
        //                                    objDataInput.Charge = "1+";
        //                                    objDataInput.TotalValue = totValue;
        //                                }
        //                                else
        //                                {
        //                                    objDataInput.PreMass = Convert.ToDouble(contents.Substring(0, contents.IndexOf('-')).Trim());
        //                                    objDataInput.Title = contents.Substring(contents.IndexOf('-') + 1).Trim();
        //                                    objDataInput.Charge = "1-";
        //                                    objDataInput.TotalValue = totValue;
        //                                }
        //                            }

        //                            else
        //                            {
        //                                Char[] delimiterChars = { '\t' };
        //                                String[] words = contents.Split(delimiterChars);
        //                                if (words.Length != 0)
        //                                {
        //                                    if (words.Length == 1 && !string.IsNullOrEmpty(words[0]))
        //                                    {
        //                                    }
        //                                    else if (GeneralClass.IsNumeric(words[0]) && GeneralClass.IsNumeric(words[1]))
        //                                    {
        //                                        objDataInputValues = new DataInputValues();
        //                                        objDataInputValues.MZ = Convert.ToDouble(Math.Round(Convert.ToDouble(words[0])));
        //                                        objDataInputValues.RI = Convert.ToDouble(Math.Round(Convert.ToDouble(words[1])));
        //                                        if (!lstDataInputValues.Contains(objDataInputValues))
        //                                            lstDataInputValues.Add(objDataInputValues);
        //                                    }
        //                                    else
        //                                    {
        //                                        if (objDataInput != null)
        //                                        {
        //                                            objDataInput.lstDataInputValues = lstDataInputValues;
        //                                            if (!lstDataInput.Contains(objDataInput))
        //                                                lstDataInput.Add(objDataInput);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (objDataInput != null)
        //                                    {
        //                                        objDataInput.lstDataInputValues = lstDataInputValues;
        //                                        if (!lstDataInput.Contains(objDataInput))
        //                                            lstDataInput.Add(objDataInput);
        //                                    }
        //                                }
        //                            }

        //                        }
        //                        break;
        //                }
        //            }
        //            {
        //                if (objDataInput != null)
        //                {
        //                    objDataInput.lstDataInputValues = lstDataInputValues;
        //                    if (!lstDataInput.Contains(objDataInput))
        //                        lstDataInput.Add(objDataInput);
        //                }
        //            }
        //        }
        //        reader.Dispose();
        //        if (System.IO.File.Exists(fileNameWithPath))
        //            System.IO.File.Delete(fileNameWithPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = "Error Ocurred in reading file!!!";
        //        return;
        //    }
        //    finally
        //    {
        //        reader.Dispose();
        //    }
        //}

        [NonAction]
        private void ProcessData()
        {
            try
            {
                foreach (DataInput objDI in lstDataInput)
                {
                    Double maxRI = 0;
                    List<Double> list = new List<Double>();
                    List<DataInputValues> lstDIV = new List<DataInputValues>();
                    maxRI = objDI.lstDataInputValues.Max(x => x.RI);
                    foreach (DataInputValues objDIV in objDI.lstDataInputValues)
                        objDIV.RIpercent = Convert.ToDouble(Math.Round(Convert.ToDouble(Convert.ToDouble(objDIV.RI * 100) / maxRI), 2));

                    //Loop to set same RI% for similar MZ value
                    List<DataInputValues> local_lstDataInputValues = new List<DataInputValues>();
                    DataInputValues local_objDIV;
                    var mzDistinctList = (from r in objDI.lstDataInputValues
                                          select new
                                          {
                                              r.MZ
                                          }).Distinct().ToList();
                    foreach (var mzObj in mzDistinctList)
                    {
                        local_objDIV = objDI.lstDataInputValues.Where(obj => obj.MZ == mzObj.MZ).OrderByDescending(x => x.RI).First();
                        local_lstDataInputValues.Add(local_objDIV);
                    }
                    int totValue = Convert.ToInt32(Session["TotalValue"]);
                    objDI.lstDataInputValues = objDI.lstDataInputValues.OrderByDescending(x => x.RIpercent).Take(totValue).OrderBy(y => y.MZ).ToList();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error Ocurred in processing data!!!";
                return;
            }
        }

        private void WriteDataToFile()
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (DataInput objDI in lstDataInput)
                {
                    sb.Append("START IONS\r\n");
                    sb.AppendFormat("TITLE={0}\r\n", objDI.Title);
                    sb.AppendFormat("CHARGE={0}\r\n", objDI.Charge);
                    sb.AppendFormat("PREMASS={0}\r\n", objDI.PreMass);
                    foreach (DataInputValues objDIV in objDI.lstDataInputValues)
                        sb.AppendFormat("{0}\t{1}\r\n", objDIV.MZ, objDIV.RIpercent);

                    sb.Append("END IONS\r\n\r\n");
                }

                //Response.Clear();
                //StringWriter stringWrite = null;
                //Response.AddHeader("content-disposition", "attachment;filename=TMSDataFile.tmsd");
                //Response.ContentType = "text/plain";
                //Response.Charset = "";
                //stringWrite = new StringWriter(sb);
                //Response.Write(stringWrite);
                //Response.Flush();
                //Response.End();
                //Delete the file uploaded to server
             
                var byteArray = Encoding.ASCII.GetBytes(sb.ToString());
                string path = Server.MapPath("~/UploadedFiles/ConvertFile"); // folder path
                string filename = "TMSDataFile.tmsd";

                string folderPath = Server.MapPath("~/UploadedFiles/ConvertFile/") + filename;
                if (System.IO.File.Exists(folderPath))
                    System.IO.File.Delete(folderPath);

                FileStream file = new FileStream(path + "/" + filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                file.Write(byteArray, 0, byteArray.Length);
                file.Dispose();

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error Ocurred in writing data to file!!!";
                return;
            }
        }

        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Server.MapPath("~/UploadedFiles/ConvertFile/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}