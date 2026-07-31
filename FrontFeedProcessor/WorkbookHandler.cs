using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    internal class WorkbookHandler
    {
        private AppSettings _settings;
        private GoogleCredential _credentials;
        private Spreadsheet _spreadsheet;
        private SheetsService _sheets;
        public List<Worksheet> Worksheets;
        public int RowCount;
        public int SheetIndex;
        public int RowIndex;
        
        public WorkbookHandler(IConfiguration configuration)
        {
            Worksheets = new List<Worksheet>();
            GetApiConfig(configuration);
            GetSpreadsheetMetaData();
        }
        public async Task<bool> UpdateData()
        {
            try
            {
                await BatchRead();
                SheetIndex = GetDefaultSheetIndex();
                ChangeSheet();
                return true;
            }
            catch { return false; }
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
        private int GetDefaultSheetIndex()
        {
            string currentMonth = DateTime.Now.ToString("MMMM");
            for(int i = 0; i < Worksheets.Count; i++)
            {
                if (Worksheets[i].Title == currentMonth) return i;
            }
            return Worksheets.Count - 1;
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
                var request = _sheets.Spreadsheets.Get(_settings.PlanSheetID);
                _spreadsheet = request.Execute();
            }
            catch (Exception ex) { Logger.WriteLog(@"C:\Code\TestingFS\goofer.txt", false, ex.Message); }
        }
        private List<string> GetRangesToFetch()
        {
            return _spreadsheet.Sheets.Select(sheet => $"'{sheet.Properties.Title}'").ToList();
        }
        private async Task BatchRead()
        {
            var batchRequest = _sheets.Spreadsheets.Values.BatchGet(_settings.PlanSheetID);
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
