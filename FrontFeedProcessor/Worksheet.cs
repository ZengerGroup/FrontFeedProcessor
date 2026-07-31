using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    class Worksheet
    {
        public Row[] SheetRows;
        public string Title;
        public Worksheet(string title, IList<IList<object>> rows)
        {
            Title = title;
            SheetRows = GetRows(rows);
        }
        private Row[] GetRows(IList<IList<object>> rowList)
        {
            List<Row> rows = new List<Row>();
            for(int i =0; i < rowList.Count; i++)
            {
                if (rowList[i].Count < 58) continue;
                //else File.AppendAllText(@"C:\Code\TestingFS\ROWINDEXCOUNT.txt", String.Format("{0} - {1} - {2}{3}",Title,i.ToString(), rowList[i].Count.ToString(),Environment.NewLine));
                rows.Add(new Row(rowList[i], Title));
            }
            return rows.ToArray();
        }
    }
}
