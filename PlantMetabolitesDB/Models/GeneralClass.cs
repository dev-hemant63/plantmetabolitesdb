using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO.Ports;
using System.IO;
using PlantMetabolitesDB.ViewModel;
using System.Configuration;
using System.Web.Services.Description;
using System.Web.Mail;
using System.Security.Cryptography;
using System.Text;

namespace PlantMetabolitesDB.Models
{
    public class GeneralClass
    {
    
        public enum MonthName
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }
        public List<SelectListItem> GetPolarity()
        {
            List<SelectListItem> ObjPolarity = new List<SelectListItem>()
            {
              new SelectListItem {Text = "Select", Value = "" , Selected = true },
              new SelectListItem {Text = "Positive", Value = "1" },
              new SelectListItem {Text = "Negative", Value = "2" }

            };

            return ObjPolarity;
        }

        public List<SelectListItem> GetAnnotationType()
        {
            List<SelectListItem> ObjAnnotationType = new List<SelectListItem>()
            {
              new SelectListItem {Text = "Select", Value = "" , Selected = true },
              new SelectListItem {Text = "MS1", Value = "1" },
              new SelectListItem {Text = "MS2", Value = "2" },
              //new SelectListItem {Text = "Adduct", Value = "2" },
              new SelectListItem {Text = "MS3", Value = "3" }

            };

            return ObjAnnotationType;
        }

