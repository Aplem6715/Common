using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using System;

namespace Aplem.Common
{
    using Random = Unity.Mathematics.Random;

    public class RandomUtil : Singleton<RandomUtil>
    {
        private Random _seedRand = new Random((uint)Environment.TickCount);

        public static ref Random GlobalRand => ref Inst._seedRand;

        public RandomUtil()
        {
            Reset();
        }

        public override void Reset()
        {
            _seedRand = new Random((uint)Environment.TickCount);
        }

        public Random CreateRandom()
        {
            return new Random(_seedRand.NextUInt());
        }
    }
}