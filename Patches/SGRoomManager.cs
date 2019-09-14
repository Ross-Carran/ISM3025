using System.IO;
using BattleTech.UI;
using Harmony;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace ISM3025.Patches
{
    [HarmonyPatch(typeof(SGRoomManager), "OnSimGameReady")]
    public static class SGRoomManager_EnterRoom_Patch
    {
        public static void Postfix()
        {
            if (string.IsNullOrEmpty(Main.Settings.NavRoomScreenTexturePath))
                return;

            var path = Path.Combine(Main.ModDir, Main.Settings.NavRoomScreenTexturePath);
            if (!File.Exists(path))
                return;

            // from https://answers.unity.com/questions/432655/loading-texture-file-from-pngjpg-file-on-disk.html
            var texture2d = new Texture2D(2, 2);
            //texture2d.LoadImage(File.ReadAllBytes(path));

            var argoRenderer = GameObject.Find("VisibleStarmap")?.GetComponent<MeshRenderer>();
            if (argoRenderer != null)
                argoRenderer.material.mainTexture = texture2d;

            var leopardRenderer = GameObject.Find("VisibleStarmap (1)")?.GetComponent<MeshRenderer>();
            if (leopardRenderer != null)
                leopardRenderer.material.mainTexture = texture2d;
        }
    }
}
