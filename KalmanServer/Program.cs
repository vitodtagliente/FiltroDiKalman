using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using KalmanLib;

namespace KalmanServer
{
    class Program
    {
        static ConsoleColor defaultColor;

        static readonly int DefaultPort = 55655;
        
        static void Main(string[] args)
        {
            Console.Title = "Kalman Server";
            
            defaultColor = Console.ForegroundColor;

            LogLine("Configuration...", ConsoleColor.DarkCyan);

            Console.WriteLine(string.Empty);

            int port = 0;
            Log("Receiver Port [default " + DefaultPort.ToString() + "]: ", ConsoleColor.DarkMagenta);
            string _port = Console.ReadLine();
            if (!string.IsNullOrEmpty(_port))
            {
                try
                {
                    port = int.Parse(_port);
                }
                catch (Exception e)
                {
                    LogLine("Invalid format, port must be an integer number!", ConsoleColor.DarkRed);
                    return;
                }
            }
            else port = DefaultPort;

            Console.WriteLine(string.Empty);

            UdpClient server = new UdpClient(port);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                       
            KalmanFilter filter = new KalmanFilter();

            Log("Bandwidth C [default " + filter.C.ToString() + " kbps]: ", ConsoleColor.Green);
            string _cCanale = Console.ReadLine();
            if (!string.IsNullOrEmpty(_cCanale))
            {
                filter.C = float.Parse(_cCanale);
            }

            LogLine(string.Empty);
            LogLine("Kalman Filter settings at time t=0", ConsoleColor.DarkGreen);
            LogLine(string.Empty);
            Log("m (one way delay variation): ", ConsoleColor.DarkGreen);
            LogLine(filter.m.ToString(), ConsoleColor.White);
            Log("sigma (measurement error): ", ConsoleColor.DarkGreen);
            LogLine(filter.sigma.ToString(), ConsoleColor.White);

            LogLine(string.Empty);
            LogLine("Listening...", ConsoleColor.Cyan);
            LogLine(string.Empty);
            
            while(true)
            {
                byte[] bytes = server.Receive(ref endPoint);

                string packet = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                Console.WriteLine(string.Empty);

                Console.WriteLine("Received packet from {0} on {1}", endPoint.ToString(), DateTime.Now.TimeOfDay.ToString());

                filter.NextStep(bytes);
                LogLine("Estimated link capacity: " + ((filter.C * 8000) / Math.Pow(2, 30)) + "Gbps", ConsoleColor.Yellow);
            }
        }        
                
        static void Log(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = defaultColor;
        }

        static void LogLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = defaultColor;
        }

        static void LogLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
