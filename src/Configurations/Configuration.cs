using BepInEx.Configuration;

namespace BingoAPI.Configurations;

/// <summary>
/// Class that holds every other configuration
/// </summary>
internal class Configuration
{
    public readonly NetworkConfig Network;

    private Configuration(ConfigFile cfg)
    {
        Network = new NetworkConfig(cfg);
    }

    /// <summary>
    /// Configuration loaded
    /// </summary>
    public static Configuration? Instance { get; private set; }

    /// <summary>
    /// Loads the configuration from the given configuration file
    /// </summary>
    public static void Load(ConfigFile cfg)
    {
        Instance = new Configuration(cfg);
    }
}