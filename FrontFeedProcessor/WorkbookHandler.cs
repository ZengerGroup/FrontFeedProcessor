using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FrontFeedProcessor
{
    internal class WorkbookHandler
    {
        private AppSettings _settings;
        private GoogleCredential _credentials;
        private Spreadsheet _spreadsheet;
        private SheetsService _sheets;
        private Catalogue _catalogue;
        public List<Worksheet> Worksheets;
        public int RowCount;
        public int SheetIndex;
        public int RowIndex;
        public string WorkingYear;
        
        public WorkbookHandler(IConfiguration configuration)
        {
            Worksheets = new List<Worksheet>();
            _catalogue = configuration.GetSection("Catalogue").Get<Catalogue>() ?? new Catalogue();
            WorkingYear = DateTime.Now.ToString("yyyy");
            GetApiConfig(configuration);
            GetSpreadsheetMetaData();
        }
        public async Task<bool> UpdateData(bool firstLoad)
        {
            try
            {
                await BatchRead();
                SheetIndex = GetDefaultSheetIndex(firstLoad);
                ChangeSheet();
                return true;
            }
            catch (Exception e){ Logger.WriteLog(e.Message, false); return false; }
        }
        public void ChangeSheet()
        {
            RowCount = Worksheets[SheetIndex].SheetRows.Length;
            RowIndex = 1;
        }
        public List<Row> GetRowsToProcess()
        {
            List<Row> toProcess = new List<Row>();
            for (int i = 0; i < Worksheets[SheetIndex].SheetRows.Length; i++)
                if (Worksheets[SheetIndex].SheetRows[i].Selected) toProcess.Add(Worksheets[SheetIndex].SheetRows[i]);
            return toProcess;
        }
        public bool YearIsAvailable(int yearChange)
        {
            string yearToCheck = (Int32.Parse(WorkingYear) + yearChange).ToString();
            if (_catalogue.Workbooks.ContainsKey(yearToCheck)) return true;
            else return false;
        }
        public void ChangeYear(int yearChange)
        {
            try
            {
                Worksheets = new List<Worksheet>();
                WorkingYear = (Int32.Parse(WorkingYear) + yearChange).ToString();
                GetSpreadsheetMetaData();
                if (yearChange < 0) SheetIndex = _spreadsheet.Sheets.Count - 1;
                else SheetIndex = 0;
            }
            catch (Exception e){ Logger.WriteLog(e.Message, false); }
        }
        public void UpdateGoogleSheet(List<JobBatch> batches)
        {
            for(int i = 0; i < batches.Count; i++)
            {
                try
                {
                    string cellString = String.Format("{0}!O{1}", batches[i].BatchRows[0].Month, batches[i].BatchRows[0].RowNumber);
                    var valueRange = new ValueRange();
                    valueRange.Values = new List<IList<object>> { new List<object> { batches[i].BatchRecords.Count } };
                    var updateRequest = _sheets.Spreadsheets.Values.Update(valueRange, _catalogue.Workbooks[WorkingYear], cellString);
                    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                    UpdateValuesResponse response = updateRequest.Execute();
                }
                catch { ErrorReport.NewError(String.Format("Failed to update google sheet, please review. Row: {0}", batches[i].BatchRows[0].RowNumber)); }
            }
        }
        public void UpdateGoogleSheet(List<JobBatch> batches, string jobNumber)
        {
            for (int i = 0; i < batches.Count; i++)
            {
                try
                {
                    string countCellString = String.Format("{0}!O{1}", batches[i].BatchRows[0].Month, batches[i].BatchRows[0].RowNumber);
                    string jobCellString = String.Format("{0}!E{1}", batches[i].BatchRows[0].Month, batches[i].BatchRows[0].RowNumber);
                    var toUpdateRange = new List<ValueRange>();
                    toUpdateRange.Add(new ValueRange
                    {
                        Range = countCellString,
                        Values = new List<IList<object>> { new List<object> { batches[i].BatchRecords.Count } }
                    });
                    toUpdateRange.Add(new ValueRange
                    {
                        Range = jobCellString,
                        Values = new List<IList<object>> { new List<object> { jobNumber } }
                    });
                    var batchBody = new BatchUpdateValuesRequest
                    {
                        ValueInputOption = "RAW",
                        Data = toUpdateRange
                    };
                    var batchRequest = _sheets.Spreadsheets.Values.BatchUpdate(batchBody, _catalogue.Workbooks[WorkingYear]);
                    BatchUpdateValuesResponse response = batchRequest.Execute();
                }
                catch { ErrorReport.NewError(String.Format("Failed to update google sheet, please review. Row: {0}", batches[i].BatchRows[0].RowNumber)); }
            }
        }
        private int GetDefaultSheetIndex(bool firstLoad)
        {
            if (firstLoad)
            {
                string currentMonth = DateTime.Now.ToString("MMMM");
                for(int i = 0; i < Worksheets.Count; i++)
                {
                    if (Worksheets[i].Title == currentMonth) return i;
                }
                return Worksheets.Count - 1;
            }
            else
            {
                Logger.WriteLog(SheetIndex.ToString(), false);
                return SheetIndex;
            }
            
        }
        private void GetApiConfig(IConfiguration configuration)
        {
            _settings = configuration.GetSection("Settings").Get<AppSettings>() ?? new AppSettings();
            var credentialJson = JsonConvert.SerializeObject(configuration.GetSection("Credentials").GetChildren().ToDictionary(x => x.Key, x => x.Value));
            _credentials = GoogleCredential.FromJson(credentialJson).CreateScoped(new string[] { SheetsService.Scope.Spreadsheets });
            _sheets = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credentials,
                ApplicationName = "Front Feed"
            });
        }
        private void GetSpreadsheetMetaData()
        {
            try
            {
                var request = _sheets.Spreadsheets.Get(_catalogue.Workbooks[WorkingYear]);
                _spreadsheet = request.Execute();
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message, false); }
        }
        private List<string> GetRangesToFetch()
        {
            return _spreadsheet.Sheets.Select(sheet => $"'{sheet.Properties.Title}'").ToList();
        }
        private async Task BatchRead()
        {
            var batchRequest = _sheets.Spreadsheets.Values.BatchGet(_catalogue.Workbooks[WorkingYear]);
            batchRequest.Ranges = GetRangesToFetch();
            BatchGetValuesResponse batchResponse = await batchRequest.ExecuteAsync();
            if(batchResponse.ValueRanges != null)
            {
                foreach (ValueRange sheetData in batchResponse.ValueRanges)
                {
                    string cleanSheetName = sheetData.Range.Split('!')[0].Replace("'", "");
                    Worksheets.Add(new Worksheet(cleanSheetName, sheetData.Values));
                }
            }
        }
    }
}
