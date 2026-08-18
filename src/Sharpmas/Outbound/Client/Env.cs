namespace Sharpmas.Outbound.Client;

public static class Env
{
    const string CONTACT_KEY = "CONTACT";

    public static string UserAgent()
    {
        return Environment.GetEnvironmentVariable(CONTACT_KEY)
            ?? throw new InvalidOperationException($"{CONTACT_KEY} not set");
    }
}
