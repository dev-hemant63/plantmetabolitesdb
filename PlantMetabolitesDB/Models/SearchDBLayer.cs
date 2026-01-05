using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantMetabolitesDB.ViewModel;
using System.Configuration;

namespace PlantMetabolitesDB.Models
{
    public class SearchDBLayer
    {
        string ConnectionStrName = ConfigurationManager.ConnectionStrings["ConnectionStr"].ConnectionString;
        public List<MassSpectra_MSAdv_Search> ReadMassSpectra_Advanced_Search(List<MassSpectra_MSAdv_Search> lstMassSpectra_Comprehensive_Search)
        {
            DataSet sqlDS = new DataSet();
            String sProcName = "MassSpectra_Comprehensive_Search";
            SqlParameter[] paramList;
            List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search_Result = new List<MassSpectra_MSAdv_Search>();
            try
            {
                for (int i = 0; i < lstMassSpectra_Comprehensive_Search.Count; i++)
                {
                    List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search_SubResult = new List<MassSpectra_MSAdv_Search>();
                    paramList = new SqlParameter[5];
                    if (lstMassSpectra_Comprehensive_Search[i].Input_DataBaseKey > 0)
                    {
                        if (lstMassSpectra_Comprehensive_Search[i].IncludeMS3MassSpectraKey > 0 && lstMassSpectra_Comprehensive_Search[i].IncludeMS2MassSpectraKey > 0)
                        {
                            paramList[0] = new SqlParameter("@QueryType", "SelectByDatabaseAndMS2_MS3");
                        }
                        else if (lstMassSpectra_Comprehensive_Search[i].IncludeMS3MassSpectraKey > 0)
                        {
                            paramList[0] = new SqlParameter("@QueryType", "SelectByDatabaseAndMS3");
                        }

                        else if (lstMassSpectra_Comprehensive_Search[i].IncludeMS2MassSpectraKey > 0)
                        {
                            paramList[0] = new SqlParameter("@QueryType", "SelectByDatabaseAndMS2");
                        }
                    }                                           
                    else
                    {
                        if (lstMassSpectra_Comprehensive_Search[i].IncludeMS3MassSpectraKey > 0 && lstMassSpectra_Comprehensive_Search[i].IncludeMS2MassSpectraKey > 0)
                        {
                            paramList[0] = new SqlParameter("@QueryType", "SelectByMS2_MS3");
                        }
                        else if (lstMassSpectra_Comprehensive_Search[i].IncludeMS3MassSpectraKey > 0)
                        {
                            paramList[0] = new SqlParameter("@QueryType", "SelectByMS3");
                        }

                        else if (lstMassSpectra_Comprehensive_Search[i].IncludeMS2MassSpectraKey > 0)
                        {
                            paramList[0] = new SqlParameter("@QueryType", "SelectByMS2");
                        }
                    }
                    paramList[1] = new SqlParameter("@InstrumentKey", lstMassSpectra_Comprehensive_Search[i].Input_InstrumentKey);
                    if (!String.IsNullOrEmpty(lstMassSpectra_Comprehensive_Search[i].Input_ParentIon))
                        paramList[2] = new SqlParameter("@MS2ParentIon", lstMassSpectra_Comprehensive_Search[i].Input_ParentIon);
                    else
                        paramList[2] = new SqlParameter("@MS2ParentIon", DBNull.Value);
                    paramList[3] = new SqlParameter("@DataBaseKey", lstMassSpectra_Comprehensive_Search[i].Input_DataBaseKey);
                    paramList[4] = new SqlParameter("@Polarity", lstMassSpectra_Comprehensive_Search[i].Input_Polarity);

                    sqlDS = SqlHelper.ExecuteDataset(ConnectionStrName, CommandType.StoredProcedure, sProcName, paramList);
                    DataTable dtSQL = sqlDS.Tables[0];
                    int count = 0;
                    foreach (DataRow drDetail in dtSQL.DefaultView.ToTable(true, new String[] { "CompoundKey", "MS2MassSpectraKey", "MassSpectraType", "AnnotationKey" }).Rows)
                    {
                        MassSpectra_MSAdv_Search objMassSpectra_MSAdv_Search_Result = new MassSpectra_MSAdv_Search();
                        String matchingString = "(CompoundKey = " + drDetail["CompoundKey"].ToString() + " AND MS2MassSpectraKey = " + drDetail["MS2MassSpectraKey"].ToString() + " AND  MassSpectraType = " + drDetail["MassSpectraType"].ToString() + " AND  AnnotationKey = " + drDetail["AnnotationKey"].ToString() + ")";
                        DataView dv = new DataView(dtSQL, matchingString, null, DataViewRowState.CurrentRows);
                        DataTable dtnew = new DataTable();
                        Int32 cntDataValues = dv.Count;
                        Int32 cntMZMatched = 0;
                        Int32 cntRIMatched = 0;
                        for (int j = 0; j < lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues.Count; j++)
                        {
                            String matchingString_Internal = "(mz_int = " + lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_mz_int.ToString() + ")" + " AND " + "(relative_int - " + lstMassSpectra_Comprehensive_Search[i].Input_Tolerance.ToString() + " <= " + lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_relative_int.ToString() + ") AND (relative_int + " + lstMassSpectra_Comprehensive_Search[i].Input_Tolerance.ToString() + " >= " + lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_relative_int.ToString() + ")";
                            matchingString_Internal = matchingString + " AND " + matchingString_Internal;
                            DataView dv_Internal = new DataView(dtSQL, matchingString_Internal, null, DataViewRowState.CurrentRows);
                            if (dv_Internal.Count > 0)
                            {
                                objMassSpectra_MSAdv_Search_Result.Data_MW = dv_Internal[0]["MolecularWeight"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_Name = dv_Internal[0]["CompoundName"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_Formula = dv_Internal[0]["Formula"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_CompoundKey = dv_Internal[0]["CompoundKey"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_MHit = dv_Internal[0]["AnnotationName"].ToString();
                                objMassSpectra_MSAdv_Search_Result.matchedAnnotationType = Convert.ToInt16(dv_Internal[0]["MassSpectraType"].ToString());
                                objMassSpectra_MSAdv_Search_Result.matchedMassSpectraKey = Convert.ToInt16(dv_Internal[0]["MS2MassSpectraKey"].ToString());
                                objMassSpectra_MSAdv_Search_Result.matchedInputIndex = i;
                                ++cntMZMatched;
                            }
                            matchingString_Internal = "(mz_int = " + lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_mz_int.ToString() + ")" + " AND " + "(relative_int - " + lstMassSpectra_Comprehensive_Search[i].Input_Tolerance.ToString() + " <= " + lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_relative_int.ToString() + ") AND (relative_int + " + lstMassSpectra_Comprehensive_Search[i].Input_Tolerance.ToString() + " >= " + lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_relative_int.ToString() + ")";
                            matchingString_Internal = matchingString + " AND " + matchingString_Internal;
                            dv_Internal = new DataView(dtSQL, matchingString_Internal, null, DataViewRowState.CurrentRows);
                            if (dv_Internal.Count > 0)
                            {
                                objMassSpectra_MSAdv_Search_Result.Data_MW = dv_Internal[0]["MolecularWeight"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_Name = dv_Internal[0]["CompoundName"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_Formula = dv_Internal[0]["Formula"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_CompoundKey = dv_Internal[0]["CompoundKey"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_MHit = dv_Internal[0]["AnnotationName"].ToString();
                                objMassSpectra_MSAdv_Search_Result.matchedAnnotationType = Convert.ToInt16(dv_Internal[0]["MassSpectraType"].ToString());
                                objMassSpectra_MSAdv_Search_Result.matchedMassSpectraKey = Convert.ToInt16(dv_Internal[0]["MS2MassSpectraKey"].ToString());
                                objMassSpectra_MSAdv_Search_Result.matchedInputIndex = i;
                                ++cntRIMatched;
                            }
                        }
                        if (!(cntMZMatched == 0) && !(cntRIMatched == 0))
                        {
                            //objMassSpectra_MSAdv_Search_Result.matchedInputIndex = count;
                            objMassSpectra_MSAdv_Search_Result.Data_mz_DataCoverage = (cntMZMatched * 100) / cntDataValues;
                            objMassSpectra_MSAdv_Search_Result.Data_RI_DataCoverage = (cntRIMatched * 100) / cntDataValues;

                            objMassSpectra_MSAdv_Search_Result.Data_mz_DataCoverage = ((objMassSpectra_MSAdv_Search_Result.Data_mz_DataCoverage) + (objMassSpectra_MSAdv_Search_Result.Data_RI_DataCoverage)) / 2;

                            if (lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues.Count > 0)
                            {
                                objMassSpectra_MSAdv_Search_Result.Data_mz_MatchFactor = Convert.ToDouble(cntMZMatched) / (lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues.Count);
                                objMassSpectra_MSAdv_Search_Result.Data_RI_MatchFactor = Convert.ToDouble(cntRIMatched) / (lstMassSpectra_Comprehensive_Search[i].lstMassSpectra_MSAdv_Search_InputValues.Count);

                                objMassSpectra_MSAdv_Search_Result.Data_mz_MatchFactor = ((objMassSpectra_MSAdv_Search_Result.Data_mz_MatchFactor) + (objMassSpectra_MSAdv_Search_Result.Data_RI_MatchFactor)) / 2;

                            }

                            //int startIndx = -1;
                            //string[] mz_MatchFactor = Convert.ToString(objMassSpectra_MSAdv_Search_Result.Data_mz_MatchFactor).Split(new char[] { '.' });
                            //if (mz_MatchFactor.Length > 1)
                            //{
                            //    startIndx = mz_MatchFactor[1].IndexOf('0', 0, 1);
                            //}

                            //if (startIndx == -1)
                            //{
                            //    lstMassSpectra_MSAdv_Search_SubResult.Add(objMassSpectra_MSAdv_Search_Result);
                            //}

                            lstMassSpectra_MSAdv_Search_SubResult.Add(objMassSpectra_MSAdv_Search_Result);
                            count++;
                        }
                        lstMassSpectra_MSAdv_Search_SubResult = lstMassSpectra_MSAdv_Search_SubResult.OrderByDescending(a => a.Data_mz_DataCoverage).ThenBy(c => c.Data_RI_DataCoverage).ThenBy(c => c.Data_mz_MatchFactor).ThenBy(c => c.Data_RI_MatchFactor).ToList();

                    }
                    if (lstMassSpectra_MSAdv_Search_SubResult.Count > 0)
                    {
                        Int32 maxRecords = (lstMassSpectra_Comprehensive_Search[i].Input_ReportTop < lstMassSpectra_MSAdv_Search_SubResult.Count ? lstMassSpectra_Comprehensive_Search[i].Input_ReportTop : lstMassSpectra_MSAdv_Search_SubResult.Count);
                        lstMassSpectra_MSAdv_Search_SubResult = lstMassSpectra_MSAdv_Search_SubResult.GetRange(0, maxRecords);
                        lstMassSpectra_MSAdv_Search_SubResult[0].Query = lstMassSpectra_Comprehensive_Search[i].Query;
                        lstMassSpectra_MSAdv_Search_SubResult[0].Data_QM = lstMassSpectra_MSAdv_Search_SubResult.Count.ToString();
                        lstMassSpectra_MSAdv_Search_Result.AddRange(lstMassSpectra_MSAdv_Search_SubResult);
                    }
                }
                return lstMassSpectra_MSAdv_Search_Result;
            }
            catch (Exception ex)
            {
                List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search_Result1 = new List<MassSpectra_MSAdv_Search>();
                return lstMassSpectra_MSAdv_Search_Result1;
            }
            finally
            {
                if (sqlDS != null)
                    sqlDS.Dispose();
            }
        }

        public List<MassSpectra_MSAdv_Search> ReadMassSpectra_MSIon_Search(List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search)
        {
            DataSet sqlDS = new DataSet();
            String sProcName = "MassSpectra_MSAdv_Search";
            SqlParameter[] paramList;
            List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search_Result = new List<MassSpectra_MSAdv_Search>();

            try
            {
                for (int i = 0; i < lstMassSpectra_MSAdv_Search.Count; i++)
                {
                    List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search_SubResult = new List<MassSpectra_MSAdv_Search>();

                    paramList = new SqlParameter[7];
                    paramList[0] = new SqlParameter("@InstrumentKey", lstMassSpectra_MSAdv_Search[i].Input_InstrumentKey);
                    paramList[1] = new SqlParameter("@Polarity", (Int16)lstMassSpectra_MSAdv_Search[i].Input_Polarity);
                    if (!String.IsNullOrEmpty(lstMassSpectra_MSAdv_Search[i].Input_ParentIon))
                        paramList[2] = new SqlParameter("@MS2ParentIon", lstMassSpectra_MSAdv_Search[i].Input_ParentIon);
                    else
                        paramList[2] = new SqlParameter("@MS2ParentIon", DBNull.Value);
                    paramList[3] = new SqlParameter("@AnnotationKey", lstMassSpectra_MSAdv_Search[i].Input_AnnotationKey);
                    paramList[4] = new SqlParameter("@AnnotationType", (Int16)lstMassSpectra_MSAdv_Search[i].Input_AnnotationType);
                    paramList[5] = new SqlParameter("@DataBaseKey", (Int16)lstMassSpectra_MSAdv_Search[i].Input_DataBaseKey);
                    paramList[6] = new SqlParameter("@IncludeMS3MassSpectraKey", lstMassSpectra_MSAdv_Search[i].Input_IncludeMS3PrecursorIonKey);

                    sqlDS = SqlHelper.ExecuteDataset(ConnectionStrName, CommandType.StoredProcedure, sProcName, paramList);
                    DataTable dtSQL = sqlDS.Tables[0];

                    //List<int> levels = dtSQL.AsEnumerable().Select(al => al.Field<int>("AccountLevel")).Distinct().ToList();

                    foreach (DataRow drDetail in dtSQL.DefaultView.ToTable(true, new String[] { "CompoundKey", "MS2MassSpectraKey", "MassSpectraType", "AnnotationKey" }).Rows)
                    {
                        MassSpectra_MSAdv_Search objMassSpectra_MSAdv_Search_Result = new MassSpectra_MSAdv_Search();

                        String matchingString = "(CompoundKey = " + drDetail["CompoundKey"].ToString() + " AND MS2MassSpectraKey = " + drDetail["MS2MassSpectraKey"].ToString() + " AND  MassSpectraType = " + drDetail["MassSpectraType"].ToString() + " AND  AnnotationKey = " + drDetail["AnnotationKey"].ToString() + ")";
                        DataView dv = new DataView(dtSQL, matchingString, null, DataViewRowState.CurrentRows);

                        Int32 cntDataValues = dv.Count;
                        Int32 cntMZMatched = 0;
                        Int32 cntRIMatched = 0;

                        for (int j = 0; j < lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues.Count; j++)
                        {
                            String matchingString_Internal = "(mz_int = " + lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_mz_int.ToString() + ")" + " AND " + "(relative_int - " + lstMassSpectra_MSAdv_Search[i].Input_Tolerance.ToString() + " <= " + lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_relative_int.ToString() + ") AND (relative_int + " + lstMassSpectra_MSAdv_Search[i].Input_Tolerance.ToString() + " >= " + lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_relative_int.ToString() + ")";
                            matchingString_Internal = matchingString + " AND " + matchingString_Internal;

                            DataView dv_Internal = new DataView(dtSQL, matchingString_Internal, null, DataViewRowState.CurrentRows);
                            if (dv_Internal.Count > 0)
                            {
                                objMassSpectra_MSAdv_Search_Result.Data_MW = dv_Internal[0]["MolecularWeight"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_Name = dv_Internal[0]["CompoundName"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_Formula = dv_Internal[0]["Formula"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_CompoundKey = dv_Internal[0]["CompoundKey"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_MHit = dv_Internal[0]["AnnotationName"].ToString();
                                objMassSpectra_MSAdv_Search_Result.matchedAnnotationType = Convert.ToInt16(dv_Internal[0]["MassSpectraType"].ToString());
                                objMassSpectra_MSAdv_Search_Result.matchedMassSpectraKey = Convert.ToInt16(dv_Internal[0]["MS2MassSpectraKey"].ToString());
                                objMassSpectra_MSAdv_Search_Result.matchedInputIndex = i;
                                ++cntMZMatched;
                            }

                            matchingString_Internal = "(mz_int = " + lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_mz_int.ToString() + ")" + " AND " + "(relative_int - " + lstMassSpectra_MSAdv_Search[i].Input_Tolerance.ToString() + " <= " + lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_relative_int.ToString() + ") AND (relative_int + " + lstMassSpectra_MSAdv_Search[i].Input_Tolerance.ToString() + " >= " + lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues[j].Input_relative_int.ToString() + ")";
                            matchingString_Internal = matchingString + " AND " + matchingString_Internal;

                            dv_Internal = new DataView(dtSQL, matchingString_Internal, null, DataViewRowState.CurrentRows);
                            if (dv_Internal.Count > 0)
                            {
                                objMassSpectra_MSAdv_Search_Result.Data_MW = dv_Internal[0]["MolecularWeight"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_Name = dv_Internal[0]["CompoundName"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_Formula = dv_Internal[0]["Formula"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_CompoundKey = dv_Internal[0]["CompoundKey"].ToString();
                                objMassSpectra_MSAdv_Search_Result.Data_MHit = dv_Internal[0]["AnnotationName"].ToString();
                                objMassSpectra_MSAdv_Search_Result.matchedAnnotationType = Convert.ToInt16(dv_Internal[0]["MassSpectraType"].ToString());
                                objMassSpectra_MSAdv_Search_Result.matchedMassSpectraKey = Convert.ToInt16(dv_Internal[0]["MS2MassSpectraKey"].ToString());
                                objMassSpectra_MSAdv_Search_Result.matchedInputIndex = i;
                                ++cntRIMatched;

                            }
                        }
                        if (!(cntMZMatched == 0) && !(cntRIMatched == 0))
                        {
                            objMassSpectra_MSAdv_Search_Result.Data_mz_DataCoverage = (cntMZMatched * 100) / cntDataValues;
                            objMassSpectra_MSAdv_Search_Result.Data_RI_DataCoverage = (cntRIMatched * 100) / cntDataValues;

                            if (lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues.Count > 0)
                            {
                                objMassSpectra_MSAdv_Search_Result.Data_mz_MatchFactor = Convert.ToDouble(cntMZMatched) / (lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues.Count);
                                objMassSpectra_MSAdv_Search_Result.Data_RI_MatchFactor = Convert.ToDouble(cntRIMatched) / (lstMassSpectra_MSAdv_Search[i].lstMassSpectra_MSAdv_Search_InputValues.Count);
                            }

                            lstMassSpectra_MSAdv_Search_SubResult.Add(objMassSpectra_MSAdv_Search_Result);
                        }

                        lstMassSpectra_MSAdv_Search_SubResult = lstMassSpectra_MSAdv_Search_SubResult.OrderByDescending(a => a.Data_mz_DataCoverage).ThenBy(c => c.Data_RI_DataCoverage).ThenBy(c => c.Data_mz_MatchFactor).ThenBy(c => c.Data_RI_MatchFactor).ToList();

                    }

                    if (lstMassSpectra_MSAdv_Search_SubResult.Count > 0)
                    {

                        Int32 maxRecords = (lstMassSpectra_MSAdv_Search[i].Input_ReportTop < lstMassSpectra_MSAdv_Search_SubResult.Count ? lstMassSpectra_MSAdv_Search[i].Input_ReportTop : lstMassSpectra_MSAdv_Search_SubResult.Count);
                        lstMassSpectra_MSAdv_Search_SubResult = lstMassSpectra_MSAdv_Search_SubResult.GetRange(0, maxRecords);

                        lstMassSpectra_MSAdv_Search_SubResult[0].Query = lstMassSpectra_MSAdv_Search[i].Query;
                        lstMassSpectra_MSAdv_Search_SubResult[0].Data_QM = lstMassSpectra_MSAdv_Search_SubResult.Count.ToString();
                        lstMassSpectra_MSAdv_Search_Result.AddRange(lstMassSpectra_MSAdv_Search_SubResult);
                    }

                }
                return lstMassSpectra_MSAdv_Search_Result;
            }
            catch (Exception ex)
            {
                List<MassSpectra_MSAdv_Search> lstMassSpectra_MSAdv_Search_Result1 = new List<MassSpectra_MSAdv_Search>();
                return lstMassSpectra_MSAdv_Search_Result1;
            }
            finally
            {
                if (sqlDS != null)
                    sqlDS.Dispose();
            }
        }

    }
}