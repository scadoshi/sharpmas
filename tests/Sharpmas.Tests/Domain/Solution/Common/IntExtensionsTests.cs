using Sharpmas.Domain.Solution.Common;

namespace Sharpmas.Tests.Domain.Solution.Common;

public class IntExtensionsTests
{
    // Each theory takes (lhs, rhs, expected), null meaning overflow.

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(int.MaxValue, 1, null)]
    [InlineData(int.MinValue, -1, null)]
    public void CheckedAdd(int lhs, int rhs, int? expected)
    {
        Assert.Equal(expected, lhs.CheckedAdd(rhs));
    }

    /// <summary>The last row is why this is not CheckedAdd(-rhs): negating int.MinValue overflows.</summary>
    [Theory]
    [InlineData(3, 2, 1)]
    [InlineData(int.MinValue, 1, null)]
    [InlineData(int.MaxValue, -1, null)]
    [InlineData(0, int.MinValue, null)]
    public void CheckedSub(int lhs, int rhs, int? expected)
    {
        Assert.Equal(expected, lhs.CheckedSub(rhs));
    }

    [Theory]
    [InlineData(6, 7, 42)]
    [InlineData(int.MaxValue, int.MaxValue, null)]
    [InlineData(int.MinValue, -1, null)]
    public void CheckedMul(int lhs, int rhs, int? expected)
    {
        Assert.Equal(expected, lhs.CheckedMul(rhs));
    }

    [Theory]
    [InlineData(42, 7, 6)]
    [InlineData(1, 0, null)]
    [InlineData(int.MinValue, -1, null)]
    public void CheckedDiv(int lhs, int rhs, int? expected)
    {
        Assert.Equal(expected, lhs.CheckedDiv(rhs));
    }
}

public class UIntExtensionsTests
{
    [Theory]
    [InlineData(1u, 2u, 3u)]
    [InlineData(uint.MaxValue, 1, null)]
    public void CheckedAdd(uint lhs, uint rhs, uint? expected)
    {
        Assert.Equal(expected, lhs.CheckedAdd(rhs));
    }

    [Theory]
    [InlineData(3u, 2u, 1u)]
    [InlineData(uint.MinValue, 1, null)]
    [InlineData(3_000_000_000u, 1u, 2_999_999_999u)]
    /// <summary>The big row guards the bounds: a valid difference can exceed int.MaxValue.</summary>
    public void CheckedSub(uint lhs, uint rhs, uint? expected)
    {
        Assert.Equal(expected, lhs.CheckedSub(rhs));
    }

    [Theory]
    [InlineData(6u, 7u, 42u)]
    [InlineData(uint.MaxValue, uint.MaxValue, null)]
    public void CheckedMul(uint lhs, uint rhs, uint? expected)
    {
        Assert.Equal(expected, lhs.CheckedMul(rhs));
    }

    [Theory]
    [InlineData(42u, 7u, 6u)]
    [InlineData(1, 0, null)]
    public void CheckedDiv(uint lhs, uint rhs, uint? expected)
    {
        Assert.Equal(expected, lhs.CheckedDiv(rhs));
    }
}
