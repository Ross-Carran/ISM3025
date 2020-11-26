using BattleTech;
using Harmony;
using ISM3025.Features;
using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace ISM3025.Patches
{
    

    [HarmonyPatch(typeof(SimGameState), "InitializeDataFromDefs")]
    public static class SimGameState_InitializeDataFromDefs_Patch
    {
        public static void Postfix(SimGameState __instance)
        {
            if (!(__instance.IsFromSave))
            {
                foreach (var defKVP in __instance.DataManager.SystemDefs)
                {
                    ShopGeneration.TryAddItemCollections(defKVP.Value);
                }
            }
        }
    } 
}
