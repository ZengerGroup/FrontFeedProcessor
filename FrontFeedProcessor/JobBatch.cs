using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FrontFeedProcessor
{
    public class JobBatch
    {
        public string DataFileName;
        public List<Row> BatchRows;
        public List<Record> BatchRecords;
        public Row MergedRow;
        public JobBatch()
        {
            BatchRows = new List<Row>();
            BatchRecords = new List<Record>();
        }
        public JobBatch(Row batchRow, List<Record> batchRecords, string dataFileName)
        {
            DataFileName = Path.GetFileNameWithoutExtension(dataFileName);
            BatchRows = new List<Row>();
            BatchRows.Add(batchRow);
            BatchRecords = batchRecords;
        }
        public JobBatch(List<Row> batchRows, List<Record> batchRecords, string dataFileName)
        {
            DataFileName = dataFileName;
            BatchRows = batchRows;
            BatchRecords = batchRecords;
        }
        public void MergeRows()
        {
            if (BatchRows.Count == 1) MergedRow = BatchRows[0];
            else
            {
                //something something something... darkside.
            }
        }
        public bool CheckQuantities()
        {
            int availableMemberIds = GetAvailableIds();
            return availableMemberIds >= BatchRecords.Count;
        }
        private int GetAvailableIds()
        {
            int start1 = Convert.ToInt32(Regex.Replace(MergedRow.MemberIdStart1, "[A-Za-z]*", ""));
            int end1 = Convert.ToInt32(Regex.Replace(MergedRow.MemberIdEnd1, "[A-Za-z]*", ""));
            return end1 - start1;
        }
    }
}
