using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Text;

namespace FrontFeedProcessor
{
    internal class GoogleService
    {
        /*
        public async Task<Spreadsheet> GetSpreadsheet(){
            Sheets = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credentials,
                ApplicationName = "Front Feed"
            });
            try
            {
                var request = Sheets.Spreadsheets.Get(_settings.PlanSheetID);
                var spreadsheet = request.Execute();
                return spreadsheet;
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Code\TestingFS\goofer.txt", ex.Message);
                return null;
            }
        }
        */
    }
}


/*
 * _settings = configuration.GetSection("Settings").Get<AppSettings>() ?? new AppSettings();
            var credentialJson = JsonConvert.SerializeObject(configuration.GetSection("Credentials").GetChildren().ToDictionary(x => x.Key, x => x.Value));
            _credentials = GoogleCredential.FromJson(credentialJson).CreateScoped(new string[] {SheetsService.Scope.Spreadsheets});
*/