using System;
using BattleTech;
using Harmony;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace ISM3025.Patches
{
    [HarmonyPatch(typeof(StarmapRenderer), "RefreshSystems")]
    public static class StarmapRenderer_RefreshSystems_Patch
    {
        public static void Postfix(StarmapRenderer __instance)
        {
            // disable all of the logos
            // TODO: make them dynamic based on systems
            var logoNames = new[]
            {
                "davionLogo", "marikLogo", "directorateLogo", "liaoLogo",
                "taurianLogo", "magistracyLogo", "restorationLogo"
            };

            foreach (var logoName in logoNames)
            {
                var logoObj = GameObject.Find(logoName);
                logoObj?.SetActive(false);
            }

            // set the fov to our settings
            __instance.fovMin = Main.Settings.MinFov;
            __instance.fovMax = Main.Settings.MaxFov;

            // set scale of region borders for our new size
            var border = GameObject.Find("RegionBorders");
            var starmapBorders = border?.GetComponent<StarmapBorders>();

            if (border != null)
                border.transform.localScale = new Vector3(4f * Main.Settings.MapHeight, 4f * Main.Settings.MapWidth);

            if (starmapBorders != null)
            {
                // generate black texture to use for plusTex, so that we don't have a
                // stretched texture, it looked weird anyways
                var textureSize = 64;
                var blackTexture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
                for (var x = 0; x < textureSize; x++)
                {
                    for (var y = 0; y < textureSize; y++)
                    {
                        blackTexture.SetPixel(x, y, Color.black);
                    }
                }
                blackTexture.Apply();
                starmapBorders.plusTex = blackTexture;
            }

            // disable the map edges
            // TODO: simply move them instead!
            GameObject.Find("Edges")?.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(StarmapRenderer), "PlaceLogo")]
    public static class StarmapRenderer_PlaceLogo_Patch
    {
        public static bool Prefix()
        {
            // TODO: change this to just cancel placing logos when we're not doing it
            return false;
        }
    }

    [HarmonyPatch(typeof(StarmapRenderer), "FactionColor")]
    public static class StarmapRenderer_FactionColor_Patch
    {
        public static bool Prefix(ref Color __result, Faction thisFaction)
        {
            var factionString = Enum.GetName(typeof(Faction), thisFaction);
            if (factionString == null || !Main.Settings.FactionColors.ContainsKey(factionString))
                return true;

            var c = Main.Settings.FactionColors[factionString];
            __result = new Color(c[0], c[1], c[2], c[3]);

            return false;
        }
    }

    [HarmonyPatch(typeof(StarmapRenderer), "NormalizeToMapSpace")]
    public static class StarmapRenderer_NormalizeToMapSpace_Patch
    {
        // ReSharper disable once RedundantAssignment
        public static bool Prefix(Vector2 normalizedPos, ref Vector3 __result)
        {
            Vector3 mapSpacePosition = normalizedPos;
            mapSpacePosition.x = (mapSpacePosition.x * 2f - 1f) * Main.Settings.MapHeight;
            mapSpacePosition.y = (mapSpacePosition.y * 2f - 1f) * Main.Settings.MapWidth;
            mapSpacePosition.z = 0f;

            __result = mapSpacePosition;
            return false;
        }
    }
}
