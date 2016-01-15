using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

            log.WriteLine("timestamp|dim(packet)|bit rate");

            while(!EndOfStream)
            {
                now = DateTime.Now;

                if(now >= next)
                {
                    var bytes = Service.Generate();
                    Socket.SendTo(bytes, LocalEndPoint);
                    next = now.AddMilliseconds(Rate);

                    StringBuilder line = new StringBuilder();

                    line.Append(DateTime.Now.ToString("HH:mm:ss"));
                    line.Append("|");
                    line.Append(bytes.Length);
                    line.Append("|");
                    line.Append(bytes.Length / Rate);

                    // rate = C / T => C = rate * T

                    log.WriteLine(line.ToString());
                }

                if (now >= end)
                    EndOfStream = true;
            }

            log.Close();
            log.Dispose();
        }
    }
}
