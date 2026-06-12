using BepInEx;

namespace BingoAPI.BepInEx;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
	private void Awake()
	{
		Helpers.Patch.ApplyAll();
		Helpers.Log.Info($"{Name} v{Version} has loaded!");
	}
}
