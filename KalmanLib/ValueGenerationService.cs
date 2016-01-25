using System;
using System.Collections.Generic;

namespace KalmanLib
{
    public class ValueGenerationService : PacketGenerationService
    {
        public List<int> Sizes = new List<int>();
        int _counter = 0;
        public int Counter
        {
            get
            {
                int value = _counter;
                _counter++;
                if (_counter >= Sizes.Count)
                    _counter = 0;
                return value;
            }
            private set { _counter = value; }
        }

        public ValueGenerationService()
        {

        }

        public override int Size()
        {
            return Sizes[Counter];
        }
    }
}
