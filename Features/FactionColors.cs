using System;
using BattleTech;
using UnityEngine;

namespace ISM3025.Features
{
    public static class FactionColors
    {
        public static Color? GetModdedFactionColor(Faction faction)
        {
            var factionString = Enum.GetName(typeof(Faction), faction);
            if (factionString == null || !Main.Settings.FactionColors.ContainsKey(factionString))
                return null;

            var c = Main.Settings.FactionColors[factionString];
            return new Color(c[0], c[1], c[2], c[3]);
        }
    }
}
