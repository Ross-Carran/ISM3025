using System;
using System.Collections.Generic;
using System.IO;
using BattleTech;
using Harmony;
using UnityEngine;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace ISM3025.Features
{
    public static class DynamicLogos
    {
        private static Dictionary<string, string> _vanillaLogoNames = new Dictionary<string, string>
        {
            { "Davion", "davionLogo" },
            { "Marik", "marikLogo" },
            { "AuriganDirectorate", "directorateLogo" },
            { "Liao", "liaoLogo" },
            { "TaurianConcordat", "taurianLogo" },
            { "MagistracyOfCanopus", "magistracyLogo" },
            { "AuriganRestoration", "restorationLogo" },

        };

        public static void PlaceLogos(StarmapRenderer renderer)
        {
            var rendererTraverse = Traverse.Create(renderer);
            var factions = Enum.GetValues(typeof(Faction));

            foreach (Faction faction in factions)
            {
                var factionName = Enum.GetName(typeof(Faction), faction);
                if (factionName == null)
                    continue;

                // use the vanilla object if it exists, make our own if it doesn't
                GameObject logo = null;
                if (_vanillaLogoNames.ContainsKey(factionName))
                {
                    logo = GameObject.Find(_vanillaLogoNames[factionName]);
                }
                else if (Main.Settings.FactionLogoPaths.ContainsKey(factionName))
                {
                    var logoName = $"{factionName.ToLower()}Logo";

                    logo = GameObject.Find(logoName);
                    if (logo == null)
                    {
                        logo = GameObject.Instantiate(renderer.restorationLogo);
                        logo.name = logoName;
                    }
                }

                if (logo == null)
                    continue;

                if (Main.Settings.FactionLogoPaths.ContainsKey(factionName))
                {
                    var path = Path.Combine(Main.ModDir, Main.Settings.FactionLogoPaths[factionName]);

                    // from https://answers.unity.com/questions/432655/loading-texture-file-from-pngjpg-file-on-disk.html
                    var texture2d = new Texture2D(2, 2);
                    texture2d.LoadImage(File.ReadAllBytes(path));

                    logo.GetComponent<Renderer>().material.mainTexture = texture2d;
                }

                rendererTraverse.Method("PlaceLogo", faction, logo).GetValue();
            }
        }
    }
}
