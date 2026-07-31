using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FrontFeedProcessor
{
    public class ProcessHandler
    {
        private AppSettings _settings;
        public List<string> EncryptedPaths;
        public string SuppliedPath;
        public string WorkingPath;
        public string LiveJobPath;
        public string MailingSegment;
        private List<string> FailedDecryption;
        public List<JobBatch> JobBatches;
        public List<Row> RowsToProcess;
        public DataParser Parser;
        public string ZGJobNumber;
        public ProcessReport Report;
        CsvBuilder OutputBuilder;
        GrxIdAppentionHandler AppentionHandler;

        public ProcessHandler(IConfiguration configuration)
        {
            EncryptedPaths = new List<string>();
            FailedDecryption = new List<string>();
            _settings = configuration.GetSection("Settings").Get<AppSettings>() ?? new AppSettings();
            JobBatches = new List<JobBatch>();
            AppentionHandler = new GrxIdAppentionHandler(_settings.AppentionPath);
        }
        public bool PrepareJobForProcessing(List<Row> rowsToProcess, string workingJobNumber)
        {
            try
            {
                ZGJobNumber = workingJobNumber;
                RowsToProcess = rowsToProcess;
                MailingSegment = GenerateMailingSegment(rowsToProcess);
                CreateDirectories(MailingSegment, workingJobNumber);
                MoveEncryptedFiles();
                if (!DecryptAllFiles()) return false;
                Parser = new DataParser(SuppliedPath);
                LogJobStats(workingJobNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ProcessJob(List<Row> rowsToProcess, string workingJobNumber, bool dontAssignMemberId)
        {
            Logger.WriteLog("Starting process.", false);
            try
            {
                string outputPath = Path.Combine(WorkingPath, String.Format("{0}({1})_FF.txt", MailingSegment, ZGJobNumber));
                OutputBuilder = new CsvBuilder(JobBatches, dontAssignMemberId, outputPath, MailingSegment, ZGJobNumber, AppentionHandler);
                Report = new ProcessReport(this);
                return true;
            }
            catch { return false; }
        }
        public void ClearEncryptedPaths()
        {
            EncryptedPaths = new List<string>();
        }
        public void AddEncryptedPath(string fullPath)
        {
            EncryptedPaths.Add(fullPath);
        }
        private bool CreateDirectories(string djc, string jobNumber)
        {
            try
            {
                SuppliedPath = Path.Combine(_settings.GRXPath, string.Format("{0}({1})", djc, jobNumber), "supplied");
                WorkingPath = Path.Combine(_settings.GRXPath, string.Format("{0}({1})", djc, jobNumber), "working");
                LiveJobPath = Path.Combine(_settings.LiveJobsPath, GetPrefixFolder(jobNumber), String.Format("{0} GRX DM", jobNumber));
                string reportsPath = Path.Combine(_settings.GRXPath, string.Format("{0}({1})", djc, jobNumber), "Reports");
                string pdfProofPath = Path.Combine(_settings.GRXPath, string.Format("{0}({1})", djc, jobNumber), "PDF Proofs");
                string dataPath = Path.Combine(_settings.GRXPath, string.Format("{0}({1})", djc, jobNumber), "Data");
                Directory.CreateDirectory(SuppliedPath);
                Directory.CreateDirectory(WorkingPath);
                Directory.CreateDirectory(reportsPath);
                Directory.CreateDirectory(pdfProofPath);
                Directory.CreateDirectory(dataPath);
                Directory.CreateDirectory(Path.Combine(LiveJobPath, "Digital Output"));
                return true;
            }
            catch { return false; }
        }
        private string GetPrefixFolder(string jobNumber)
        {
            try { return String.Format("{0}000", jobNumber.Substring(0, 3)); }
            catch { return "000000"; }
        }
        private bool MoveEncryptedFiles()
        {
            try
            {
                for (int i = 0; i < EncryptedPaths.Count; i++) File.Copy(EncryptedPaths[i], Path.Combine(SuppliedPath, Path.GetFileName(EncryptedPaths[i])));
                return true;
            }
            catch { return false; }
        }
        private string GenerateMailingSegment(List<Row> rows)
        {
            if (rows.Count == 1) return rows[0].DescriptorJobCode1;
            else
            {
                string answer = "";
                string dateCode = rows[0].DescriptorJobCode1.Split("-")[^1];
                for (int i = 0; i < rows.Count; i++)
                {
                    if (i != 0) answer += "-";
                    answer += rows[i].DescriptorJobCode1.Split("-")[0];
                }
                answer += "-" + dateCode;
                return answer;
            }
        }
        private void LogJobStats(string jobNumber)
        {
            string encryptedList = "";
            for(int i = 0; i < EncryptedPaths.Count; i++)
            {
                if (i != 0) encryptedList += "; ";
                encryptedList += Path.GetFileName(EncryptedPaths[i]);
            }
            Logger.WriteLog("Beginning Process:", true);
            Logger.WriteLog("Encrypted files selected:", false);
            Logger.WriteLog(encryptedList, false);
            Logger.WriteLog("Job Number: {0}", false, jobNumber);
        }
        private bool DecryptAllFiles()
        {
            try
            {
                string[] encrypted = Directory.GetFiles(SuppliedPath, "*gpg");
                Logger.WriteLog("Preparing to decrypt {0} files.", false, encrypted.Length.ToString());
                for(int i = 0; i < encrypted.Length; i++)
                {
                    Logger.WriteLog("Attempting to decrypt {0}.", false, encrypted[i]);
                    Decryptor fileDecryptor = new Decryptor(encrypted[i]);
                    if (!fileDecryptor.Success) FailedDecryption.Add(encrypted[i]);
                }
                if (FailedDecryption.Count > 0)
                {
                    Logger.WriteLog("The following files failed to decrypt:", false);
                    for (int i = 0; i < FailedDecryption.Count; i++) Logger.WriteLog(FailedDecryption[i], false);
                    Logger.WriteLog("Please check file integrity and try again.", false);
                    return false;
                }
                else
                {
                    Logger.WriteLog("Decryption successful", false);
                    return true;
                }
            }
            catch
            {
                Logger.WriteLog("Critical error while attempting to decrypt files", false);
                return false;
            }
        }
    }
}
