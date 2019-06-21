using BattleTech;
using Harmony;
using ISM3025.Features;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace ISM3025.Patches
{
    [HarmonyPatch(typeof(SimGameState), "InitializeDataFromDefs")]
    public static class SimGameState_InitializeDataFromDefs_Patch
    {
        public static void Prefix(SimGameState __instance)
        {
            foreach (var defKVP in __instance.DataManager.SystemDefs)
            {
                ShopGeneration.TryAddItemCollections(defKVP.Value);
            }
        }
    }

    [HarmonyPatch(typeof(SimGameState), "Rehydrate")]
    public static class SimGameState_Rehydrate_Patch
    {
        public static void Prefix(SimGameState __instance)
        {
            foreach (var defKVP in __instance.DataManager.SystemDefs)
            {
                ShopGeneration.TryAddItemCollections(defKVP.Value);
            }
        }
    }
}
