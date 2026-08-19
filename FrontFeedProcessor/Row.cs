using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    public class Row
    {
        //From plan
        public string Segment;
        public string LeadSourceType;
        public string LeadSourceName;
        public string PreviousDescriptorJobCode;
        public string VendorJobNumber;
        public string DescriptorJobCode1;
        public string DescriptorJobCode2;
        public string StateSelection;
        public string DaysForIntrafileSuppression;
        public string AddSuppressionCriteria;
        public string AddInfo;
        public string ControlTest;
        public string ControlTestDescription;
        public string ControlTestVariantName;
        public string PlannedQty;
        public string ActualQty;
        public string QtyInProduction;
        public string Network1;
        public string Bin1;
        public string PCN1;
        public string GroupNumber1;
        public string MemberIdStart1;
        public string MemberIdEnd1;
        public string Network1CustomerQ;
        public string Network1PharmacistQ;
        public string Network2;
        public string Bin2;
        public string PCN2;
        public string GroupNumber2;
        public string MemberIdStart2;
        public string MemberIdEnd2;
        public string Network2CustomerQ;
        public string Network2PharmacistQ;
        public string GrxExportDate;
        public string DataToVendorActual;
        public string DataToVendorFrom;
        public string DateProofsApproved;
        public string ProductionCompleteDate;
        public string DateMailed;
        public string DaysAtVendor;
        public string PackageDescription;
        public string OuterEnvelopeId;
        public string EnvelopeCopy;
        public string ClassOfPostage;
        public string LetterPreprintedShellCode;
        public string LetterImprintCode;
        public string CardPreprintedShellCode;
        public string Insert1Code;
        public string Insert2Code;
        public string Insert3Code;
        public string PlannedMailingCPP;
        public string PlannedMailingTotalCost;
        public string EstimatedPostage;
        public string VendorCPP;
        public string VendorTotalCost;
        public string InvoiceReceivedData;
        public string Notes;
        public string PO;
        //Functional or Abstracted
        public bool Selected;
        public bool SecondNetwork;
        public bool NoMemberIds;
        public string Month;
        public int RowNumber;

        public Row(IList<object> rowList, string month, int rowNumber)
        {
            Segment = rowList[0].ToString() ?? "void";
            LeadSourceType = rowList[1].ToString() ?? "void";
            LeadSourceName = rowList[2].ToString() ?? "void";
            PreviousDescriptorJobCode = rowList[3].ToString() ?? "void";
            VendorJobNumber = rowList[4].ToString() ?? "void";
            DescriptorJobCode1 = rowList[5].ToString() ?? "void";
            DescriptorJobCode2 = rowList[6].ToString() ?? "void";
            StateSelection = rowList[7].ToString() ?? "void";
            DaysForIntrafileSuppression = rowList[8].ToString() ?? "void";
            AddSuppressionCriteria = rowList[9].ToString() ?? "void";
            AddInfo = rowList[10].ToString() ?? "void";
            ControlTest = rowList[11].ToString() ?? "void";
            ControlTestDescription = rowList[12].ToString() ?? "void";
            ControlTestVariantName = rowList[13].ToString() ?? "void";
            PlannedQty = rowList[14].ToString() ?? "void";
            ActualQty = rowList[15].ToString() ?? "void";
            QtyInProduction = rowList[16].ToString() ?? "void";
            Network1 = rowList[17].ToString() ?? "void";
            Bin1 = rowList[18].ToString() ?? "void";
            PCN1 = rowList[19].ToString() ?? "void";
            GroupNumber1 = rowList[20].ToString() ?? "void";
            MemberIdStart1 = rowList[21].ToString() ?? "void";
            MemberIdEnd1 = rowList[22].ToString() ?? "void";
            Network1CustomerQ = rowList[23].ToString() ?? "void";
            Network1PharmacistQ = rowList[24].ToString() ?? "void";
            Network2 = rowList[25].ToString() ?? "void";
            Bin2 = rowList[26].ToString() ?? "void";
            PCN2 = rowList[27].ToString() ?? "void";
            GroupNumber2 = rowList[28].ToString() ?? "void";
            MemberIdStart2 = rowList[29].ToString() ?? "void";
            MemberIdEnd2 = rowList[30].ToString() ?? "void";
            Network2CustomerQ = rowList[31].ToString() ?? "void";
            Network2PharmacistQ = rowList[32].ToString() ?? "void";
            GrxExportDate = rowList[33].ToString() ?? "void";
            DataToVendorActual = rowList[34].ToString() ?? "void";
            DataToVendorFrom = rowList[35].ToString() ?? "void";
            DateProofsApproved = rowList[36].ToString() ?? "void";
            ProductionCompleteDate = rowList[37].ToString() ?? "void";
            DateMailed = rowList[38].ToString() ?? "void";
            DaysAtVendor = rowList[39].ToString() ?? "void";
            PackageDescription = rowList[40].ToString() ?? "void";
            OuterEnvelopeId = rowList[41].ToString() ?? "void";
            EnvelopeCopy = rowList[42].ToString() ?? "void";
            ClassOfPostage = rowList[43].ToString() ?? "void";
            LetterPreprintedShellCode = rowList[44].ToString() ?? "void";
            LetterImprintCode = rowList[45].ToString() ?? "void";
            CardPreprintedShellCode = rowList[46].ToString() ?? "void";
            Insert1Code = rowList[47].ToString() ?? "void";
            Insert2Code = rowList[48].ToString() ?? "void";
            Insert3Code = rowList[49].ToString() ?? "void";
            PlannedMailingCPP = rowList[50].ToString() ?? "void";
            PlannedMailingTotalCost = rowList[51].ToString() ?? "void";
            EstimatedPostage = rowList[52].ToString() ?? "void";
            VendorCPP = rowList[53].ToString() ?? "void";
            VendorTotalCost = rowList[54].ToString() ?? "void";
            InvoiceReceivedData = rowList[55].ToString() ?? "void";
            Notes = rowList[56].ToString() ?? "void";
            PO = rowList[57].ToString() ?? "void";
            //Getting truthy with it.
            Selected = false;
            NoMemberIds = (MemberIdStart1 == "");
            SecondNetwork = (DescriptorJobCode2 != "");
            Month = month;
            RowNumber = rowNumber;
        }
    }
}
