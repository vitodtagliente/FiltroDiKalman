using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KalmanLib
{
    public class StressSendService
    {
        public PacketGenerationService Service { get; private set; }
        public bool EndOfStream { get; private set; }

        public Socket Socket { get; private set; }
        public EndPoint LocalEndPoint { get; private set; }

        public int Rate { get; private set; }
        public int Duration { get; private set; }
        
        static string Filename = "client.log.txt";

        public StressSendService(Socket socket, EndPoint endPoint, PacketGenerationService service)
        {
            Socket = socket;
            LocalEndPoint = endPoint;

            Service = service;
            EndOfStream = false;
        }

        public virtual void Start(int duration, int rate = 1000)
        {
            var log = new StreamWriter(Filename);

            Duration = duration;
            Rate = rate;

            var now = DateTime.Now;
            var end = now.AddSeconds(Duration);

            var next = now.AddMilliseconds(Rate);

            while (!EndOfStream)
            {
                now = DateTime.Now;

                if(now >= next)
                {
                    var bytes = Service.Generate();
                    Socket.SendTo(bytes, LocalEndPoint);

                    next = now.AddMilliseconds(Rate);
                    
                    // Log CSV
                    // Timestamp(in millisecondi) , bitrare (bytes/millisecondi)
                    StringBuilder line = new StringBuilder();

                    line.Append(DateTime.Now.TimeOfDay.TotalMilliseconds);
                    line.Append(" , ");
                    line.Append((bytes.Length / (double)Rate).ToString());

                    // bitrate = L / Rate

                    log.WriteLine(line.ToString());
                    // Aggiornamento del file di log
                }

                if (now >= end)
                    EndOfStream = true;
            }

            log.Close();
            log.Dispose();
        }
    }
}
