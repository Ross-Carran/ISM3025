using System;
using System.Collections.Generic;
using System.IO;
using BattleTech;
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
            var factions = Enum.GetValues(typeof(Faction));
            var logos = new Dictionary<Faction, GameObject>();

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
                        logo.transform.SetParent(renderer.restorationLogo.transform.parent);
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

                logos.Add(faction, logo);
            }

            PlaceAndScaleLogos(logos, renderer);
        }

        public static void PlaceAndScaleLogos(Dictionary<Faction, GameObject> logos, StarmapRenderer renderer)
        {
            var boundingRects = new Dictionary<Faction, BoundingRect>();

            foreach (var starNode in renderer.starmap.VisisbleSystem)
            {
                var faction = starNode.System.Owner;
                if (!logos.ContainsKey(faction))
                    continue;

                BoundingRect boundingRect;
                if (boundingRects.ContainsKey(faction))
                {
                    boundingRect = boundingRects[faction];
                }
                else
                {
                    boundingRect = new BoundingRect();
                    boundingRects.Add(faction, boundingRect);
                }

                boundingRect.MinX = Mathf.Min(boundingRect.MinX, starNode.NormalizedPosition.x);
                boundingRect.MaxX = Mathf.Max(boundingRect.MaxX, starNode.NormalizedPosition.x);
                boundingRect.MinY = Mathf.Min(boundingRect.MinY, starNode.NormalizedPosition.y);
                boundingRect.MaxY = Mathf.Max(boundingRect.MaxY, starNode.NormalizedPosition.y);
            }

            foreach (var faction in logos.Keys)
            {
                if (!boundingRects.ContainsKey(faction))
                    continue;

                var logo = logos[faction];

                var boundingRect = boundingRects[faction];
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (boundingRect.MinX == float.MaxValue)
                {
                    // there were no star systems
                    logo.SetActive(false);
                    continue;
                }

                logo.SetActive(true);

                // position is in the middle of the boundingRect
                var x = (boundingRect.MaxX + boundingRect.MinX) / 2f;
                var y = (boundingRect.MaxY + boundingRect.MinY) / 2f;
                var normalizedPos = new Vector2(x, y);
                logo.transform.position = StarmapRenderer.NormalizeToMapSpace(normalizedPos);

                // scale is based off of the width/height of the boundingRect
                var topRight = StarmapRenderer.NormalizeToMapSpace(new Vector2(boundingRect.MaxX, boundingRect.MaxY));
                var bottomLeft = StarmapRenderer.NormalizeToMapSpace(new Vector2(boundingRect.MinX, boundingRect.MinY));
                var width = topRight.x - bottomLeft.x;
                var height = topRight.y - bottomLeft.y;

                var scale = Mathf.Min(Mathf.Min(width, height) * Main.Settings.LogoScalar, Main.Settings.LogoMaxSize);
                logo.transform.localScale = new Vector3(scale, scale);
            }
        }

        private class BoundingRect
        {
            public float MinX = float.MaxValue;
            public float MaxX = float.MinValue;
            public float MinY = float.MaxValue;
            public float MaxY = float.MinValue;
        }
    }
}
