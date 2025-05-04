using BepInEx;
using BepInEx.Configuration;
using BingoAPI.Helpers;

namespace BingoAPI;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static Plugin? Instance;
    
    internal ConfigEntry<int>? configNetworkDelayMS;
    internal ConfigEntry<int>? configNetworkTimeoutMS;
    
    private void Awake()
    {
        AssemblyLoader.LoadEmbeddedDLL();
        
        Helpers.Logger.SetLogger(Logger);

        configNetworkDelayMS = Config.Bind(
            "Network",
            "networkDelayMillisecond",
            25,
            "The number of milliseconds to wait before checking a request's status."
        );

        configNetworkTimeoutMS = Config.Bind(
            "Network",
            "networkTimeoutMillisecond",
            30_000,
            "The number of milliseconds to wait before considering a request as timed out."
        );

        Instance = this;
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }
}