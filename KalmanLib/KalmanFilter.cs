using System;
using System.IO;
using System.Text;

namespace KalmanLib
{
    public class KalmanFilter
    {
        public static string Filename = "server.log.txt";

        // Cofigurazioni iniziali del filtro definite dalla traccia
        //public DoubleMatrix P = new DoubleMatrix(new double[,] { { 1000, 0 }, { 0, Math.Pow(10, -1) } });
        //public DoubleMatrix Q = new DoubleMatrix(new double[,] { { Math.Pow(10, -10), 0 }, { 0, Math.Pow(10, -3) } });
        public double[,] P = new double[2, 2];
        public double[,] Q = new double[2, 2];

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
        double lastPacketReceivedTime;
        double lastPacketSendTime;

        bool firstStep = true;

        public KalmanFilter()
        {
            DeltaL = 0;
            dm = 0;
            m = 0;
            lastPacketSize = 0;

            InverseC = Math.Pow(C, -1);

            // Inizializza le matrici
            P[0, 0] = 1000;
            P[0, 1] = 0;
            P[1, 0] = 0;
            P[1, 1] = Math.Pow(10, -1);

            Q[0, 0] = Math.Pow(10, -10);
            Q[0, 1] = 0;
            Q[1, 0] = 0;
            Q[1, 1] = Math.Pow(10, -3);

            // Crea il file di log

            StreamWriter wr = new StreamWriter(Filename);
            wr.Close();
            wr.Dispose();
        }

