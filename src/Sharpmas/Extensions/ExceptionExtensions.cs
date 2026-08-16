namespace Sharpmas.Extensions;

/// <summary>Extensions on <see cref="Exception"/>.</summary>
public static class ExceptionExtensions
{
    /// <summary>The exception and every inner one, outermost first.</summary>
    /// <remarks>
    /// Joining these is what makes a failure read like Rust's <c>{e:#}</c>. The
    /// outermost message on its own rarely says which day or file failed.
    /// </remarks>
    public static IEnumerable<Exception> Causes(this Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            yield return current;
        }
    }
}
