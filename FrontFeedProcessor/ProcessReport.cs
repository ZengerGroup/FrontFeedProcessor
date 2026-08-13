using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    public class ProcessReport
    {
        public string MailingSegment;
        public string ClassOfPostage;
        public string ZGJobNumber;
        public string ReportPath;
        public string OtherWarnings;
        public List<Dictionary<string, string>> BatchDetails;
        public ProcessReport(ProcessHandler process)
        {
            MailingSegment = process.MailingSegment;
            ClassOfPostage = process.JobBatches[0].MergedRow.ClassOfPostage;
            ZGJobNumber = process.ZGJobNumber;
            ReportPath = Path.Combine(process.WorkingPath, String.Format("{0}({1})_FF Report.txt", MailingSegment, ZGJobNumber));
            OtherWarnings = "";
            GenerateBatchDetails(process.JobBatches);
            if (!GenerateReportFile()) OtherWarnings = "Failed to generate report file!";
        }
        private void GenerateBatchDetails(List<JobBatch> batches) 
        {
            BatchDetails = new List<Dictionary<string, string>>();
            for (int i = 0; i < batches.Count; i++)
            {
                BatchDetails.Add(new Dictionary<string, string>());
                BatchDetails[i].Add("Descriptor Job Code", batches[i].MergedRow.DescriptorJobCode1);
                BatchDetails[i].Add("Final Record Count", batches[i].BatchRecords.Count.ToString());
                BatchDetails[i].Add("Days for Intrafile Suppression", batches[i].MergedRow.DaysForIntrafileSuppression);
                BatchDetails[i].Add("Additional Suppression Criteria", batches[i].MergedRow.AddSuppressionCriteria);
                BatchDetails[i].Add("Additional Info", batches[i].MergedRow.AddInfo);
                BatchDetails[i].Add("State Selection", batches[i].MergedRow.StateSelection);
                BatchDetails[i].Add("Network", batches[i].MergedRow.Network1);
            }
        }
        private bool GenerateReportFile()
        {
            try
            {
                StreamWriter sWriter = new StreamWriter(ReportPath);
                sWriter.WriteLine("SUMMARY:");
                sWriter.WriteLine(String.Format("Mailing Segment:\t{0}", MailingSegment));
                sWriter.WriteLine(String.Format("Class of Postage:\t{0}", ClassOfPostage));
                sWriter.WriteLine(String.Format("Zenger Job Number:\t{0}", ZGJobNumber));
                for(int i = 0; i < BatchDetails.Count; i++)
                {
                    sWriter.WriteLine("------------------------------------------------------------------------");
                    foreach(var kvp in BatchDetails[i])
                    {
                        sWriter.WriteLine(String.Format("{0}:\t{1}", kvp.Key, kvp.Value));
                    }
                }
                sWriter.Close();
                return true;
            }
            catch { return false; }
        }
        //ADD ARCHIVE (BELOW) TO SETTINGS PAGE (default to current archive location)
        //ARCHIVE PGP ON FTP FILES WHEN REPORT IS BUILT (..incoming_data/Direct_Mail/Archive)
    }
}