        public void NextStep(byte[] bytes)
        {
            string packet = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

            double timeInPacket = 0;
            /*
            try
            {
                timeInPacket = double.Parse(packet.Substring(0, DateTime.Now.TimeOfDay.TotalMilliseconds.ToString().Length));
            }
            catch(Exception)
            {
                Console.WriteLine(String.Format("Encoding error {0}", packet.Substring(0, DateTime.Now.TimeOfDay.TotalMilliseconds.ToString().Length)));
                return;
            }
            */
            timeInPacket = BitConverter.ToDouble(bytes, 0);
            Console.WriteLine(String.Format("Send timespan: {0}", timeInPacket));

            if (firstStep)
            {
                // first step
                //lastPacketReceivedTime = DateTime.Now;
                //lastPacketSendTime = DateTime.Parse(packet.Substring(0, DateTime.Now.TimeOfDay.ToString().Length));
                lastPacketReceivedTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
                //lastPacketSendTime = double.Parse(packet.Substring(0, DateTime.Now.TimeOfDay.TotalMilliseconds.ToString().Length));
                lastPacketSendTime = timeInPacket;

                lastPacketSize = bytes.Length;
                
                // CSV: timestamp(millisecondi), DeltaL, dm, K[0,0], K[0,1], P[0,0] , P[0,1], P[1,0], P[1,1], sigma, m, C

                firstStep = false;
                return;
            }
            
            // Calcolo la variazione DeltaL(i+1) = L(i+1) - L(i)
            DeltaL = bytes.Length - lastPacketSize;

            // Calcolo della variazione One Way Delay Variation
            // dm(i+1) = (T(i+1)-T(i)) - (t(i+1) - t(i))
            // Delta tempo di arrivo - Delta tempo di invio

            // determina t(i+1)
            //var tiplus1 = DateTime.Parse(packet.Substring(0, DateTime.Now.TimeOfDay.ToString().Length));
            double tiplus1 = timeInPacket;

            //double Deltatplus1 = tiplus1.Subtract(lastPacketSendTime).TotalMilliseconds;
            double Deltatplus1 = tiplus1 - lastPacketSendTime;

            //double DeltaTplus1 = DateTime.Now.Subtract(lastPacketReceivedTime).TotalMilliseconds;
            double DeltaTplus1 = DateTime.Now.TimeOfDay.TotalMilliseconds - lastPacketReceivedTime;

            dm = DeltaTplus1 - Deltatplus1;
            Console.WriteLine(String.Format("dm = {0}", dm));

            // Aggiornamento di P = P + Q
            P = AddMatrix(P, Q);
            // H = [Delta L 1]
            double[,] H = new double[1, 2];
            H[0, 0] = DeltaL;
            H[0, 1] = 1;
            // PH = P * H' (prodotto tra matrici)
            double[,] Ht = new double[2, 1];
            Ht[0, 0] = H[0, 0];
            Ht[1, 0] = H[0, 1];

            var PH = MulMatrix(P, Ht);
            // residuo = dm - 1/C*H(0) - m
            double residuo = dm - InverseC * H[0, 0] - m;
            // sigma = β * sigma + (1 − β)*residuo ^ 2(β = 0, 95)
            sigma = beta * sigma + (1 - beta) * (Math.Pow(residuo, 2));
            // denominatore = sigma + H(0)*PH(0)+ H(1)*PH(1)
            var denominatore = sigma + H[0, 0] * PH[0, 0] + H[0, 1] * PH[1, 0];
            // K = [ PH(0)/ denominatore; PH(1)/ denominatore] (kalman gain) (vettore 2x1)
            double[,] K = new double[2, 1];
            K[0, 0] = PH[0, 0] / denominatore;
            K[1, 0] = PH[1, 0] / denominatore;
            // IKH = [ 1.0 - K(0)*H(0), -K(0)*H(1); -K(1)*H(0), 1.0 - K(1)*H(1) ] (matrice 2x2)
            double[,] IKH = new double[2, 2];
            IKH[0, 0] = 1.0 - (K[0, 0] * H[0, 0]);
            IKH[0, 1] = -K[0, 0] * H[0, 1];
            IKH[1, 0] = -K[1, 0] * H[0, 0];
            IKH[1, 1] = 1.0 - (K[1, 0] * H[0, 1]);
            // P = IKH * P (prodotto tra matrici)
            P = MulMatrix(IKH, P);
            // m = m + K(1) * residuo (nuova stima di m da loggare)
            m = m + (K[1, 0] * residuo);
            // 1/C = 1/C + K(0) * residuo (nuova stima di C da loggare)
            InverseC = InverseC + (K[0, 0] * residuo);
            C = Math.Pow(InverseC, -1);

            // aggiornamento dei dati del filtro
            //lastPacketReceivedTime = DateTime.Now;
            lastPacketReceivedTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
            lastPacketSendTime = tiplus1;
            lastPacketSize = bytes.Length;

            // Log su file
            StringBuilder line = new StringBuilder();

            line.Append(DateTime.Now.TimeOfDay.ToString());
            line.Append(" , ");
            line.Append(DeltaL);
            line.Append(" , ");
            line.Append(dm);
            line.Append(" , ");
            line.Append(K[0, 0]);
            line.Append(" , ");
            line.Append(K[1, 0]);
            line.Append(" , ");
            line.Append(P[0, 0]);
            line.Append(" , ");
            line.Append(P[0, 1]);
            line.Append(" , ");
            line.Append(P[1, 0]);
            line.Append(" , ");
            line.Append(P[1, 1]);
            line.Append(" , ");
            line.Append(sigma);
            line.Append(" , ");
            line.Append(m);
            line.Append(" , ");
            line.Append(C);

            LogLine(line.ToString());
        }

        double[,] MulMatrix(double[,] A, double[,] B)
        {
            double[,] result = new double[A.GetLength(0), B.GetLength(1)];
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < B.GetLength(1); j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < A.GetLength(1); k++)
                    {
                        result[i, j] = result[i, j] + A[i, k] * B[k, j];
                    }
                }
            }
            return result;
        }

        double[,] AddMatrix(double[,] A, double[,] B)
        {
            double[,] result = new double[A.GetLength(0), A.GetLength(1)];
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    result[i, j] = A[i, j] + B[i, j];
                }
            }
            return result;
        }
        
        void Log(string text)
        {
            StreamWriter wr = new StreamWriter(Filename, true);
            wr.Write(text);
            wr.Close();
            wr.Dispose();
        }
        void LogLine(string text)
        {
            StreamWriter wr = new StreamWriter(Filename, true);
            wr.WriteLine(text);
            wr.Close();
            wr.Dispose();
        }

    }
}
