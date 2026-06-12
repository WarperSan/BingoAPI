using HarmonyLib;

namespace BingoAPI.BepInEx.Patches;

[HarmonyPatch(typeof(string))]
internal static class String_Patches
{
	[HarmonyPrefix]
	[HarmonyPatch(nameof(string.Remove))]
	private static void Remove_Prefix(string __instance)
	{
	}

	[HarmonyPostfix]
	[HarmonyPatch(nameof(string.Remove))]
	private static void Remove_Postfix(string __instance)
	{
	}
}
