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
        public int? CheckedAdd(int rhs) =>
            (long)value + rhs is >= int.MinValue and <= int.MaxValue and var sum ? (int)sum : null;

        /// <summary>The difference, or null past either end of int.</summary>
        public int? CheckedSub(int rhs) =>
            (long)value - rhs is >= int.MinValue and <= int.MaxValue and var difference
                ? (int)difference
                : null;

        /// <summary>The product, or null past either end of int.</summary>
        public int? CheckedMul(int rhs) =>
            (long)value * rhs is >= int.MinValue and <= int.MaxValue and var product
                ? (int)product
                : null;

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
        public uint? CheckedAdd(uint rhs) =>
            (ulong)value + rhs is >= uint.MinValue and <= uint.MaxValue and var sum
                ? (uint)sum
                : null;

        /// <summary>The difference, or null below zero.</summary>
        public uint? CheckedSub(uint rhs) =>
            (long)value - rhs is >= uint.MinValue and <= uint.MaxValue and var difference
                ? (uint)difference
                : null;

        /// <summary>The product, or null past uint.MaxValue.</summary>
        /// <remarks>Widens to ulong: two uints can multiply past long.</remarks>
        public uint? CheckedMul(uint rhs) =>
            (ulong)value * rhs is >= uint.MinValue and <= uint.MaxValue and var product
                ? (uint)product
                : null;

        /// <summary>The quotient, or null for zero. No negatives, so no other case.</summary>
        public uint? CheckedDiv(uint rhs) => rhs == 0 ? null : value / rhs;
    }
}
