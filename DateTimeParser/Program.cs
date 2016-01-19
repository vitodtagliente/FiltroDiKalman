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

            bool toMillisecs = false;
            bool toSeconds = false;

            Console.WriteLine("To Milliseconds? [default no]: ");
            string value = Console.ReadLine();
            if(string.IsNullOrEmpty(value) == false)
            {
                toMillisecs = (value.ToLower() == "yes");
            }
            Console.WriteLine("To Seconds? [default no]: ");
            value = Console.ReadLine();
            if (string.IsNullOrEmpty(value) == false)
            {
                toSeconds = (value.ToLower() == "yes");
            }

            StreamReader rd = new StreamReader(filename);
            while(rd.EndOfStream == false)
            {
                string line = rd.ReadLine();
                var pieces = line.Split(',');
                var first_piece = pieces[0].Trim();

                if (toMillisecs)
                {
                    var date = DateTime.Parse(first_piece);
                    line = line.Replace(first_piece, date.TimeOfDay.TotalMilliseconds.ToString());
                }
                else if(toSeconds)
                {
                    var date = TimeSpan.FromMilliseconds(double.Parse(first_piece));
                    line = line.Replace(first_piece, date.Seconds.ToString());
                }
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
