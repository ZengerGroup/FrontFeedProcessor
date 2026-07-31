using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FrontFeedProcessor
{
    public class Record
    {
        public string SignupTime;
        public string LeadKey;
        public string GoodRxId;
        public string FirstName;
        public string LastName;
        public string Address;
        public string Address2;
        public string City;
        public string State;
        public string Zip;
        public string LeadSourceName;
        public string Network;
        public string ToMail;
        public string BIN;
        public string PCN;
        public string Group;
        public string MemberId0;
        public string MemberId1;
        public string NewtorkAlt;
        public string Network2;
        public string BIN2;
        public string PCN2;
        public string Group2;
        public string DescriptorJobCode;
        public string DescriptorJobCode2;
        public string LeadSourceTypeZG;
        public string LeadSourceNameZG;
        public int RecordNumber;
        public Record(string unparsedLine, int recordNumber)
        {
            string[] splitLine = unparsedLine.Replace("\\", "").Replace("|", "").Replace(",","").Split("\t");
            SignupTime = splitLine[0];
            LeadKey = splitLine[1];
            GoodRxId = splitLine[2];
            FirstName = splitLine[3];
            LastName = splitLine[4];
            Address = splitLine[5];
            Address2 = splitLine[6];
            City = splitLine[7];
            State = splitLine[8];
            Zip = splitLine[9];
            LeadSourceName = splitLine[10];
            Network = splitLine[11];
            RecordNumber = recordNumber;
        }
        public void PrintToFile(string path)
        {
            string fileName = Path.Combine(path, String.Format("{0}.txt", LeadKey));
            File.AppendAllText(fileName, String.Format("Signup Time: {0}{1}", SignupTime, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("Lead Key: {0}{1}", LeadKey, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("GoodRx ID: {0}{1}", GoodRxId, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("First Name: {0}{1}", FirstName, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("Last Name: {0}{1}", LastName, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("Address: {0}{1}", Address, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("Address 2: {0}{1}", Address2, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("City: {0}{1}", City, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("State: {0}{1}", State, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("Zip: {0}{1}", Zip, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("Lead Source Name: {0}{1}", LeadSourceName, Environment.NewLine));
            File.AppendAllText(fileName, String.Format("Network: {0}{1}", Network, Environment.NewLine));
        }
        public void SetMemberId(string memberId1, string memberId2)
        {
            string mem1NoChars = Regex.Replace(memberId1, "[A-Za-z]*", "");
            string mem1NoDigits = Regex.Replace(memberId1, "[0-9]*", "");
            int mem1Int = Convert.ToInt32(mem1NoChars) + RecordNumber;
            MemberId0 = String.Format("{0}{1}", mem1NoDigits, mem1Int);
            if(memberId2 != "")
            {
                string mem2NoChars = Regex.Replace(memberId2, "[A-Za-z]*", "");
                string mem2NoDigits = Regex.Replace(memberId2, "[0-9]*", "");
                int mem2Int = Convert.ToInt32(mem2NoChars) + RecordNumber;
                MemberId1 = String.Format("{0}{1}", mem2NoDigits, mem2Int);
            }
        }
    }
}
