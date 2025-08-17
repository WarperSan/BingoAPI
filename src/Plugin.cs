using BepInEx;
using BingoAPI.Configurations;

namespace BingoAPI;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);
        Configuration.Load(Config);
        
        Helpers.Logger.Info($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} has loaded!");
    }
}