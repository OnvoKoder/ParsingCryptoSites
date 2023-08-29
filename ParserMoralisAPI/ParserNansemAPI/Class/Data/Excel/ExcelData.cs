using OfficeOpenXml;
using ParserNansemAPI.Class.Interface;
using ParserNansemAPI.Class.Patterns;
using System;
using System.Collections.Generic;
using System.IO;

namespace ParserNansemAPI.Class.Data.Excel
{
     class ExcelData:IData
    {
        private ExcelClose close = new ExcelClose();
        public ExcelData()=> close.CloseProcess();

        public void SaveInFile(ref List<ExcelTable> list)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string dir = Directory.GetCurrentDirectory();
            string path = dir + $@"/report_{DateTime.Now.ToShortDateString()}.xlsx";
            FileInfo excelFile = new FileInfo(path);
            using (ExcelPackage excel = new ExcelPackage(excelFile))
            {
                excel.Workbook.Worksheets.Add("Owner");
                excel.Workbook.Worksheets.Add("Transactions");
                excel.Workbook.Worksheets.Add("Links");
                ExcelWorksheet worksheetClient = excel.Workbook.Worksheets["Owner"];
                string[] header = new string[] { "ID","Owner", "ENS", "Balance"};
                for(int i = 1; i < header.Length; i++)
                    worksheetClient.Cells[1,i].Value = header[i];
                for (int i = 0; i < list.Count; i++)
                {
                    worksheetClient.Cells[i + 2, 1].Value = i +1;
                    worksheetClient.Cells[i + 2, 2].Value = list[i].OwnerOf;
                    worksheetClient.Cells[i + 2, 3].Value = list[i].ENS;
                    worksheetClient.Cells[i + 2, 4].Value = list[i].Balance;
                }
                ExcelWorksheet worksheetTrabsaction = excel.Workbook.Worksheets["Transactions"];
                string[] headerTransaction = new string[] { "ID", "Transactions" };
                for (int i = 1; i < headerTransaction.Length; i++)
                    worksheetTrabsaction.Cells[1, i].Value = headerTransaction[i];
                int step = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if(step == 0)
                    {
                        worksheetTrabsaction.Cells[i + 2, 1].Value = i + 1;
                        if (list[i].AddressTransactions != null)
                        {
                            for (int j = 0; j < list[i].AddressTransactions.Length; j++)
                                worksheetTrabsaction.Cells[i + 2 + j, 2].Value = list[i].AddressTransactions[j];
                        }
                    }
                    else
                    {
                        worksheetTrabsaction.Cells[i + step, 1].Value = i + 1;
                        if(list[i].AddressTransactions != null)
                        {
                            for (int j = 0; j < list[i].AddressTransactions.Length; j++)
                                worksheetTrabsaction.Cells[i + step + j, 2].Value = list[i].AddressTransactions[j];
                            step += list[i].AddressTransactions.Length;
                        } 
                    }
                    
                }
                ExcelWorksheet worksheetLink = excel.Workbook.Worksheets["Links"];
                string[] headerLink = new string[] { "ID", "Links" };
                for (int i = 1; i < headerLink.Length; i++)
                    worksheetLink.Cells[1, i].Value = headerLink[i];
                step = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if (step == 0)
                    {
                        worksheetLink.Cells[i + 2, 1].Value = i + 1;
                        for (int j = 0; j < list[i].Link.Length; j++)
                            worksheetLink.Cells[i + 2 + j, 2].Value = list[i].Link[j];
                    }
                    else
                    {
                        worksheetLink.Cells[i + step, 1].Value = i + 1;
                        for (int j = 0; j < list[i].Link.Length; j++)
                            worksheetLink.Cells[i + step + j, 2].Value = list[i].Link[j];

                    }
                    step += list[i].Link.Length;
                   
                }
                excel.SaveAs(excelFile);
            }
        }
    }
}
