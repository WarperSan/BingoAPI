using BepInEx.Configuration;

namespace BingoAPI.Configurations;

/// <summary>
/// Class that holds the configurations related to the networking
/// </summary>
public class NetworkConfig
{
    private const string SECTION = "Network";

    public readonly ConfigEntry<int> NetworkDelayMS;
    public readonly ConfigEntry<int> NetworkTimeoutMS;
    
    public NetworkConfig(ConfigFile cfg)
    {
        NetworkDelayMS = cfg.Bind(
            SECTION,
            "networkDelayMillisecond",
            25,
            "The number of milliseconds to wait before checking a request's status."
        );

        NetworkTimeoutMS = cfg.Bind(
            SECTION,
            "networkTimeoutMillisecond",
            30_000,
            "The number of milliseconds to wait before considering a request as timed out."
        );
    }
}