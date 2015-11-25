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

        static void Main(string[] args)
        {
            Console.Title = "Kalman Server";

            defaultColor = Console.ForegroundColor;

            LogLine("Configuration...", ConsoleColor.Cyan);

            Console.WriteLine(string.Empty);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;

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

            NetHost server = new NetHost(string.Empty, port);
            server.Init();


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
