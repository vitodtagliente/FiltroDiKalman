using System;
using System.Collections.Generic;
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

        public StressSendService(Socket socket, EndPoint endPoint, PacketGenerationService service)
        {
            Socket = socket;
            LocalEndPoint = endPoint;

            Service = service;
            EndOfStream = false;
        }

        public virtual void Start(int duration, int rate = 1)
        {
            Duration = duration;
            Rate = rate;

            var now = DateTime.Now;
            var end = now.AddSeconds(Duration);

            var next = now.AddSeconds(Rate);

            while(!EndOfStream)
            {
                now = DateTime.Now;

                if(now >= next)
                {
                    Socket.SendTo(Service.Generate(), LocalEndPoint);
                    next = now.AddSeconds(Rate);
                }

                if (now >= end)
                    EndOfStream = true;
            }
        }
    }
}
