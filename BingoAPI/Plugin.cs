using BepInEx;
using BingoAPI.Conditions;
using BingoAPI.Conditions.BuiltIn;
using BingoAPI.Helpers;

namespace BingoAPI;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
	private void Start()
	{
		Log.SetLogger(Logger);

		ConditionRegistry.Register("AND",
			data =>
			{
				var children = data.GetChildren();

				return new AndCondition(children);
			});

		ConditionRegistry.Register("OR",
			data =>
			{
				var children = data.GetChildren();

				return new OrCondition(children);
			});

		ConditionRegistry.Register("NOT",
			data =>
			{
				var child = data.GetChild();

				return new NotCondition(child);
			});

		ConditionRegistry.Register("SOME",
			data =>
			{
				var children = data.GetChildren();
				var amount = data.GetOptionalParam<uint>("amount", 1);

				return new SomeCondition(amount, children);
			});
	}
}
