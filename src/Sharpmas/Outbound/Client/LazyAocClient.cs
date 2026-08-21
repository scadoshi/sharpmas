namespace Sharpmas.Outbound.Client;

/// <summary>An <see cref="AocClient"/> built on first use, so nothing offline pays for one.</summary>
/// <remarks>
/// Holding one does not mean a cookie exists: that is checked the first time
/// <see cref="Connected"/> runs, which for a fully cached run is never.
/// </remarks>
public sealed class LazyAocClient
{
    AocClient? client;

    /// <summary>The client, built now if it never was.</summary>
    /// <remarks>Idempotent, so calling it eagerly and again later costs one construction.</remarks>
    public AocClient Connected() => client ??= new AocClient();
}
