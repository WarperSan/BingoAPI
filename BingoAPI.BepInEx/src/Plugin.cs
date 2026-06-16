using BepInEx;
using BingoAPI.Conditions;

namespace BingoAPI.BepInEx;

[BepInAutoPlugin("dev.warpersan.bingoapi.bepinex")]
public partial class Plugin : BaseUnityPlugin
{
	private void Awake()
	{
		BingoAPI.Helpers.Log.SetLogger(Helpers.Log.LogCore);

		Helpers.Log.Info($"{Name} v{Version} has loaded!");
	}

	private void Start()
	{
		ConditionRegistry.AddAll();
	}
}
