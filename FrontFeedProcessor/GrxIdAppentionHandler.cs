using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML;
using ClosedXML.Excel;

namespace FrontFeedProcessor
{
    public class GrxIdAppentionHandler
    {
        private string IdAlpha;
        private int CurrentId;
        private string LastUsed;
        private int StartId;
        private XLWorkbook Workbook;
        private IXLWorksheet Worksheet;
        private IXLRow LastRow;
        public GrxIdAppentionHandler(string path)
        {
            Workbook = new XLWorkbook(path);
            Worksheet = Workbook.Worksheet("Sheet1");
            
        }
        public string GetNewGoodRxId()
        {
            CurrentId++;
            string id = String.Format("{0}{1}", IdAlpha, CurrentId.ToString("d9"));
            return id;
        }
        public bool GeneratedNewIds()
        {
            if (CurrentId != StartId) return true;
            else return false;
        }
        public void AddRowToSheet(string descriptorJobCode, string leadSourceName, string zgJobNumber)
        {
            IXLRow newRow = Worksheet.Row(LastRow.RowNumber() + 1);
            newRow.Cell("A").Value = DateTime.Now.ToString("MM/dd/yyyy");
            newRow.Cell("B").Value = zgJobNumber;
            newRow.Cell("C").Value = descriptorJobCode;
            newRow.Cell("D").Value = String.Format("{0}{1}", IdAlpha, (StartId + 1).ToString());
            newRow.Cell("E").Value = String.Format("{0}{1}", IdAlpha, CurrentId.ToString());
            newRow.Cell("F").Value = leadSourceName;
            Workbook.Save();
        }
        public void StartRow()
        {
            LastRow = Worksheet.LastRowUsed();
            LastUsed = LastRow.Cell("D").Value.ToString();
            IdAlpha = Regex.Replace(LastUsed, "[0-9]*", "");
            StartId = Int32.Parse(Regex.Replace(LastUsed, "[A-Za-z]*", ""));
            CurrentId = StartId;
        }
        public void CloseRow()
        {
            LastRow = null;
            LastUsed = null;
            IdAlpha = "";
            StartId = -1;
            CurrentId = -1;
        }
        public void Close()
        {
            Workbook.Dispose();
        }
    }
}
