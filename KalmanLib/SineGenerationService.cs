using System;
using System.Collections.Generic;
using System.Text;

namespace KalmanLib
{
    public class SineGenerationService : PacketGenerationService
    {
        // Dimensione massima in byte del pacchetto
        public int Amplitude { get; private set; }
        public int Phase { get; private set; }
        public float AngularFrequency { get; private set; }

        int time;

        public SineGenerationService(int amplitude = 32)
            : this(amplitude, 1f, 0)
        {

        }

        public SineGenerationService(int amplitude, float angularFrequency, int phase)
        {
            Amplitude = amplitude;
            AngularFrequency = angularFrequency;
            Phase = phase;
            time = 0;
        }

        int Sine()
        {
            var now = DateTime.Now;
            time += now.Second; 
            var sine = Amplitude * Math.Sin(AngularFrequency * time + Phase);

            return Convert.ToInt32(sine);
        }

        public override byte[] Generate()
        {
            List<byte> buffer = new List<byte>();
            // Ora di invio nel formato HH:mm:ss.fff
            //buffer.AddRange(Encoding.ASCII.GetBytes(DateTime.Now.TimeOfDay.ToString()));
            buffer.AddRange(Encoding.ASCII.GetBytes(DateTime.Now.TimeOfDay.TotalMilliseconds.ToString()));
            // Accodamento dei byte casuali
            Random random = new Random();
            int size = Sine();
            if (size < 0) size *= -1;
            Byte[] b = new Byte[size];
            random.NextBytes(b);
            buffer.AddRange(b);
            return buffer.ToArray();
        }
    }
}
