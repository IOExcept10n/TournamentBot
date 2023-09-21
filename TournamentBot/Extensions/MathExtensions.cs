using System.Runtime.CompilerServices;

namespace TournamentBot.Extensions
{
    internal static class MathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NextPowerOfTwo(this uint n)
        {
            n--;
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            n++;
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetHighBit(this uint n)
        {
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            return n - (n >> 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetBitCount(this uint x)
        {
            x = (x & 0x55555555) + ((x >> 1) & 0x55555555);
            x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
            x = (x & 0x0F0F0F0F) + ((x >> 4) & 0x0F0F0F0F);
            x = (x & 0x00FF00FF) + ((x >> 8) & 0x00FF00FF);
            return (x & 0x0000FFFF) + (x >> 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetLog2(this uint x)
        {
            return GetBitCount(GetHighBit(x));
        }
    }
}