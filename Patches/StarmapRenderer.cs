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
            var instructionList = instructions.ToList();

            var setTransformPosition = AccessTools.Property(typeof(Transform), nameof(Transform.position)).GetSetMethod();
            var setPositionIndex = instructionList.FindLastIndex(instruction =>
                instruction.opcode == OpCodes.Callvirt && setTransformPosition.Equals(instruction.operand));

            // this simply removes `this.starmapCamera.transform.position = position3;`
            // taken from Morph's InnerSphereMap
            instructionList[setPositionIndex - 4].opcode = OpCodes.Nop;
            instructionList[setPositionIndex - 3].opcode = OpCodes.Nop;
            instructionList[setPositionIndex - 2].opcode = OpCodes.Nop;
            instructionList[setPositionIndex - 1].opcode = OpCodes.Nop;
            instructionList[setPositionIndex].opcode = OpCodes.Nop;

            return instructionList;
        }

        public static void Postfix(StarmapRenderer __instance)
        {
            var zPos = __instance.starmapCamera.transform.position.z;
            var fov = __instance.starmapCamera.fieldOfView;

            // TODO: move to ResizableMap feature
            var xView = GetViewSize(Mathf.Abs(zPos), GetHorizontalFov(fov));
            var yView = GetViewSize(Mathf.Abs(zPos), fov);

            var current = __instance.starmapCamera.transform.position;
            var clamped = current;

            var leftBoundary = -Main.Settings.MapWidth - Main.Settings.MapMargin + xView;
            var rightBoundary = Main.Settings.MapWidth + Main.Settings.MapMargin - xView;
            var bottomBoundary = -Main.Settings.MapHeight - Main.Settings.MapBottomMargin + yView;
            var topBoundary = Main.Settings.MapHeight + Main.Settings.MapMargin - yView;

            var totalWidth = Main.Settings.MapWidth * 2f + 2f * Main.Settings.MapMargin;
            var totalHeight = Main.Settings.MapHeight * 2f + Main.Settings.MapMargin + Main.Settings.MapBottomMargin;

            clamped.x = xView * 2f >= totalWidth ? 0
                : Mathf.Clamp(current.x, leftBoundary, rightBoundary);

            clamped.y = yView * 2f >= totalHeight ? 0
                : Mathf.Clamp(current.y, bottomBoundary, topBoundary);

            __instance.starmapCamera.transform.position = clamped;
        }

        private static float GetViewSize(float zPos, float fov)
        {
            return zPos * Mathf.Tan(fov * Mathf.Deg2Rad / 2.0f);
        }

        private static float GetHorizontalFov(float verticalFov)
        {
            return 2.0f * Mathf.Atan((16f/9f) * Mathf.Tan(verticalFov * Mathf.Deg2Rad / 2.0f)) * Mathf.Rad2Deg;
        }
    }
}
