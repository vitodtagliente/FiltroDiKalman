using System.Collections.Generic;

namespace KalmanLib
{
    public class TriangleGenerationService : PacketGenerationService
    {
        List<int> Sizes = new List<int>();
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

        public TriangleGenerationService(int max, int min = 500, int step = 500)
        {
            int x = min;
            while(x < max)
            {
                Sizes.Add(x);
                x += step;
            }
            Sizes.Add(max);
            int y = max - min;
            while(y >= 2 * min)
            {
                Sizes.Add(y);
                y -= step;
            }
        }

        public override int Size()
        {
            return Sizes[Counter];
        }
    }
}
