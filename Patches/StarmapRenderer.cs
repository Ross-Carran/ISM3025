using BattleTech;
using Harmony;
using ISM3025.Features;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace ISM3025.Patches
{
    [HarmonyPatch(typeof(StarmapRenderer), "RefreshSystems")]
    public static class StarmapRenderer_RefreshSystems_Patch
    {
        public static void Postfix(StarmapRenderer __instance)
        {
            DynamicLogos.PlaceLogos(__instance);
            SquareMap.OnMapRefresh(__instance);
        }
    }

    [HarmonyPatch(typeof(StarmapRenderer), "FactionColor")]
    public static class StarmapRenderer_FactionColor_Patch
    {
        public static bool Prefix(ref Color __result, Faction thisFaction)
        {
            var color = FactionColors.GetModdedFactionColor(thisFaction);
            if (color == null)
                return true;

            __result = color.Value;
            return false;
        }
    }

    [HarmonyPatch(typeof(StarmapRenderer), "NormalizeToMapSpace")]
    public static class StarmapRenderer_NormalizeToMapSpace_Patch
    {
        // ReSharper disable once RedundantAssignment
        public static bool Prefix(Vector2 normalizedPos, ref Vector3 __result)
        {
            var squareMapSpace = SquareMap.NormalizeToSquareMapSpace(normalizedPos);
            if (squareMapSpace == null)
                return true;

            __result = squareMapSpace.Value;
            return false;
        }
    }
}
