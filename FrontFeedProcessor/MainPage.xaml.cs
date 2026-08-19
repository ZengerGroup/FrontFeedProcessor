using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel;

namespace FrontFeedProcessor
{
    public partial class MainPage : ContentPage
    {
        private readonly AppSettings _settings;
        private readonly GoogleCredential _credentials;
        //private readonly Catalogue _catalogue;
        private WorkbookHandler Workbook;
        private ProcessHandler Process;
        private bool NewJobNumber;


        public MainPage(IConfiguration configuration)
        {
            InitializeComponent();
            Logger.InitializeLogger(configuration);
            ErrorReport.Initialize();
            Workbook = new WorkbookHandler(configuration);
            Process = new ProcessHandler(configuration);
            UpdateDisplay(true, true);
        }
        private async void UpdateDisplay(bool download, bool firstLoad)
        {
            if (download)
            {
                try
                {
                    if (await Workbook.UpdateData(firstLoad)) UpdateValues();
                    else await DisplayAlertAsync("Connection Error", "Unable to download plan data. Please Restart and/or check your connection.", "Okay");
                }
                catch(Exception e) { Logger.WriteLog(e.Message, false); }
            }
            else UpdateValues();
        }
        private void UpdateValues()
        {
            SetSpecialNoteText();
            SelectedBox.IsChecked = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].Selected;
            ActiveMonth.Text = String.Format("{0} {1}", Workbook.Worksheets[Workbook.SheetIndex].Title, Workbook.WorkingYear);
            ActiveRow.Text = Workbook.RowIndex.ToString();
            LeadSourceTypeData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].LeadSourceType;
            LeadSourceNameData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].LeadSourceName;
            PreviousDescriptorJobCodeData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].PreviousDescriptorJobCode;
            VendorJobData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].VendorJobNumber;
            DecriptorJobCodeData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].DescriptorJobCode1;
            StateSelectionData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].StateSelection;
            IntrafileSuppresionDaysData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].DaysForIntrafileSuppression;
            AddSuppCriteriaData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].AddSuppressionCriteria;
            AddInfoData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].AddInfo;
            ControlTestData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].ControlTest;
            ControlDescriptionData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].ControlTestDescription;
            CTVariantNameData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].ControlTestVariantName;
            PlannedQTYData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].PlannedQty;
            Network1Data.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].Network1;
            Bin1Data.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].Bin1;
            PCN1Data.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].PCN1;
            GroupName1Data.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].GroupNumber1;
            MIDStart1Data.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].MemberIdStart1;
            MIDEnd1Data.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].MemberIdEnd1;
            Net1CQCallData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].Network1CustomerQ;
            Net1PhQCallData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].Network1PharmacistQ;
            ClassOfPostageData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].ClassOfPostage;
            CardShellCodeData.Text = Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].CardPreprintedShellCode;
        }
        private void SetSpecialNoteText()
        {
            SpecialNoteData.Text = "";
            if (Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].SecondNetwork)
            {
                SpecialNoteData.Text = "Secondary Network";
                if (Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].NoMemberIds) SpecialNoteData.Text += ", No member IDs";
            }
            else if (Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].NoMemberIds) SpecialNoteData.Text = "No member IDs";
        }
        private void MonthButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button) Workbook.SheetIndex += Int32.Parse(button.CommandParameter.ToString());
                if (Workbook.SheetIndex < 0)
                {
                    if (Workbook.YearIsAvailable(-1))
                    {
                        Workbook.ChangeYear(-1);
                        UpdateDisplay(true, false);
                    }
                    else Workbook.SheetIndex = 0;
                }
                else if (Workbook.SheetIndex >= Workbook.Worksheets.Count) 
                { 
                    if (Workbook.YearIsAvailable(1))
                    {
                        Workbook.ChangeYear(1);
                        UpdateDisplay(true, false);
                    }
                    else Workbook.SheetIndex = Workbook.Worksheets.Count - 1;
                }
                else
                {
                    Workbook.ChangeSheet();
                    UpdateDisplay(false, false);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message, false); }
        }
        private void RowButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button) Workbook.RowIndex += Int32.Parse(button.CommandParameter.ToString());
            if (Workbook.RowIndex <= 0) Workbook.RowIndex = 1;
            else if (Workbook.RowIndex >= Workbook.RowCount) Workbook.RowIndex = Workbook.RowCount - 1;
            UpdateDisplay(false, false);
        }
        private void SelectedBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Workbook.Worksheets[Workbook.SheetIndex].SheetRows[Workbook.RowIndex].Selected = e.Value;
        }
        private void FilePickerButton_Clicked(object sender, EventArgs e)
        {
            PickFilesToDecrypt();
        }
        private async Task PickFilesToDecrypt()
        {
            try
            {
                PickOptions options = new PickOptions { PickerTitle = "Select Files to Decrypt." };
                IEnumerable<FileResult> results = await FilePicker.Default.PickMultipleAsync(options);
                if (results != null && results.Any())
                {
                    FilesChosen.Text = "";
                    Process.ClearEncryptedPaths();
                    foreach (FileResult file in results)
                    {
                        FilesChosen.Text += " " + file.FileName + ";";
                        Process.AddEncryptedPath(file.FullPath);
                    }
                }
            }
            catch { await DisplayAlertAsync("I/O Error", "Failed to select files, program may need restart.", "Okay"); }
        }
        private async void ProcessButton_Clicked(object sender, EventArgs e)
        {
            if (!Preferences.Default.ContainsKey("gpg_path"))
            {
                await DisplayAlertAsync("Preferences Error", "GPG path not assigned, please see preferences.", "Okay");
                return;
            }
            else
            {
                List<Row> rowsToProcess = Workbook.GetRowsToProcess();
                if (rowsToProcess.Count == 0)
                {
                    await DisplayAlertAsync("Workbook Error", "No rows selected to work.", "Okay");
                    return;
                }
                if(rowsToProcess.Count > 1)
                {
                    string firstCOP = rowsToProcess[0].ClassOfPostage;
                    for (int i = 1; i < rowsToProcess.Count; i++) if (rowsToProcess[i].ClassOfPostage != firstCOP)
                    {
                        await DisplayAlertAsync("Workbook Error", "Class of Postage does not match accross rows.", "Okay");
                        return;
                    }
                }
                if (EnteredNewJobNumber(rowsToProcess[0].VendorJobNumber, JobNumberEntry.Text))
                {
                    if (!await DisplayAlertAsync("Confirm Job Number", "Are you sure you want to override the job number found on the plan?", "Yes", "No")) return;
                    else await RunProcess(rowsToProcess, JobNumberEntry.Text);
                }
                else
                {
                    if (!VerifyNetworks(rowsToProcess)) await DisplayAlertAsync("Plan Error", "Error in GoodRx plan spreadsheet.", "Okay");
                    else await RunProcess(rowsToProcess, rowsToProcess[0].VendorJobNumber);
                }
            }
        }
        private bool EnteredNewJobNumber(string planJobNumber, string enteredJobNumber)
        {
            if (enteredJobNumber != planJobNumber)
            {
                if (enteredJobNumber == string.Empty || enteredJobNumber == null || enteredJobNumber.Trim() == "") NewJobNumber = false;
                else NewJobNumber = true;
            }
            else NewJobNumber = false;
            return NewJobNumber;
        }
        private async Task RunProcess(List<Row> rowsToProcess, string JobNumber)
        {
            if (!Process.PrepareJobForProcessing(rowsToProcess, JobNumber, Workbook.WorkingYear))
            {
                await DisplayAlertAsync("Processing Error", "Failed to prepare job files/folders.", "Okay");
            }
            else
            {
                if (await GetJobBatches(rowsToProcess))
                {
                    if (Process.ProcessJob(rowsToProcess, JobNumber, NoMemberIdBox.IsChecked))
                    {
                        if (NewJobNumber) Workbook.UpdateGoogleSheet(Process.JobBatches, JobNumber);
                        else Workbook.UpdateGoogleSheet(Process.JobBatches);
                        await DisplayAlertAsync("Processing Complete!", "Job processing complete.", "Okay");
                        string archivePath = Preferences.Get("archive_path", "_");
                        if(archivePath != "_") for (int i = 0; i < Process.EncryptedPaths.Count; i++)
                                File.Move(Process.EncryptedPaths[i], Path.Combine(archivePath, Path.GetFileName(Process.EncryptedPaths[i])));
                        OpenReportWindow(Process.Report);
                    }
                    else await DisplayAlertAsync("Processing Error", "Job processing failed, check logs.", "Okay");
                }
            }
        }
        private async Task<bool> GetJobBatches(List<Row> rowsToProcess)
        {
            try
            {
                if (rowsToProcess.Count == 1 && Process.Parser.DecryptedFiles.Length == 1)
                {   //One data file and one sheet row.
                    Logger.WriteLog("Working a single row with a single data file.", false);
                    List<Record> records = Process.Parser.GetRecords(Process.Parser.DecryptedFiles[0]);
                    if (records == null)
                    {
                        await DisplayAlertAsync("Process Error", String.Format("Unable to extract records from {0}", Process.Parser.DecryptedFiles[0]), "Okay");
                        return false;
                    }
                    else FinishBatchPreparation(new JobBatch(rowsToProcess[0], records, Process.Parser.DecryptedFiles[0]));
                    if (Process.JobBatches.Count == 0) return false;
                    else return true;
                }
                else if (rowsToProcess.Count == 1)
                {   //One sheet row to more than one data file.
                    Logger.WriteLog("Working a single row with multiple data files.", false);
                    List<Record> records = new List<Record>();
                    for (int i = 0; i < Process.Parser.DecryptedFiles.Length; i++)
                    {
                        List<Record> fileRecords = Process.Parser.GetRecords(Process.Parser.DecryptedFiles[i]);
                        if (fileRecords == null)
                        {
                            await DisplayAlertAsync("Process Error", String.Format("Unable to extract records from {0}", Process.Parser.DecryptedFiles[i]), "Okay");
                            return false;
                        }
                        records.AddRange(fileRecords);
                    }
                    FinishBatchPreparation(new JobBatch(rowsToProcess[0], records, Process.Parser.DecryptedFiles[0]));
                    if (Process.JobBatches.Count == 0) return false;
                    else return true;
                }
                else if (Process.Parser.DecryptedFiles.Length == 1)
                {   //One data file to multiple sheet rows. (SHOULD NOT BE OCCURRING, BUT BUILT JUST IN CASE)
                    Logger.WriteLog("Selected multiple rows with a single data file.", false);
                    await DisplayAlertAsync("Setup Error", "Selected multiple rows with a single data file.", "Okay");
                    return false;
                }
                else
                {   //Multiple data files to multiple sheet rows
                    Logger.WriteLog("Working multiple rows with multiple data files.", false);
                    for(int i = 0; i < rowsToProcess.Count; i++)
                    {
                        string linkedData = await DisplayActionSheetAsync(String.Format("Select Data for {0}", Path.GetFileName(rowsToProcess[i].DescriptorJobCode1)), 
                            null, null, Process.Parser.DecryptedFiles);
                        List<Record> records = Process.Parser.GetRecords(linkedData);
                        if (records == null)
                        {
                            await DisplayAlertAsync("Process Error", String.Format("Unable to extract records from {0}", linkedData), "Okay");
                            return false;
                        }
                        else FinishBatchPreparation(new JobBatch(rowsToProcess[i], records, Process.Parser.DecryptedFiles[i]));
                    }
                    if (Process.JobBatches.Count < 2) return false;
                    else return true;
                }
            }
            catch
            {
                Logger.WriteLog("Failed to build job batches.", false);
                return false;
            }
        }
        private void SettingsButton_Clicked(object sender, EventArgs e)
        {
            var SettingsWindow = new Window(new SettingsPage())
            {
                Title = "Settings",
                Width = 800,
                Height = 500,
                X = 100,
                Y = 100
            };
            Application.Current?.OpenWindow(SettingsWindow);
        }
        private bool VerifyNetworks(List<Row> rowsToProcess)
        {
            for(int i = 0; i < rowsToProcess.Count; i++)
            {
                if (rowsToProcess[i].Network1 == "" && rowsToProcess[i].Network2 != "") return false;
            }
            return true;
        }
        private async void FinishBatchPreparation(JobBatch batch)
        {
            batch.MergeRows();
            if (!batch.CheckQuantities())
            {
                await DisplayAlertAsync("Process Error", "Member ID range is smaller than record count.", "Okay");
            }
            else Process.JobBatches.Add(batch);
        }
        private void OpenReportWindow(ProcessReport report)
        {
            report.AddErrorMessages();
            var ReportWindow = new Window(new ReportPage(report))
            {
                Title = "Process Report",
                Width = 1000,
                Height = 600,
                X = 100,
                Y = 100
            };
            Application.Current?.OpenWindow(ReportWindow);
        }
    }
}
