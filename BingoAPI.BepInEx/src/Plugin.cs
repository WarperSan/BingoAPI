using BepInEx;
using BingoAPI.Conditions;

namespace BingoAPI.BepInEx;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
	private void Awake()
	{
		ConditionRegistry.AddBuiltIn();
		Helpers.Log.Info($"{Name} v{Version} has loaded!");
	}
}
