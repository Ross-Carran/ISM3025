using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace ISM3025
{
    internal class ModSettings
    {
        public float MapWidth = 100f;
        public float MapHeight = 100f;

        public float MinFov = 15f;
        public float MaxFov = 100f;

        public float StarHitboxSize = 1.5f;
        public float BorderMargin = 5f;

        public Dictionary<string, float[]> FactionColors = new Dictionary<string, float[]>();
        public Dictionary<string, string> FactionLogoPaths = new Dictionary<string, string>();

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
