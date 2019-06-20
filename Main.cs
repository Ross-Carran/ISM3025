using System.Reflection;
using Harmony;
using HBS.Logging;

namespace ISM3025
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;
        internal static string ModDir;

        public static void Init(string modDir, string settings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.ISM3025");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("ISM3025");
            Settings = ModSettings.Parse(settings);
            ModDir = modDir;
        }
    }
}
