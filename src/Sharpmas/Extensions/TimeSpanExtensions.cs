namespace Sharpmas.Extensions;

public static class TimeSpanExtensions
{
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
