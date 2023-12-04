using System;

namespace Helper
{
    public class Generator
    {
        private readonly Random random;

        public Generator()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public int Next()
        {
            return random.Next(int.MaxValue);
        }

        public int Next(int value)
        {
            return random.Next(value);
        }
    }
}