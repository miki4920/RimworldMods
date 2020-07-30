using System;
using Verse;

[HarmonyPatch(typeof(TimeSlower))]
[HarmonyPatch("ForcedNormalSpeed", PropertyMethod.Getter)]
public static class TimeSlower_ForcedNormalSpeed_Patch {
	private static readonly FieldInfo forceNormalSpeedUntilField = AccessTools.Field(typeof(TimeSlower), "forceNormalSpeedUntil");

	[HarmonyPostfix]
	public static void NoPause(TimeSlower __instance, ref bool __result) {
		__result = true;
	}
}