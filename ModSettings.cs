using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace ISM3025
{
    internal class ModSettings
    {
        // TODO: add feature to not resize in campaign
        public float MapWidth = 200f;
        public float MapHeight = 200f;

        public float MapMargin = 50f;
        public float MapBottomMargin = 100f;

        public float MinFov = 20f;
        public float MaxFov = 125f;

        public bool UseSmoothZoom = true;
        public float SmoothZoomTime = 0.15f;

        public float StarHitboxSize = 2f;
        public float BorderMargin = 5f;
        public float LogoScalar = 0.75f;
        public float LogoMaxSize = 50f;

        public string NavRoomScreenTexturePath = "";

        public Dictionary<string, float[]> FactionColors = new Dictionary<string, float[]>();
        public Dictionary<string, string> FactionLogoPaths = new Dictionary<string, string>();

        public string GenerateShopsTag = "mod_generate_shops";
        public List<string> GenerateShopsIgnoreTags = new List<string>();
        public Dictionary<string, string> TagToShopItemCollection = new Dictionary<string, string>();

        public static ModSettings Parse(string json)
        {
            ModSettings settings;

            try
            {
                settings = JsonConvert.DeserializeObject<ModSettings>(json);
            }
            catch (Exception e)
            {
                Main.HBSLog.Log($"Reading settings failed: {e.Message}");
                settings = new ModSettings();
            }

            return settings;
        }
    }
}
