using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoAudioPlayer
{
    class FFTAnalyzer
    {
        static public float[] AvgValues(float[] data, int transformTo, int multiplyBy = 100)
        {
            float[] averaged = new float[transformTo];
            int iCounter = 0;
            int sizeOfArray = (data.Length / 2); // halved because there is no second part due to channels being messed up :(
            int divisor = sizeOfArray / transformTo;
            for (int i = 0; i < sizeOfArray; i++)
            {
                averaged[iCounter] += data[i] * multiplyBy;
                if (i % divisor == divisor - 1)
                {
                    averaged[iCounter] += data[i] * multiplyBy;
                    averaged[iCounter] = averaged[iCounter] / divisor;
                    iCounter++;
                }
            }
            return averaged;
        }
    }
}
