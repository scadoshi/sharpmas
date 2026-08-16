namespace Sharpmas.Extensions;

/// <summary>Extensions on <see cref="TimeSpan"/>.</summary>
public static class TimeSpanExtensions
{
    /// <summary>The duration in the largest unit that leaves it above one.</summary>
    /// <remarks>
    /// What Rust's <c>Duration</c> prints by default and C# has no equivalent
    /// of: <c>ToString</c> gives <c>00:00:00.0000070</c> where a part's output
    /// line wants <c>7µs</c>. Resolution is 100ns, so the nanosecond branch
    /// only ever lands on multiples of 100.
    /// </remarks>
    public static string Formatted(this TimeSpan timeSpan)
    {
        var seconds = timeSpan.TotalSeconds;
        var milliseconds = timeSpan.TotalMilliseconds;
        var microseconds = timeSpan.TotalMicroseconds;

        if (seconds >= 1)
        {
            return $"{seconds:0.####}s";
        }
        else if (milliseconds >= 1)
        {
            return $"{milliseconds:0.####}ms";
        }
        else if (microseconds >= 1)
        {
            return $"{microseconds:0.####}µs";
        }
        else
        {
            return $"{timeSpan.TotalNanoseconds:0.####}ns";
        }
    }
}
