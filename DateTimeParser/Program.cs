using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateTimeParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            string filename = args[0];

            StringBuilder lines = new StringBuilder();

            StreamReader rd = new StreamReader(filename);
            while(rd.EndOfStream == false)
            {
                string line = rd.ReadLine();
                string date = line.Substring(0, DateTime.Now.ToString("HH:mm:ss.fff").Length);
                line = line.Replace(date, DateTime.Parse(date).TimeOfDay.TotalMilliseconds.ToString());
                lines.AppendLine(line);
            }
            rd.Close();
            rd.Dispose();
            
            StreamWriter wr = new StreamWriter(filename);
            wr.Write(lines.ToString());
            wr.Close();
            wr.Dispose();
        }
    }
}
