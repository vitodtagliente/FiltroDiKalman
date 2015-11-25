using System;
using System.Text;
using KalmanLib;

namespace KalmanClient
{
    class Program
    {
        static ConsoleColor defaultColor;

        static void Main(string[] args)
        {
            Console.Title = "Kalman Client";

            defaultColor = Console.ForegroundColor;

            LogLine("Configuration...", ConsoleColor.Cyan);

            Console.WriteLine(string.Empty);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            string ip = string.Empty;
            Console.Write("Receiver IP [default " + NetHost.DefaultAddress + "]: ");
            ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip)) ip = NetHost.DefaultAddress;

            Console.WriteLine(string.Empty);

            int port = 0;
            Console.WriteLine("Receiver Port [default " + NetHost.DefaultPort.ToString() + "]: ");
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
            else port = NetHost.DefaultPort;

            Console.WriteLine(string.Empty);

            NetHost client = new NetHost(ip, port);
            client.Init();

            PacketGenerationService service;

            client.Send(Encoding.ASCII.GetBytes("ciao"));

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
