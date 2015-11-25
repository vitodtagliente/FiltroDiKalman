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
                    throw new Exception(e.ToString());
                }
            }
            else port = DefaultPort;

            Console.WriteLine(string.Empty);

            UdpClient server = new UdpClient(port);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

            LogLine("Listening...", ConsoleColor.Cyan);
            Console.WriteLine(string.Empty);

            while(true)
            {
                byte[] bytes = server.Receive(ref endPoint);

                Console.WriteLine("Received packet from {0} : {1}",
                    endPoint.ToString(),
                    Encoding.ASCII.GetString(bytes, 0, bytes.Length));
            }

            Console.ReadKey();
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
    }
}
