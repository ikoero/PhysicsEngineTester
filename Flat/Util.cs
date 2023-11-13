using System;
using System.Runtime.Serialization;

namespace Flat
{
    public static class Util
    {
        public static int Clam(int value, int min, int max) 
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("The value of \"min\" is greater than the value of \"max\"");
            }
            if (value < min) 
            {
                return min;
            }
            else if (value > max) 
            {
                return max;
            }
            else 
            {
                return value;
            }
        }

        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("The value of \"min\" is greater than the value of \"max\"");
            }
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static void Normalize(ref float x , ref float y)
        {
            float invlen = 1f / MathF.Sqrt(x * x + y * y);
            x *= invlen;
            y *= invlen;
        }
    }
}   
