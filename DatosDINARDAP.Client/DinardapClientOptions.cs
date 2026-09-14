namespace DatosDINARDAP.Client;

public sealed class DinardapClientOptions
{
    public const string SectionName = "DinardapApi";

    public string BaseUrl { get; set; } = "https://localhost:5001";
    public int TimeoutSeconds { get; set; } = 30;
}
