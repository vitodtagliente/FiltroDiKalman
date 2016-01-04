using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanLib
{
    public class SineGenerationService : PacketGenerationService
    {
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
            buffer.AddRange(Encoding.ASCII.GetBytes(DateTime.Now.TimeOfDay.ToString()));
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
