using BepInEx;
using BingoAPI.Configurations;
using BingoAPI.Helpers;

namespace BingoAPI;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        AssemblyLoader.LoadEmbeddedDLL();
        
        Helpers.Logger.SetLogger(Logger);
        Configuration.Load(Config);
        
        Helpers.Logger.Info($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }
}