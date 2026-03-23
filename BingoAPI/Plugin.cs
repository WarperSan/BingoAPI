using BepInEx;
using BingoAPI.Helpers;

namespace BingoAPI;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
	private void Awake()
	{
		Log.Info($"{Id} v{Version} has loaded!");
	}

	private void Start()
	{
		Conditions.ConditionRegistry.Refresh();
	}

	private void OnDestroy()
	{
		Log.Info($"{Id} v{Version} has unloaded!");
	}
}
