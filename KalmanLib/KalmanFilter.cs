using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinearAlgebra;
using LinearAlgebra.Matricies;

namespace KalmanLib
{
    public class KalmanFilter
    {
        // Cofigurazioni iniziali del filtro definite dalla traccia
        public DoubleMatrix P = new DoubleMatrix(new double[,] { { 1000, 0 }, { 0, Math.Pow(10, -1) } });
        public DoubleMatrix Q = new DoubleMatrix(new double[,] { { Math.Pow(10, -10), 0 }, { 0, Math.Pow(10, -3) } });

        // Rumore di misura
        public double sigma = 50;

        // One way delay variation stimato
        public double m { get; private set; }

        // One way delay variation misurato
        public double dm { get; private set; }

        // Capacità del canale in kbps
        public double C = 512;

        public double InverseC { get; private set; }

        // Parametro Beta
        public double beta = 0.95;

        // Packet size variation
        public int DeltaL { get; private set; }


        // last packet size
        int lastPacketSize = 0;
        DateTime lastPacketReceivedTime;
        DateTime lastPacketSendTime;

        bool firstStep = true;

        public KalmanFilter()
        {
            DeltaL = 0;
            dm = 0;
            m = 0;
            InverseC = 0;
            lastPacketSize = 0;

            InverseC = 1 / C;
        }

        public void NextStep(byte[] bytes)
        {
            string packet = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

            if (firstStep)
            {
                // first step
                lastPacketReceivedTime = DateTime.Now;
                lastPacketSendTime = DateTime.Parse(packet.Substring(0, DateTime.Now.TimeOfDay.ToString().Length));
                lastPacketSize = bytes.Length;

                firstStep = false;
                return;
            }

            // Calcolo la variazione DeltaL(i+1) = L(i+1) - L(i)
            DeltaL = bytes.Length - lastPacketSize;
            Log("DeltaL(i+1) = L(i+1) - L(i) = " + bytes.Length + " - " + lastPacketSize + " = ", ConsoleColor.Magenta);
            LogLine(DeltaL.ToString(), ConsoleColor.White);

            // Calcolo della variazione One Way Delay Variation
            // dm(i+1) = (T(i+1)-T(i)) - (t(i+1) - t(i))
            // Delta tempo di arrivo - Delta tempo di invio

            // determina t(i+1)
            var tiplus1 = DateTime.Parse(packet.Substring(0, DateTime.Now.TimeOfDay.ToString().Length));
            
            double Deltatplus1 = tiplus1.Subtract(lastPacketSendTime).TotalMilliseconds;
            Log("(t(i+1) - t(i)) = (" + tiplus1.TimeOfDay.ToString() + " - " + lastPacketSendTime.TimeOfDay.ToString() + ") = ", ConsoleColor.Magenta);
            LogLine(Deltatplus1.ToString(), ConsoleColor.White);

            double DeltaTplus1 = DateTime.Now.Subtract(lastPacketReceivedTime).TotalMilliseconds;
            Log("(T(i+1) - T(i)) = (" + DateTime.Now.TimeOfDay.ToString() + " - " + lastPacketReceivedTime.TimeOfDay.ToString() + ") = ", ConsoleColor.Magenta);
            LogLine(DeltaTplus1.ToString(), ConsoleColor.White);

            dm = DeltaTplus1 - Deltatplus1;
            Log("dm = DeltaT(i+1) - Deltat(i+1) = ", ConsoleColor.Magenta);
            LogLine(dm.ToString(), ConsoleColor.White);

            // Aggiornamento di P = P + Q
            P = P + Q;
            // H = [Delta L 1]
            DoubleMatrix H = new DoubleMatrix(new double[,] { { DeltaL, 1 } });
            // PH = P * H' (prodotto tra matrici)
            var PH = P * H.Transposed;
            // residuo = dm - 1/C*H(0) - m
            double residuo = dm - InverseC * H[0, 0] - m;
            // sigma = β * sigma + (1 − β)*residuo ^ 2(β = 0, 95)
            sigma = beta * sigma + (1 - beta) * (Math.Pow(residuo, 2));
            // denominatore = sigma + H(0)*PH(0)+ H(1)*PH(1)
            var denominatore = sigma + H[0, 0] * PH[0, 0] + H[1, 0] * PH[0, 1];
            // K = [ PH(0)/ denominatore; PH(1)/ denominatore] (kalman gain) (vettore 2x1)
            var K = (PH / denominatore);
            // IKH = [ 1.0 - K(0)*H(0), -K(0)*H(1); -K(1)*H(0), 1.0 - K(1)*H(1) ] (matrice 2x2)
            var IKH = new DoubleMatrix(new double[,] {
                { 1.0 - K[0, 0] * H[0, 0], -K[0, 0] * H[1, 0] },
                { -K[0, 1] * H[0, 0], 1.0 - K[0, 1] * H[1, 0] }
            });
            // P = IKH * P (prodotto tra matrici)
            P = IKH * P;
            // m = m + K(1) * residuo (nuova stima di m da loggare)
            m = m + K[0, 1] * residuo;
            // 1/C = 1/C + K(0) * residuo (nuova stima di C da loggare)
            InverseC = InverseC + K[0, 0] * residuo;
        }

        public void LogResults(ConsoleColor color)
        {
            Console.WriteLine(string.Empty);
            Log("   Estimated m = ", ConsoleColor.Yellow);
            LogLine(m.ToString(), ConsoleColor.White);
            Log("   Estimated 1/C = ", ConsoleColor.Yellow);
            LogLine(InverseC.ToString(), ConsoleColor.White);
            Console.WriteLine(string.Empty);
            Log("   Enstimated C = ", ConsoleColor.Cyan);
            LogLine(Math.Pow(InverseC, -1).ToString(), ConsoleColor.White);
        }
        
        void Log(string text, ConsoleColor color)
        {
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = startColor;
        }

        void LogLine(string text, ConsoleColor color)
        {
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = startColor;
        }
    }
}
