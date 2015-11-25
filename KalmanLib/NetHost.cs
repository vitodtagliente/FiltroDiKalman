using System;
using System.Net;
using System.Net.Sockets;

namespace KalmanLib
{
    public class NetHost
    {
        public static string DefaultAddress = "127.0.0.1";
        public static int DefaultPort = 55655;

        string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                if (!init)
                    _address = value;
            }
        }

        int _port;
        public int Port
        {
            get { return _port; }
            set
            {
                if (!init)
                    _port = value;
            }
        }

        bool init = false;

        AddressFamily _family;
        public AddressFamily Family
        {
            get { return _family; }
            set
            {
                if (!init)
                    _family = value;
            }
        }

        ProtocolType _type;
        public ProtocolType Type
        {
            get { return _type; }
            set
            {
                if (!init)
                    _type = value;
            }
        }

        public Socket Socket { get; protected set; }
        public IPEndPoint LocalEndPoint { get; private set; }

        public NetHost()
            : this(DefaultAddress, DefaultPort)
        {

        }

        public NetHost(string ip, int port)
        {
            Address = ip;
            Port = port;
            Family = AddressFamily.InterNetwork;
            Type = ProtocolType.Udp;
        }

        public void Init(SocketType socketType = SocketType.Dgram)
        {
            try
            {
                Socket = new Socket(Family, socketType, Type);
                LocalEndPoint = new IPEndPoint(
                    (string.IsNullOrEmpty(Address)) ? IPAddress.Any : IPAddress.Parse(Address), 
                    Port
                    );
                init = true;
            }
            catch (Exception e)
            {
                throw new Exception("Error on NetHost.Init(), " + e.ToString());
            }
        }

        public bool Send(byte[] buffer)
        {
            if (!init) return false;
            return (Socket.SendTo(buffer, LocalEndPoint) > 0);
        }
    }
}
