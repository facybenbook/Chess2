﻿using UnityEngine;
using Random = System.Random;

namespace App
{
    public static class Math
    {
        public static Random Rand = new Random();

        public static int Abs(int n)
        {
            return n < 0 ? -n : n;
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static int RandomRanged(int min, int max)
        {
            return Rand.Next(min, max);
        }

        public static int Clamp(int min, int max, int val)
        {
            return val < min ? min : (val > max ? max : val);
        }

        public static void SetX(this Transform v, float x)
        {
            var p = v.position;
            p.x = x;
            v.position = p;
        }

        public static void SetY(this Transform v, float y)
        {
            var p = v.position;
            p.y = y;
            v.position = p;
        }

        public static void SetZ(this Transform v, float z)
        {
            var p = v.position;
            p.z = z;
            v.position = p;
        }
    }
}
