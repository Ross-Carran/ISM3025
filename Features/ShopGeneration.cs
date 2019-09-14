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
            bool ProgressTag = false;
            foreach (string tag in def.Tags)
            {
                if (tag.StartsWith("planet_progress"))
                    ProgressTag = true;
            }
            if (!ProgressTag)
                def.Tags.Add("planet_progress_3");
            else
                return;

            //var sim = UnityGameInstance.BattleTechGame.Simulation;
            //if (sim.IsCampaign && !sim.CompanyTags.Contains("map_travel_3"))
            //    return;

            // only add to systems with this tag if it exists
            var enableTag = Main.Settings.GenerateShopsTag;
            if (!string.IsNullOrEmpty(enableTag) && !def.Tags.Contains(enableTag))
                return;

            // add Black Market. Even "abandoned" systems can have Black Markets.
            if (def.BlackMarketShopItems == null)
                Traverse.Create(def).Property("BlackMarketShopItems").SetValue(new List<string>());

            if (def.Tags.Contains("planet_other_blackmarket"))
                def.BlackMarketShopItems.Add("itemCollection_faction_AuriganPirates");

            // if a system has one of these tags, we don't want to add stuff
            foreach (var ignoreTag in Main.Settings.GenerateShopsIgnoreTags)
            {
                if (def.Tags.Contains(ignoreTag))
                    return;
            }

            // if a system is a faction HQ, add the faction shop
            if (def.FactionShopItems == null)
                Traverse.Create(def).Property("FactionShopItems").SetValue(new List<string>());

            if (def.Tags.Any(x => x.StartsWith("planet_other_factionhq")) && def.Owner != Faction.ComStar)
            {
                Traverse.Create(def).Property("FactionShopOwner").SetValue(def.Owner);
                string FactionStoreName = "itemCollection_faction_" + def.Owner.ToString();
                def.FactionShopItems.Add(FactionStoreName);
            }

            //add the system shop items
            if (def.SystemShopItems == null)
                Traverse.Create(def).Property("SystemShopItems").SetValue(new List<string>());

            // add each tag -> itemCollection ID
            foreach (var tag in Main.Settings.TagToShopItemCollection.Keys)
            {
                if (!def.Tags.Contains(tag))
                    continue;

                if (tag.StartsWith("planet_faction") && (def.Tags.Contains("planet_other_megacity") || def.Tags.Contains("planet_pop_large")))
                    tag.Replace("minor", "major");

                def.SystemShopItems?.Add(Main.Settings.TagToShopItemCollection[tag]);
            }
        }
    }
}
