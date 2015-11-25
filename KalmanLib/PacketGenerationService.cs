namespace KalmanLib
{
    public abstract class PacketGenerationService
    {
        public virtual byte[] Generate()
        {
            byte[] packet = new byte[1020];
            return packet;
        }
    }
}
