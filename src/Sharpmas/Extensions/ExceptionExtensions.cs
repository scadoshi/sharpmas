namespace Sharpmas.Extensions;

public static class ExceptionExtensions
{
    public static IEnumerable<Exception> Causes(this Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            yield return current;
        }
    }
}
