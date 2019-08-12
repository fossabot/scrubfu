/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Security.Cryptography;

namespace Scrubfu.Services
{
    public class SafeRandomizer
    {
        private readonly RandomNumberGenerator randomizer;
        private readonly byte[] uint32Buffer = new byte[4];

        public SafeRandomizer()
        {
            randomizer = RandomNumberGenerator.Create();
        }

        public Int32 Next(Int32 minValue, Int32 maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));

            if (minValue == maxValue) return minValue;

            Int64 diff = maxValue - minValue;
            while (true)
            {
                randomizer.GetBytes(uint32Buffer);
                var rand = BitConverter.ToUInt32(uint32Buffer, 0);

                var max = (1 + (Int64)UInt32.MaxValue);
                var remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (Int32)(minValue + (rand % diff));
                }
            }
        }
    }
}
