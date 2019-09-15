using System.Collections.Generic;
using BattleTech;
using Harmony;
using System.Linq;

namespace ISM3025.Features
{
    public static class ShopGeneration
    {
        public static void TryAddItemCollections(StarSystemDef def)
        {
            // Need to tag all systems for opening up the systems after campaign is done
            if (!def.Tags.Any(tag => tag.StartsWith("planet_progress")))
                def.Tags.Add("planet_progress_3");

            // only add to systems with this tag if it exists
            var enableTag = Main.Settings.GenerateShopsTag;
            if (!string.IsNullOrEmpty(enableTag) && !def.Tags.Contains(enableTag))
                return;

            // add Black Market. Even "abandoned" systems can have Black Markets.
            if (def.Tags.Contains("planet_other_blackmarket"))
            {
                if (def.BlackMarketShopItems == null)
                    Traverse.Create(def).Property("BlackMarketShopItems").SetValue(new List<string>());

                def.BlackMarketShopItems?.Add("itemCollection_faction_AuriganPirates");
            }

            // if a system has one of these tags, we don't want to add stuff
            foreach (var ignoreTag in Main.Settings.GenerateShopsIgnoreTags)
            {
                if (def.Tags.Contains(ignoreTag))
                    return;
            }

            // if a system is a faction HQ, add the faction shop
            if (def.Tags.Any(x => x.StartsWith("planet_other_factionhq")) && def.Owner != Faction.ComStar)
            {
                if (def.FactionShopItems == null)
                    Traverse.Create(def).Property("FactionShopItems").SetValue(new List<string>());

                Traverse.Create(def).Property("FactionShopOwner").SetValue(def.Owner);
                var factionStoreName = $"itemCollection_faction_{def.Owner}";
                def.FactionShopItems?.Add(factionStoreName);
            }

            // add the system shop items
            if (def.SystemShopItems == null)
                Traverse.Create(def).Property("SystemShopItems").SetValue(new List<string>());

            // add each tag -> itemCollection ID
            foreach (var tag in Main.Settings.TagToShopItemCollection.Keys)
            {
                var mutableTag = tag;
                if (!def.Tags.Contains(tag))
                    continue;

                if (tag.StartsWith("planet_faction") && (def.Tags.Contains("planet_other_megacity") || def.Tags.Contains("planet_pop_large")))
                    mutableTag = tag.Replace("minor", "major");

                def.SystemShopItems?.Add(Main.Settings.TagToShopItemCollection[mutableTag]);
            }
        }
    }
}
