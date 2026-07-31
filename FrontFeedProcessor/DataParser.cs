using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    public class DataParser
    {
        public string[] DecryptedFiles;
        public List<Record> WorkingRecords;
        
        public DataParser(string suppliedPath)
        {
            WorkingRecords = new List<Record>();
            string[] allFiles = Directory.GetFiles(suppliedPath);
            List<string> decryptedList = new List<string>();
            for(int i = 0; i< allFiles.Length; i++) if (!allFiles[i].Contains(".gpg")) decryptedList.Add(allFiles[i]);
            DecryptedFiles = decryptedList.ToArray();
        }
        public List<Record> GetRecords(string path)
        {
            List<Record> records = new List<Record>();
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(path));
            try
            {
                bool Okay = true;
                StreamReader sReader = new StreamReader(path);
                StreamWriter sWriter = new StreamWriter(tempPath);
                string header = sReader.ReadLine();
                int recordNumber = 0;
                while (!sReader.EndOfStream)
                {
                    string line = sReader.ReadLine();
                    if (!line.Contains("\""))
                    {
                        sWriter.WriteLine(line);
                        records.Add(new Record(line, recordNumber));
                    }
                    else
                    {
                        string newLine = TrimQuotes(line);
                        if (newLine == null)
                        {
                            Logger.WriteLog("Error removing quotes from {0}.", false, path);
                            Okay = false;
                            break;
                        }
                        records.Add(new Record(newLine, recordNumber));
                        sWriter.WriteLine(newLine);
                    }
                    recordNumber++;
                }
                sWriter.Close();
                sReader.Close();
                if (Okay)
                {
                    File.Move(tempPath, path, true);
                    return records;
                }   
                else
                {
                    File.Delete(tempPath);
                    return null;
                }
            }
            catch
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
                return null;
            }
        }
        private string TrimQuotes(string line)
        {
            string output = line;
            int startEncapsulation = -1;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    if (startEncapsulation < 0) startEncapsulation = i;
                    else 
                    {
                        output = GetNewLine(output, line.Substring(startEncapsulation, ((i - startEncapsulation) + 1)));
                        startEncapsulation = -1;
                    }
                }
            }
            return output;
        }
        private string GetNewLine(string line, string substring)
        {
            string newString = substring.Replace("\"", "").Trim();
            return line.Replace(substring, newString);
        }
    }
}
