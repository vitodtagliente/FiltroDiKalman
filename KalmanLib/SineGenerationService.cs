using System;

namespace KalmanLib
{
    public class SineGenerationService : PacketGenerationService
    {
        // Dimensione massima in byte del pacchetto
        public int Amplitude { get; private set; }
        public int Phase { get; private set; }
        public double AngularFrequency { get; private set; }
        
        public SineGenerationService(int amplitude = 32)
            : this(amplitude, 0.2, 0)
            // : this(amplitude, Math,PI / 2, 0)
        {

        }

        public SineGenerationService(int amplitude, double angularFrequency, int phase)
        {
            Amplitude = amplitude;
            AngularFrequency = angularFrequency;
            Phase = phase;
        }

        public override int Size()
        {            
            var sine = Amplitude + (Amplitude / 10) * Math.Sin(AngularFrequency * DateTime.Now.TimeOfDay.TotalSeconds + Phase);

            return Convert.ToInt32(sine);
        }
    }
}
