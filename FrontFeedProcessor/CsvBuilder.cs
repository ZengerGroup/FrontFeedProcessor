using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    public class CsvBuilder
    {
        bool NoMemberIds;
        string OutputPath;
        string MailingSegment;
        string ZGJobNumber;
        string Header;
        JobBatch Batch;
        JobBatch[] Batches;
        GrxIdAppentionHandler AppentionHandler;
        private Dictionary<string, string> DateStrings;
        public CsvBuilder(List<JobBatch> batches, bool dontAssignMemberId, string outputPath, string mailingSegment, string jobNumber, GrxIdAppentionHandler handler)
        {
            ZGJobNumber = jobNumber;
            DateStrings = GenerateDateStrings();
            OutputPath = outputPath;
            NoMemberIds = dontAssignMemberId;
            Batches = batches.ToArray();
            MailingSegment = mailingSegment;
            Header = GenerateHeader();
            AppentionHandler = handler;
            ProcessAllBatches();
        }
        private string GenerateHeader()
        {
            return "signup_time,lead_key,goodrx_id,first_name,last_name,address,address_2,city,state,zip,lead_source_name,network,to_mail,BIN,PCN,GROUP,"
                + "MEMBER_ID_1,MEMBER_ID_2,Network Alt,network_2,BIN_2,PCN_2,GROUP_2,DESCRIPTOR_JOB_CODE,DESCRIPTOR_JOB_CODE_2,LEAD_SOURCE_TYPE_ZG,"
                + "lead_source_name_ZG,ZGJobNo,Mailing Segment,State Selection,IntrafileSuppression,Control_Test,Control_Test Description,Control_Test Variant Name"
                + ",Network 1 Customer Questions Call:,Network 1 Pharmacist Questions Call:,Member ID Start 2,Member ID End 2,"
                + "Network 2 Customer Questions Call:,Network 2 Pharmacist Questions Call:,Class of Postage,Outer Envelope ID #,Letter Preprinted Shell Code,"
                + "Letter Imprint Code,Card Preprinted Shell Code,Insert 1 Code,Insert 2 Code,Insert 3 Code,CardsInMailer,PackageID,EntryDate,DrugDate_numeric,"
                + "DrugPricingDate,Select_or_all_zips,Final Job Name";
        }
        private void ProcessAllBatches()
        {
            Logger.WriteLog("Expecting to process {0} batches.", false, Batches.Length.ToString());
            StreamWriter sWriter = new StreamWriter(OutputPath);
            sWriter.WriteLine(Header);
            for(int i = 0; i < Batches.Length; i++) ProcessIndividualBatch(Batches[i], sWriter);
            sWriter.Close();
            AppentionHandler.Close();
            Logger.WriteLog("Processing complete.", false);
        }
        private void ProcessIndividualBatch(JobBatch batch, StreamWriter streamWriter) 
        {
            AppentionHandler.StartRow();
            for(int i = 0; i < batch.BatchRecords.Count; i++)
            {
                batch.BatchRecords[i].SetMemberId(batch.MergedRow.MemberIdStart1, batch.MergedRow.MemberIdStart2);
                string line = BuildLine(batch.BatchRecords[i], batch.MergedRow, NoMemberIds);
                streamWriter.WriteLine(line);
            }
            if (AppentionHandler.GeneratedNewIds()) AppentionHandler.AddRowToSheet(batch.MergedRow.DescriptorJobCode1, batch.MergedRow.LeadSourceName, ZGJobNumber);
            AppentionHandler.CloseRow();
        }
        private string BuildLine(Record record, Row row, bool noMemberId)
        {
            string packageId = "GRX-1CPM"; //DEFAULTING FOR NOW BUT MAY ADD LOGIC LATER
            string selectOrAllZips = "NO ZIP SELECTS"; //DEFAULTING FOR NOW BUT MAY ADD LOGIC LATER
            string classOfPostage = (row.ClassOfPostage.ToLower().Contains("standard")) ? "STD" : "FCM";
            string outLine = AddCustomerInfo(record);
            outLine += AddNetworkInfo(row, record, noMemberId);
            outLine += AddPostageInfo(row, record, classOfPostage, packageId, selectOrAllZips, GetCardsInMailing(row));
            return outLine;
        }
        private string AddCustomerInfo(Record record)
        {
            if (record.GoodRxId == "") record.GoodRxId = AppentionHandler.GetNewGoodRxId();
            return $"{record.SignupTime},{record.LeadKey},{record.GoodRxId},{record.FirstName},{record.LastName},{record.Address},{record.Address2},"
                + $"{record.City},{record.State},{record.Zip},{record.LeadSourceName},";
        }
        private string AddNetworkInfo(Row row, Record record, bool  noMemberId)
        {
            string net1 = (noMemberId) ? "" : row.Network1;
            string bin1 = (noMemberId) ? "" : row.Bin1;
            string pcn1 = (noMemberId) ? "" : row.PCN1;
            string group1 = (noMemberId) ? "" : row.GroupNumber1;
            string net2 = (noMemberId) ? "" : row.Network2;
            string bin2 = (noMemberId) ? "" : row.Bin2;
            string pcn2 = (noMemberId) ? "" : row.PCN2;
            string group2 = (noMemberId) ? "" : row.GroupNumber2;
            string mem1start = (noMemberId) ? "" : record.MemberId0;
            string mem2start = (noMemberId) ? "" : record.MemberId1;
            string mem2end = (noMemberId) ? "" : row.MemberIdEnd2;
            return $"{net1},{record.ToMail},{bin1},{pcn1},{group1},{mem1start},{mem2start},{record.Network},{net2},{bin2},{pcn2},{group2},"
                + $"{row.DescriptorJobCode1},{row.DescriptorJobCode2},{row.LeadSourceType},{row.LeadSourceName},{ZGJobNumber},{MailingSegment},"
                + $"{row.StateSelection},{row.DaysForIntrafileSuppression},{row.ControlTest},{row.ControlTestDescription},{row.ControlTestVariantName},"
                + $"{row.Network1CustomerQ},{row.Network1PharmacistQ},{mem2start},{mem2end},{row.Network2CustomerQ},{row.Network2PharmacistQ},";
        }
        private string AddPostageInfo(Row row, Record record, string classOfPostage, string packageId, string selectOrAllZips, string cardsInMailing)
        {
            return $"{classOfPostage},{row.OuterEnvelopeId},{row.LetterPreprintedShellCode},{row.LetterImprintCode},{row.CardPreprintedShellCode},{row.Insert1Code},"
                + $"{row.Insert2Code},{row.Insert3Code},{cardsInMailing},{packageId},{DateStrings[row.Month]},{DateStrings[row.Month]},"
                + $"{GetMonthYear(row.Month)},{selectOrAllZips},{String.Format("{0}({1})",MailingSegment,ZGJobNumber)}";
        }
        private string GetCardsInMailing(Row row)
        {
            if (row.Network2 != "") return "2";
            else
            {
                if (row.Network1 != "") return "1";
                else return "0";
            }
        }
        private string GetMonthYear(string month)
        {
            return String.Format("{0} {1}", month, DateTime.Now.ToString("yyyy"));
        }
        private Dictionary<string, string> GenerateDateStrings()
        {
            Logger.WriteLog("Generating date strings.", false);
            Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
            tempDictionary.Add("January", String.Format("01/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("Febuary", String.Format("02/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("March", String.Format("03/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("April", String.Format("04/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("May", String.Format("05/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("June", String.Format("06/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("July", String.Format("07/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("August", String.Format("08/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("September", String.Format("09/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("October", String.Format("10/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("November", String.Format("11/01/{0}", DateTime.Now.ToString("yyyy")));
            tempDictionary.Add("December", String.Format("12/01/{0}", DateTime.Now.ToString("yyyy")));
            Logger.WriteLog("Finished generating date strings.", false);
            return tempDictionary;
        }
    }
}
