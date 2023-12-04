using System;
using System.Security.Cryptography;

namespace Protections
{
    public sealed class xCryptoRandom : Random, IDisposable
    {
        private RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
        private byte[] uint32Buffer = new byte[sizeof(uint)];
        public xCryptoRandom()
        {
        }
        public xCryptoRandom(int seedIgnored)
        {
        }
        public override int Next()
        {
            cryptoProvider.GetBytes(uint32Buffer);
            return BitConverter.ToInt32(uint32Buffer, 0) & 0x7FFFFFFF;
        }
        public override int Next(int maxValue)
        {
            if (maxValue < 0) throw new ArgumentOutOfRangeException("maxValue");
            return Next(0, maxValue);
        }
        public override int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue) throw new ArgumentOutOfRangeException("minValue");
            if (minValue == maxValue) return minValue;

            long diff = maxValue - minValue;
            long max = (1 + (long) uint.MaxValue);
            long remainder = max % diff;

            while (true)
            {
                cryptoProvider.GetBytes(uint32Buffer);
                uint rand = BitConverter.ToUInt32(uint32Buffer, 0);
                if (rand < max - remainder)
                {
                    return (int) (minValue + (rand % diff));
                }
            }
        }
        public override double NextDouble()
        {
            cryptoProvider.GetBytes(uint32Buffer);
            uint rand = BitConverter.ToUInt32(uint32Buffer, 0);
            return rand / (1.0 + uint.MaxValue);
        }
        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            cryptoProvider.GetBytes(buffer);
        }
        public void Dispose()
        {
            InternalDispose();
        }
        ~xCryptoRandom()
        {
            InternalDispose();
        }
        void InternalDispose()
        {
            if (cryptoProvider != null)
            {
                cryptoProvider.Dispose();
                cryptoProvider = null;
            }
        }
    }
}