using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
            ResizableMap.OnMapRefresh(__instance);
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
            var squareMapSpace = ResizableMap.NormalizeToSquareMapSpace(normalizedPos);
            if (squareMapSpace == null)
                return true;

            __result = squareMapSpace.Value;
            return false;
        }
    }

    [HarmonyPatch(typeof(StarmapRenderer), "Update")]
    public static class StarmapRenderer_Update_Patch
    {
        // almost all of this patch is based on Morph's InnerSphereMap
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // this simply removes `this.starmapCamera.transform.position = position3;`
            var instructionList = instructions.ToList();

            var setTransformPosition = AccessTools.Property(typeof(Transform), nameof(Transform.position)).GetSetMethod();
            var setPositionIndex = instructionList.FindLastIndex(instruction =>
                instruction.opcode == OpCodes.Callvirt && setTransformPosition.Equals(instruction.operand));

            for (var i = 0; i < 5; i++)
                instructionList[setPositionIndex - i].opcode = OpCodes.Nop;

            return instructionList;
        }

        public static void Prefix(StarmapRenderer __instance)
        {
            SmoothZoom.PreUpdate(__instance);
        }

        public static void Postfix(StarmapRenderer __instance)
        {
            SmoothZoom.PostUpdate(__instance);

            // clamp camera position
            var cameraPosition = __instance.starmapCamera.transform.position;
            var fov = __instance.starmapCamera.fieldOfView;
            var zPos = __instance.starmapCamera.transform.position.z;

            __instance.starmapCamera.transform.position = ResizableMap.ClampCameraPosition(cameraPosition, fov, zPos);
        }
    }
}
