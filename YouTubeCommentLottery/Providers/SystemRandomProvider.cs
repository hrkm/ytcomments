using System;
using System.Collections.Generic;
using YouTubeCommentLottery.Interfaces;

namespace YouTubeCommentLottery.Providers
{
    public class SystemRandomProvider : IRandomProvider
    {
        private readonly Random random;

        public SystemRandomProvider()
        {
            random = new Random();
        }

        public int Next()
        {
            return random.Next();
        }

        public int Next(int max)
        {
            return random.Next(max);
        }

        public int Next(int min, int max)
        {
            return random.Next(min, max);
        }

        public List<int> NextSequence(int min, int max, int count)
        {
            var list = new List<int>();
            if (count > max - min)
                throw new ArgumentOutOfRangeException("count", "count cannot be bigger than (max - min)");
            while (list.Count < count)
            {
                var n = Next(min, max);
                if (!list.Contains(n))
                    list.Add(n);
            }
            return list;
        }
    }
}