using BepInEx;
using BingoAPI.Configurations;

namespace BingoAPI;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        Helpers.Log.SetLogger(Logger);
        Configuration.Load(Config);
        
        Helpers.Log.Info($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }
}