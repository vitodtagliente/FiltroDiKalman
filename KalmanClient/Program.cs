using System;
using System.Net;
using System.Net.Sockets;
using KalmanLib;

namespace KalmanClient
{
    class Program
    {
        static readonly string DefaultAddress = "127.0.0.1";
        static readonly int DefaultPort = 55655;
        
        static ConsoleColor defaultColor;

        static void Main(string[] args)
        {
            Console.Title = "Kalman Client";
            
            defaultColor = Console.ForegroundColor;

            LogLine("Configuration...", ConsoleColor.DarkCyan);

            Console.WriteLine(string.Empty);
            
            string ip = string.Empty;
            Log("Receiver IP [default " + DefaultAddress + "]: ", ConsoleColor.DarkMagenta);
            ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip)) ip = DefaultAddress;
            
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

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
            ProtocolType.Udp);

            IPAddress Address = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(Address, port);
            
            int duration = 0;
            Log("Send Duration [default 10 seconds]: ", ConsoleColor.DarkGreen);
            string _duration = Console.ReadLine();
            if (!string.IsNullOrEmpty(_duration))
            {
                try
                {
                    duration = int.Parse(_duration);
                }
                catch (Exception e)
                {
                    LogLine("Invalid format, duration must be an integer number!", ConsoleColor.DarkRed);
                    throw new Exception(e.ToString());
                }
            }
            else duration = 10;

            int delay = 0;
            Log("Delay [default 1000 milli seconds ( = 1 second)]: ", ConsoleColor.DarkGreen);
            string _delay = Console.ReadLine();
            if (!string.IsNullOrEmpty(_delay))
            {
                try
                {
                    delay = int.Parse(_delay);
                }
                catch (Exception e)
                {
                    LogLine("Invalid format, delay must be an integer number!", ConsoleColor.DarkRed);
                    throw new Exception(e.ToString());
                }
            }
            else delay = 1000;

            int amplitude = 32;
            Log("Amplitude [default 32 bytes]: ", ConsoleColor.DarkGreen);
            string _amplitude = Console.ReadLine();
            if (!string.IsNullOrEmpty(_amplitude))
            {
                amplitude = int.Parse(_amplitude);
            }

            PacketGenerationService sineService = new SineGenerationService(amplitude);
            StressSendService sendService = new StressSendService(s, endPoint, sineService);

            Console.WriteLine(string.Empty);
            LogLine("Sending data...", ConsoleColor.DarkCyan);

            sendService.Start(duration, delay);

            Console.WriteLine(string.Empty);
            Console.WriteLine("Press a key to quit...");
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
