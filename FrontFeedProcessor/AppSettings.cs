using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    internal class AppSettings
    {
        public string PlanSheetID { get; set; } = "Plan Sheet";
        public string SeedSheetID { get; set; } = "Seed Sheet";
        public string LogPath { get; set; } = @"C:\Code\TestingFS\";
        public string GRXPath { get; set; } = @"\\zengerfp02\mail_production_encrypted\GRX_DM_Prgms\GRX";
        public string AppentionPath { get; set; } = @"\\zengerfp02\mail_production_encrypted\GRX_DM_Prgms\MISC & Archive\GoodRx ID Appention Tracking Spreadsheet.xlsx";
        public string LiveJobsPath { get; set; } = @"Z:\";
        public string ProofingMailTrafficPath { get; set; }
    }
}
