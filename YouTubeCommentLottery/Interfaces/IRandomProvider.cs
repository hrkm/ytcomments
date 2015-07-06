using System.Collections.Generic;

namespace YouTubeCommentLottery.Interfaces
{
    public interface IRandomProvider
    {
        /// <summary>
        ///     Get next random number from the provider.
        /// </summary>
        /// <returns>Next random number</returns>
        int Next();

        /// <summary>
        ///     Get next random number from 0 to max.
        /// </summary>
        /// <param name="max">Maximum exclusive value</param>
        /// <returns>Random number from range [0, max)</returns>
        int Next(int max);

        /// <summary>
        ///     Get next random number from range [min, max).
        /// </summary>
        /// <param name="min">Minimum inclusive value</param>
        /// <param name="max">Maximum exclusive value</param>
        /// <returns>Random number from range [min, max)</returns>
        int Next(int min, int max);

        /// <summary>
        /// Get a sequence of "count" unique numbers from range [min, max).
        /// </summary>
        /// <param name="min">Minimum inclusive value</param>
        /// <param name="max">Maximum exclusive value</param>
        /// <param name="count">Number of items in the sequence</param>
        /// <returns>Sequence of "count" unique numbers from the range [min, max)</returns>
        List<int> NextSequence(int min, int max, int count);
    }
}