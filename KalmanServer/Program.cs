using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using KalmanLib;
using LinearAlgebra;
using LinearAlgebra.Matricies;
using System.Threading;

namespace KalmanServer
{
    class Program
    {
        static ConsoleColor defaultColor;

        static readonly int DefaultPort = 55655;

        public static bool running = false;

        static KalmanFilter filter;

        static IPEndPoint endPoint;

        static UdpClient server;

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
                    throw new Exception(e.ToString());
                }
            }
            else port = DefaultPort;

            Console.WriteLine(string.Empty);

            server = new UdpClient(port);
            endPoint = new IPEndPoint(IPAddress.Any, port);
                       
            filter = new KalmanFilter();

            Log("Bandwidth C [default " + filter.C.ToString() + " kbps]: ", ConsoleColor.Green);
            string _cCanale = Console.ReadLine();
            if (!string.IsNullOrEmpty(_cCanale))
            {
                filter.C = float.Parse(_cCanale);
            }

            LogLine(string.Empty);
            LogLine("Kalman Filter settings at time t=0", ConsoleColor.DarkGreen);
            LogLine(string.Empty);
            LogLine("  P: ", ConsoleColor.DarkGreen);
            LogLine(filter.P.ToString(), ConsoleColor.White);
            LogLine("  Q: ", ConsoleColor.DarkGreen);
            LogLine(filter.Q.ToString(), ConsoleColor.White);
            Log("  m (one way delay variation): ", ConsoleColor.DarkGreen);
            LogLine(filter.m.ToString(), ConsoleColor.White);
            Log("  sigma (measurement error): ", ConsoleColor.DarkGreen);
            LogLine(filter.sigma.ToString(), ConsoleColor.White);

            LogLine(string.Empty);
            LogLine("Listening...", ConsoleColor.Cyan);
            LogLine(string.Empty);
            /*
            DoubleMatrix H = new DoubleMatrix(new double[,] { { 56, 1 } });
            LogLine(H.ToString(), ConsoleColor.Red);
            LogLine(H[0, 0].ToString(), ConsoleColor.Red);
            */
            
            while(true)
            {
                byte[] bytes = server.Receive(ref endPoint);

                string packet = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                Console.WriteLine(string.Empty);

                Console.WriteLine("Received packet from {0} : {1}",
                    endPoint.ToString(),
                    packet);

                filter.NextStep(bytes);
                filter.LogResults(ConsoleColor.Yellow);
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