        public List<SelectListItem> GetDisplayOrder()
        {
            List<SelectListItem> ObjDisplayOrder = new List<SelectListItem>();
            for (int i = 1; i < 100; i++)
            {
                ObjDisplayOrder.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
            ObjDisplayOrder.Insert(0, new SelectListItem { Text = "Select", Value = "", Selected = true });

            return ObjDisplayOrder;
        }

        public List<SelectListItem> GetTotalValues()
        {
            List<SelectListItem> ObjTotalValues = new List<SelectListItem>();
            for (int i = 1; i <= 100; i++)
            {
                ObjTotalValues.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
           // ObjTotalValues.Insert(0, new SelectListItem { Text = "Select", Value = "", Selected = true });

            return ObjTotalValues;
        }

        public List<SelectListItem> GetIntensityType()
        {
            List<SelectListItem> ObjIntensityType = new List<SelectListItem>()
            {
              new SelectListItem {Text = "Select", Value = "" , Selected = true },
              new SelectListItem {Text = "Relative", Value = "1" },
              new SelectListItem {Text = "Absolute", Value = "2" }

            };

            return ObjIntensityType;
        }

        public List<SelectListItem> GetReportTop()
        {
            List<SelectListItem> ObjReportTop = new List<SelectListItem>();
            for (int i = 1; i <= 10; i++)
            {
                ObjReportTop.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
            ObjReportTop.Insert(0, new SelectListItem { Text = "Select", Value = "", Selected = true });

            return ObjReportTop;
        }

        public List<SelectListItem> GetTolerance()
        {
            List<SelectListItem> ObjTolerance = new List<SelectListItem>();
            for (int i = 0; i <= 50; i += 5)
            {
                ObjTolerance.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
            ObjTolerance.Insert(0, new SelectListItem { Text = "Select", Value = "", Selected = true });

            return ObjTolerance;
        }

        public List<SelectListItem> GetProductIons()
        {
            List<SelectListItem> ObjTolerance = new List<SelectListItem>();
            for (int i = 1; i <= 25; i++)
            {
                ObjTolerance.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
            ObjTolerance.Insert(0, new SelectListItem { Text = "Select", Value = "", Selected = true });

            return ObjTolerance;
        }

        public List<SelectListItem> GetMessageType()
        {
            List<SelectListItem> ObjMessageType = new List<SelectListItem>()
            {
              new SelectListItem {Text = "Select", Value = "" , Selected = true },
              new SelectListItem {Text = "Description Only", Value = "1" },
              new SelectListItem {Text = "Upload File", Value = "2" }

            };

            return ObjMessageType;
        }

        public List<SelectListItem> GetUserType()
        {
            List<SelectListItem> ObjUserType = new List<SelectListItem>()
            {
              new SelectListItem {Text = "Select", Value = "" , Selected = true },
              new SelectListItem {Text = "SuperAdmin", Value = "1" },
              new SelectListItem {Text = "Admin", Value = "2" },
              new SelectListItem {Text = "User", Value = "3" }

            };

            return ObjUserType;
        }

        public List<SelectListItem> GetCollaboratorsType()
        {
            List<SelectListItem> ObjCollaboratorsType = new List<SelectListItem>()
            {
              new SelectListItem {Text = "Collaborators", Value = "1" },
              new SelectListItem {Text = "Contributors", Value = "2" }

            };

            return ObjCollaboratorsType;
        }

        public List<SelectListItem> GetDataFormats()
        {
            List<SelectListItem> GetDataFormats = new List<SelectListItem>()
            {
              new SelectListItem {Text = "MassLymx.txt", Value = "1" },
              new SelectListItem {Text = "mgf Format", Value = "2" },
              new SelectListItem {Text = "Xcalibur Format", Value = "3" },
              new SelectListItem {Text = "Analyst Format", Value = "4" }

            };

            return GetDataFormats;
        }

        public static string CreateRandomPassword(int length = 8)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        public void SendEmailToUser(string emailId, string userVerificationLink)
        {
            try
            {
                var fromMail = new MailAddress("tmsdatabase80@gmail.com", "tmsdatabase.org"); // set your email    
                var fromEmailpassword = "ajbujtzhmoztcgbv"; // Set your password     
                var toEmail = new MailAddress(emailId);

                var smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

                var Message = new System.Net.Mail.MailMessage(fromMail, toEmail);
                Message.Subject = "Verification mail from tmsdatabase.org";
                Message.Body = "<br/> Your registration completed succesfully." +
                               "<br/> please click on the below link for account verification" +
                               "<br/><br/><a href=" + userVerificationLink + ">" + userVerificationLink + "</a>";
                Message.IsBodyHtml = true;
                smtp.Send(Message);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void SendEmail(string emailId, string emailsubject, string emailbody)
        {
            try
            {
                var fromMail = new MailAddress("tmsdatabase80@gmail.com", "tmsdatabase.org"); // set your email    
                var fromEmailpassword = "ajbujtzhmoztcgbv"; //"TMS@db80cdri"; // Set your password     
                var toEmail = new MailAddress(emailId);

                var smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

                var Message = new System.Net.Mail.MailMessage(fromMail, toEmail);

                Message.Subject = emailsubject; //"Verification mail from tmsdatabase.org";
                Message.Body = emailbody;
                Message.IsBodyHtml = true;
                smtp.Send(Message);
            }
            catch (Exception)
            {
                 throw;
            }
            
        }

        public static Boolean IsNumeric(Object Expression)
        {
            if (Expression == null || Expression is DateTime)
                return false;

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
                return true;

            try
            {
                if (Expression is string)
                    Double.Parse(Expression as string);
                else
                    Double.Parse(Expression.ToString());
                return true;
            }
            catch { } // just dismiss errors but return false
            {
                return false;
            }
        }

        public static void SetCookie(HttpContext context, string key, string value, int expireDay = 1)
        {
            var cookie = new HttpCookie(key, value);
            cookie.Expires = DateTime.Now.AddDays(expireDay);
            context.Response.Cookies.Add(cookie);
        }

        public static string GetCookie(HttpContext context, string key)
        {
            string value = string.Empty;

            var cookie = context.Request.Cookies[key];

            if (cookie != null)
            {
                if (string.IsNullOrWhiteSpace(cookie.Value))
                {
                    return value;
                }
                value = cookie.Value;
            }

            return value;
        }

        public static List<MassSpectra_DataValues> GetMassSpectra_DataValues(String fileNameWithPath, int annotationType)
        {
            List<MassSpectra_DataValues> lstMassSpectra_DataValues;
            String contents;
            MassSpectra_DataValues lstObjRecord = null;
            StreamReader reader = null;

            try
            {
                lstMassSpectra_DataValues = new List<MassSpectra_DataValues>();
                using (reader = File.OpenText(fileNameWithPath))
                {
                    while (reader.Peek() != -1)
                    {
                        contents = reader.ReadLine();
                        //Char[] delimiterChars = { ' ', ',', ':', '\t' };
                        Char[] delimiterChars = { '\t' };
                        String[] words = contents.Split(delimiterChars);
                        if (words.Length != 0)
                        {
                            //if (GlobalFunctions.IsNumeric(words[0]) && GlobalFunctions.IsNumeric(words[1]) && GlobalFunctions.IsNumeric(words[2]))
                            if (IsNumeric(words[0]) && IsNumeric(words[1]))
                            {
                                lstObjRecord = new MassSpectra_DataValues();
                                lstObjRecord.MassSpectraType = (Int16)annotationType;
                                lstObjRecord.mz = words[0];
                                //lstObjRecord.absolute = words[1];
                                lstObjRecord.relative = words[1];

                                Double dbValue;

                                Double.TryParse(lstObjRecord.mz, out dbValue);
                                lstObjRecord.mz_int = Convert.ToInt32(Math.Round(dbValue));

                                //Double.TryParse(lstObjRecord.absolute, out dbValue);
                                //lstObjRecord.absolute_int = Convert.ToInt32(Math.Round(dbValue));

                                Double.TryParse(lstObjRecord.relative, out dbValue);
                                lstObjRecord.relative_int = Convert.ToInt32(Math.Round(dbValue));

                                lstMassSpectra_DataValues.Add(lstObjRecord);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lstMassSpectra_DataValues = new List<MassSpectra_DataValues>();
            }
            finally
            {
                reader.Dispose();
            }
            return lstMassSpectra_DataValues;
        }

        public List<MassSpectra_MSAdv_Search> GetMassSpectra_Advanced_Search(AdvancedSearchViewModel model)
        {
            List<MassSpectra_MSAdv_Search> lstMassSpectra_Advanced_Search = new List<MassSpectra_MSAdv_Search>();
            String contents;
            MassSpectra_MSAdv_Search objMassSpectra_MSAdv_Search = null;
            List<MassSpectra_MSAdv_Search_InputValues> lstMassSpectra_MSAdv_Search_InputValues = null;
            MassSpectra_MSAdv_Search_InputValues objMassSpectra_MSAdv_Search_InputValues = null;
            StreamReader reader = null;
            try
            {
                using (reader = File.OpenText(model.DataFilePathWithName))
                {
                    while (reader.Peek() != -1)
                    {
                        contents = reader.ReadLine();
                        if (contents.StartsWith("START IONS", StringComparison.OrdinalIgnoreCase))
                        {

                            objMassSpectra_MSAdv_Search = new MassSpectra_MSAdv_Search();
                            lstMassSpectra_MSAdv_Search_InputValues = new List<MassSpectra_MSAdv_Search_InputValues>();

                            objMassSpectra_MSAdv_Search.Input_InstrumentKey = model.InstrumentKey;
                            objMassSpectra_MSAdv_Search.Input_ReportTop = model.ReportTopKey;
                            objMassSpectra_MSAdv_Search.Input_AnnotationKey = model.AnnotationKey;
                            objMassSpectra_MSAdv_Search.Input_AnnotationType = model.MassSpectraType;
                            objMassSpectra_MSAdv_Search.IncludeMS3MassSpectraKey = model.MS3IonSearch == true ? Convert.ToInt16(1) : Convert.ToInt16(0);
                            objMassSpectra_MSAdv_Search.IncludeMS2MassSpectraKey = model.MS2IonSearch == true ? Convert.ToInt16(1) : Convert.ToInt16(0);
                            objMassSpectra_MSAdv_Search.Input_Tolerance = model.ToleranceKey;
                            objMassSpectra_MSAdv_Search.Input_DataBaseKey = model.DatabaseKey;
                        }

                        else if (contents.StartsWith("CHARGE=", StringComparison.OrdinalIgnoreCase))
                        {
                            string val = contents.Substring(contents.IndexOf('=') + 1).Trim();
                            if (val.Contains("+"))
                            {
                                objMassSpectra_MSAdv_Search.Input_Polarity = 1;
                            }
                            else if (val.Contains("-"))
                            {
                                objMassSpectra_MSAdv_Search.Input_Polarity = 2;
                            }
                        }
                        else if (contents.StartsWith("TITLE=", StringComparison.OrdinalIgnoreCase))
                        {
                            objMassSpectra_MSAdv_Search.Query = contents.Substring(contents.IndexOf('=') + 1).Trim();
                        }
                        else if (contents.StartsWith("PREMASS=", StringComparison.OrdinalIgnoreCase))
                        {
                            objMassSpectra_MSAdv_Search.Input_ParentIon = contents.Substring(contents.IndexOf('=') + 1).Trim();
                        }
                        else if (contents.StartsWith("END IONS", StringComparison.OrdinalIgnoreCase))
                        {   
                            if (model.IntensityTypeKey.Equals(1))
                                ReCalculateInput_relative(ref lstMassSpectra_MSAdv_Search_InputValues);
                            objMassSpectra_MSAdv_Search.lstMassSpectra_MSAdv_Search_InputValues = lstMassSpectra_MSAdv_Search_InputValues;
                            lstMassSpectra_Advanced_Search.Add(objMassSpectra_MSAdv_Search);

                        }
                        else
                        {
                            Char[] delimiterChars = { '\t' };
                            String[] words = contents.Split(delimiterChars);
                            if (words.Length != 0)
                            {
                                if (IsNumeric(words[0]) && IsNumeric(words[1]))
                                {
                                    objMassSpectra_MSAdv_Search_InputValues = new MassSpectra_MSAdv_Search_InputValues();
                                    String txtMZ, txtIntensity;
                                    txtMZ = words[0];
                                    txtIntensity = words[1];
                                    Double dbValue;
                                    Double.TryParse(txtMZ, out dbValue);
                                    objMassSpectra_MSAdv_Search_InputValues.Input_mz_int = Convert.ToInt32(Math.Round(dbValue));
                                    Double.TryParse(txtIntensity, out dbValue);
                                    if (model.IntensityTypeKey.Equals(1)) //Relative
                                    {
                                        objMassSpectra_MSAdv_Search_InputValues.Input_relative_int = Convert.ToInt32(Math.Round(dbValue));
                                        objMassSpectra_MSAdv_Search_InputValues.mznew = dbValue.ToString();
                                        if (lstMassSpectra_MSAdv_Search_InputValues != null)
                                        {
                                            if (!lstMassSpectra_MSAdv_Search_InputValues.Contains(objMassSpectra_MSAdv_Search_InputValues))
                                                lstMassSpectra_MSAdv_Search_InputValues.Add(objMassSpectra_MSAdv_Search_InputValues);
                                        }

                                    }
                                    else
                                    {
                                        objMassSpectra_MSAdv_Search_InputValues.Input_absolute_int = Convert.ToInt32(Math.Round(dbValue));
                                        objMassSpectra_MSAdv_Search_InputValues.mznew = dbValue.ToString();
                                        if (lstMassSpectra_MSAdv_Search_InputValues != null)
                                        {
                                            if (!lstMassSpectra_MSAdv_Search_InputValues.Contains(objMassSpectra_MSAdv_Search_InputValues))
                                                lstMassSpectra_MSAdv_Search_InputValues.Add(objMassSpectra_MSAdv_Search_InputValues);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                reader.Dispose();
                if (File.Exists(model.DataFilePathWithName))
                    File.Delete(model.DataFilePathWithName);

                return lstMassSpectra_Advanced_Search;
            }
            catch (Exception ex)
            {
                return lstMassSpectra_Advanced_Search;
            }
            finally
            {
                reader.Dispose();
            }

        }

        private void ReCalculateInput_relative(ref List<MassSpectra_MSAdv_Search_InputValues> lstMassSpectra_MSAdv_Search_InputValues)
        {
            lstMassSpectra_MSAdv_Search_InputValues.Sort(delegate (MassSpectra_MSAdv_Search_InputValues a, MassSpectra_MSAdv_Search_InputValues b)
            {
                return b.Input_absolute_int.CompareTo(a.Input_absolute_int);
            });
            for (int i = 0; i < lstMassSpectra_MSAdv_Search_InputValues.Count; i++)
            {
                if (lstMassSpectra_MSAdv_Search_InputValues[i].Input_absolute_int != 0)
                {
                    lstMassSpectra_MSAdv_Search_InputValues[i].Input_relative_int = (lstMassSpectra_MSAdv_Search_InputValues[i].Input_absolute_int * 100) / lstMassSpectra_MSAdv_Search_InputValues[i].Input_absolute_int;
                }
            }
        }


        public static string GeneratePassword(int length) //length of salt    
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var randNum = new Random();
            var chars = new char[length];
            var allowedCharCount = allowedChars.Length-1;
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedCharCount) * randNum.NextDouble())];
            }
            return new string(chars);
        }
        public static string EncodePassword(string pass, string salt) //encrypt password    
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Encoding.Unicode.GetBytes(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            //return Convert.ToBase64String(inArray);    
            return EncodePasswordMd5(Convert.ToBase64String(inArray));
        }
        public static string EncodePasswordMd5(string pass) //Encrypt using MD5    
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)    
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);
        }
        public static string base64Encode(string sData) // Encode    
        {
            try
            {
                byte[] encData_byte = new byte[sData.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(sData);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        public static string base64Decode(string sData) //Decode    
        {
            try
            {
                var encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecodeByte = Convert.FromBase64String(sData);
                int charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                char[] decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                string result = new String(decodedChar);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);
            }
        }

    }
}