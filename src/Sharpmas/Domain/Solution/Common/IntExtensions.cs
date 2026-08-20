namespace Sharpmas.Domain.Solution.Common;

/// <summary>Arithmetic that reports overflow as null instead of wrapping.</summary>
/// <remarks>
/// C# wraps silently in debug and release both, so a walk off the end of a
/// number line becomes a plausible wrong answer with nothing to notice it.
/// These match Rust's checked family: widen, compute, null if the result will
/// not fit back.
/// </remarks>
public static class IntExtensions
{
    extension(int value)
    {
        /// <summary>The sum, or null past either end of int.</summary>
        public int? CheckedAdd(int rhs)
        {
            var sum = (long)value + rhs;
            return (sum >= int.MinValue && sum <= int.MaxValue) ? (int)sum : null;
        }

        /// <summary>The difference, or null past either end of int.</summary>
        public int? CheckedSub(int rhs)
        {
            var diff = (long)value - rhs;
            return (diff >= int.MinValue && diff <= int.MaxValue) ? (int)diff : null;
        }

        /// <summary>The product, or null past either end of int.</summary>
        public int? CheckedMul(int rhs)
        {
            var product = (long)value * rhs;
            return (product >= int.MinValue && product <= int.MaxValue) ? (int)product : null;
        }

        /// <summary>The quotient, or null for zero and for int.MinValue / -1.</summary>
        /// <remarks>
        /// The second case is an overflow: negating int.MinValue lands one past
        /// int.MaxValue, and the raw expression is a hardware trap that throws
        /// even in unchecked code.
        /// </remarks>
        public int? CheckedDiv(int rhs) =>
            rhs == 0 || (value == int.MinValue && rhs == -1) ? null : value / rhs;
    }

    extension(uint value)
    {
        /// <summary>The sum, or null past uint.MaxValue.</summary>
        public uint? CheckedAdd(uint rhs)
        {
            var sum = (ulong)value + rhs;
            return sum <= uint.MaxValue ? (uint)sum : null;
        }

        /// <summary>The difference, or null below zero.</summary>
        public uint? CheckedSub(uint rhs)
        {
            var diff = (long)value - rhs;
            return diff >= 0 ? (uint)diff : null;
        }

        /// <summary>The product, or null past uint.MaxValue.</summary>
        /// <remarks>Widens to ulong: two uints can multiply past long.</remarks>
        public uint? CheckedMul(uint rhs)
        {
            var product = (ulong)value * rhs;
            return product <= uint.MaxValue ? (uint)product : null;
        }

        /// <summary>The quotient, or null for zero. No negatives, so no other case.</summary>
        public uint? CheckedDiv(uint rhs) => rhs == 0 ? null : value / rhs;
    }
}
