using System;
using System.Collections.Generic;

namespace KalmanLib
{
    public abstract class PacketGenerationService
    {
        public virtual int Size()
        {
            return 1024;
        }

        public byte[] Generate()
        {
            List<byte> buffer = new List<byte>();
            // Accodamento dell'orario di invio in millisecondi
            double millisecs = DateTime.Now.TimeOfDay.TotalMilliseconds;
            buffer.AddRange(BitConverter.GetBytes(millisecs));
            Console.WriteLine(String.Format("Send on {0}", millisecs));
            // Accodamento dei byte casuali
            Random random = new Random();
            int size = Size();
            Console.WriteLine(String.Format("Packet size: {0}", size));
            if (size < 0) size *= -1;
            byte[] b = new byte[size];
            random.NextBytes(b);
            buffer.AddRange(b);
            return buffer.ToArray();
        }
    }
}
