using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace ISM3025.Features
{
    public static class ShopGeneration
    {
        public static void TryAddItemCollections(StarSystemDef def)
        {
            // only add to systems with this tag if it exists
            var enableTag = Main.Settings.GenerateShopsTag;
            if (!string.IsNullOrEmpty(enableTag) && !def.Tags.Contains(enableTag))
                return;

            // if a system has one of these tags, we don't want to add stuff
            foreach (var ignoreTag in Main.Settings.GenerateShopsIgnoreTags)
            {
                if (def.Tags.Contains(ignoreTag))
                    return;
            }

            if (def.SystemShopItems == null)
                Traverse.Create(def).Property("SystemShopItems").SetValue(new List<string>());

            // add each tag -> itemCollection ID
            foreach (var tag in Main.Settings.TagToShopItemCollection.Keys)
            {
                if (!def.Tags.Contains(tag))
                    continue;

                def.SystemShopItems?.Add(Main.Settings.TagToShopItemCollection[tag]);
            }
        }
    }
}
