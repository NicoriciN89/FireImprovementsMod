using Il2Cpp;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace FireImprovementsMod
{
    /// <summary>
    /// Loads FI.* translations from localization.json shipped alongside the DLL.
    /// User override: UserData/FireImprovementsMod/localization.json takes priority.
    /// Falls back to built-in English strings if the JSON is missing or corrupt.
    /// </summary>
    internal static class LocalizationManager
    {
        private static Dictionary<string, Dictionary<string, string>> _data;

        internal static Dictionary<string, Dictionary<string, string>> Data
        {
            get { return _data ?? (_data = Load()); }
        }

        internal static void Reload() => _data = null;

        private static Dictionary<string, Dictionary<string, string>> Load()
        {
            // Bundled path: same folder as the DLL (e.g. Mods/localization.json)
            string dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            string bundledPath = Path.Combine(dllDir, "localization.json");

            // User override: UserData/FireImprovementsMod/localization.json
            string userPath = Path.Combine(
                Path.GetDirectoryName(dllDir) ?? dllDir,  // go up from Mods/ to game root
                "UserData", "FireImprovementsMod", "localization.json");

            string filePath =
                File.Exists(userPath)    ? userPath :
                File.Exists(bundledPath) ? bundledPath :
                string.Empty;

            if (filePath != string.Empty)
            {
                try
                {
                    string json = File.ReadAllText(filePath, Encoding.UTF8);
                    var result = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
                    if (result != null)
                    {
                        MelonLogger.Msg($"[FireImprovementsMod] Loaded localization from: {filePath}");
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[FireImprovementsMod] Could not load localization.json: {ex.Message}");
                }
            }
            else
            {
                MelonLogger.Warning("[FireImprovementsMod] localization.json not found — using built-in English strings.");
            }

            return FallbackEnglish;
        }

        // Minimal English fallback so the mod works even without the JSON file.
        private static readonly Dictionary<string, Dictionary<string, string>> FallbackEnglish = new()
        {
            ["English"] = new()
            {
                ["FI.SEC_DURATION"]  = "Fuel Burn Duration",
                ["FI.SEC_WIND"]      = "Wind Resistance",
                ["FI.SEC_FIRESTART"] = "Fire Starting",
                ["FI.SEC_WARMTH"]    = "Fire Warmth",
                ["FI.DURATION_MULT"]  = "Burn Duration Multiplier",
                ["FI.MAX_DURATION"]   = "Max Fire Duration (hours)",
                ["FI.WIND_RESIST"]    = "Wind Blowout Resistance (%)",
                ["FI.START_BONUS"]    = "Fire Start Bonus (%)",
                ["FI.INDOOR_WARMTH"]  = "Indoor Warmth Bonus (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Outdoor Warmth Bonus (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Fuel burn time multiplier.\n1.0 = no change | 1.5 = 50% longer | 2.0 = twice as long.\nAffects all fuel: wood, coal, accelerant.",
                ["FI.DESC_MAX_DURATION"]   = "Maximum possible fire burn time in hours.\n12 = vanilla | 24 = one day | 72 = three days | 200 = eight days.",
                ["FI.DESC_WIND_RESIST"]    = "Chance (%) that wind does NOT blow out the fire.\n0 = no protection | 50 = half as often | 100 = never.",
                ["FI.DESC_START_BONUS"]    = "Flat bonus added to fire start success chance.\n0 = no change | 15 = +15% to success.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Extra warmth from fire indoors (stoves, fireplaces).\n0 = no change | 5 = +5\u00b0C.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Extra warmth from fire outdoors (campfires).\n0 = no change | 3 = +3\u00b0C.",
            }
        };

        internal static string Get(string key)
        {
            string lang = Localization.Language ?? "English";
            var data = Data;
            if (data.TryGetValue(lang, out var dict) && dict.TryGetValue(key, out string val))
                return val;
            if (data.TryGetValue("English", out var en) && en.TryGetValue(key, out string enVal))
                return enVal;
            return key;
        }
    }

    [HarmonyPatch(typeof(Localization), nameof(Localization.Get))]
    internal static class LocalizationPatch
    {
        static void Postfix(string __0, ref string __result)
        {
            if (__0 == null || !__0.StartsWith("FI."))
                return;
            __result = LocalizationManager.Get(__0);
        }
    }

    /// <summary>
    /// Патч на DescriptionHolder.get_Text (ModSettings) —
    /// перехватывает чтение описания в момент показа, когда язык уже установлен.
    /// </summary>
    [HarmonyPatch]
    internal static class DescriptionTextTranslatePatch
    {
        static System.Reflection.MethodBase TargetMethod() =>
            AccessTools.PropertyGetter(
                AccessTools.TypeByName("ModSettings.DescriptionHolder"), "Text");

        static void Postfix(ref string __result)
        {
            if (__result == null || !__result.StartsWith("FI."))
                return;
            __result = LocalizationManager.Get(__result);
        }
    }
}