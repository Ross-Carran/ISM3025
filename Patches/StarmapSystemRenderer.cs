using BattleTech;
using Harmony;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace ISM3025.Patches
{
    [HarmonyPatch(typeof(StarmapSystemRenderer), "Init")]
    public static class StarmapSystemRenderer_Init_Patch
    {
        public static void Postfix(StarmapSystemRenderer __instance)
        {
            var collider = __instance?.gameObject?.GetComponent<BoxCollider>();
            if (collider == null)
                return;

			var size = new Vector3(Main.Settings.StarHitboxSize, Main.Settings.StarHitboxSize, 0.1f);
            collider.size = size;
        }
    }
}
