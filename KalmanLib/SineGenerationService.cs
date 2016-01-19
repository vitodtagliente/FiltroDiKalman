using System;
using System.Collections.Generic;

namespace KalmanLib
{
    public class SineGenerationService : PacketGenerationService
    {
        // Dimensione massima in byte del pacchetto
        public int Amplitude { get; private set; }
        public int Phase { get; private set; }
        public double AngularFrequency { get; private set; }
        
        public SineGenerationService(int amplitude = 32)
            : this(amplitude, Math.PI / 2, 0)
        {

        }

        public SineGenerationService(int amplitude, double angularFrequency, int phase)
        {
            Amplitude = amplitude;
            AngularFrequency = angularFrequency;
            Phase = phase;
        }

        int Sine()
        {
            var sine = Amplitude + (Amplitude / 10) * Math.Sin(AngularFrequency * DateTime.Now.TimeOfDay.TotalSeconds + Phase);

            return Convert.ToInt32(sine);
        }

        public override byte[] Generate()
        {
            List<byte> buffer = new List<byte>();
            // Accodamento dell'orario di invio in millisecondi
            double millisecs = DateTime.Now.TimeOfDay.TotalMilliseconds;
            buffer.AddRange(BitConverter.GetBytes(millisecs));
            Console.WriteLine(String.Format("Send on {0}", millisecs));
            // Accodamento dei byte casuali
            Random random = new Random();
            int size = Sine();
            Console.WriteLine(String.Format("Packet size: {0}", size));
            if (size < 0) size *= -1;
            byte[] b = new byte[size];
            random.NextBytes(b);
            buffer.AddRange(b);
            return buffer.ToArray();
        }
    }
}
