﻿using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using System;

namespace Aplem.Common
{
    using Random = Unity.Mathematics.Random;

    public class RandomUtil : Singleton<RandomUtil>
    {
        private Random _seedRand = new((uint)Environment.TickCount);

        public static ref Random GlobalRand => ref Inst._seedRand;

        public RandomUtil()
        {
            SingletonInitialize();
        }

        public override void SingletonInitialize()
        {
            _seedRand = new Random((uint)Environment.TickCount);
        }

        public Random CreateRandom()
        {
            return new Random(_seedRand.NextUInt());
        }
    }
}
