
using System;
using System.Collections.Generic;

namespace ReceiptStorage.Extensions
{
    public class MathHelper
    {
        private static double a;
        private static double b;
        
        /// <summary>
        /// Wyliczanie zmiennych a i b dla wzoru y=ax + b
        /// </summary>
        /// <param name="values"></param>
        public static void TrendLineOperand(double[] values)
        {

            double xAvg = 0;
            double yAvg = 0;
            
            for (int x = 0; x < values.Length; x++)
            {
                xAvg += x;
                yAvg += values[x];
            }

            xAvg = xAvg / values.Length;
            yAvg = yAvg / values.Length;
            
            double v1 = 0;
            double v2 = 0;
            
            for (int x = 0; x < values.Length; x++)
            {
                v1 += (x - xAvg) * (values[x] - yAvg);
                v2 += Math.Pow(x - xAvg, 2);
            }
            
            a = v1 / v2;
            b = yAvg - a * xAvg;

        }

        /// <summary>
        /// Pobieranie wartości trendu
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double GetTrendLine(int x)
        {
            var y = Math.Round(a, 2)*x + Math.Round(b, 2);
            return Math.Round(y,2);
        }
    }
}
