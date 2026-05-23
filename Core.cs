using MelonLoader;
using Il2Cpp;
using Il2CppTLD.Gameplay;
using System;
using System.Linq;

[assembly: MelonInfo(typeof(FireImprovementsMod.Core), "FireImprovementsMod", "1.2.2", "NnicolaeN")]
[assembly: MelonGame("Hinterland", "TheLongDark")]
[assembly: MelonColor(255, 255, 120, 20)]  // orange — fire colour

namespace FireImprovementsMod
{
    internal sealed class Core : MelonMod
    {
        public const string Version = "1.2.2";
        internal static MelonLogger.Instance Logger;

        /// <summary>True if the Skill-Adjustment mod is loaded alongside ours.</summary>
        internal static bool SkillAdjustmentPresent { get; private set; } = false;

        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance;
            Settings.OnLoad();

            // Detect whether Skill-Adjustment is installed
            SkillAdjustmentPresent = MelonMod.RegisteredMelons
                .Any(m => m.Info.Name == "Skill-Adjustment");

            Logger.Msg(System.ConsoleColor.Yellow, "╔══════════════════════════════════════════╗");
            Logger.Msg(System.ConsoleColor.Yellow, "║      Fire Improvements Mod  v1.2.2      ║");
            Logger.Msg(System.ConsoleColor.Yellow, "╚══════════════════════════════════════════╝");
            Logger.Msg($"  Fuel burn multiplier  : x{Settings.instance.burnDurationMultiplier}");
            Logger.Msg($"  Max fire duration     : {Settings.instance.maxFireDurationHours}h (vanilla ~12h)");
            Logger.Msg($"  Wind resistance       : {Settings.instance.windResistancePercent}%");
            Logger.Msg($"  Fire-start bonus      : +{Settings.instance.fireStartBonusPercent}%");
            Logger.Msg($"  Indoor warmth bonus   : +{Settings.instance.indoorWarmthBonus}°C");
            Logger.Msg($"  Outdoor warmth bonus  : +{Settings.instance.outdoorWarmthBonus}°C");

            if (SkillAdjustmentPresent)
            {
                Logger.Msg(System.ConsoleColor.Cyan,
                    "  [Compat] Skill-Adjustment detected.");
                Logger.Msg(System.ConsoleColor.Cyan,
                    "    * Fire-start bonus (+" + Settings.instance.fireStartBonusPercent +
                    "%) stacks additively on top of Skill-Adjustment base chances.");
                Logger.Msg(System.ConsoleColor.Cyan,
                    "    * Burn duration multiplier (x" + Settings.instance.burnDurationMultiplier +
                    ") multiplies on top of Skill-Adjustment duration bonuses.");
            }
        }

        /// <summary>
        /// Called by MelonLoader after each scene finishes initialising.
        /// Re-applies all field-level overrides because the game may have
        /// re-created its manager components for the new scene.
        /// </summary>
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!IsPlayableScene(sceneName))
                return;

            Logger.Msg($"[Scene] '{sceneName}' — applying fire settings.");

            // Reset cached vanilla values; ExperienceMode may have been recreated
            s_baseIndoorWarmth  = float.NaN;
            s_baseOutdoorWarmth = float.NaN;

            ApplyFireManagerWarmth();
            ApplyMaxFireDuration();
        }

        // Cached vanilla values — recorded on first application per scene
        private static float s_baseIndoorWarmth  = float.NaN;
        private static float s_baseOutdoorWarmth = float.NaN;

        internal static void ApplyFireManagerWarmth()
        {
            if (Settings.instance.indoorWarmthBonus == 0 && Settings.instance.outdoorWarmthBonus == 0)
                return;

            try
            {
                var emm = GameManager.GetExperienceModeManagerComponent();
                if (emm == null)
                {
                    Logger?.Warning("[FireWarmth] ExperienceModeManager not found.");
                    return;
                }

                ExperienceMode em = emm.GetCurrentExperienceMode();
                if (em == null)
                {
                    Logger?.Warning("[FireWarmth] ExperienceMode not found.");
                    return;
                }

                // Record vanilla values once per scene so repeated calls don't stack
                if (float.IsNaN(s_baseIndoorWarmth))  s_baseIndoorWarmth  = em.m_MinAirTemperatureFromFireIndoors;
                if (float.IsNaN(s_baseOutdoorWarmth)) s_baseOutdoorWarmth = em.m_MinAirTemperatureFromFireOutdoors;

                float newIndoor  = s_baseIndoorWarmth  + Settings.instance.indoorWarmthBonus;
                float newOutdoor = s_baseOutdoorWarmth + Settings.instance.outdoorWarmthBonus;

                em.m_MinAirTemperatureFromFireIndoors  = newIndoor;
                em.m_MinAirTemperatureFromFireOutdoors = newOutdoor;

                Logger?.Msg($"[FireWarmth] Indoor  min fire temp: {s_baseIndoorWarmth}°C + {Settings.instance.indoorWarmthBonus} = {newIndoor}°C");
                Logger?.Msg($"[FireWarmth] Outdoor min fire temp: {s_baseOutdoorWarmth}°C + {Settings.instance.outdoorWarmthBonus} = {newOutdoor}°C");
            }
            catch (Exception ex)
            {
                Logger?.Warning($"[FireWarmth] Failed to apply warmth bonus: {ex.Message}");
            }
        }

        internal static void ApplyMaxFireDuration()
        {
            int maxHours = Settings.instance.maxFireDurationHours;
            if (maxHours == 0)
                return;  // 0 = leave vanilla value untouched

            try
            {
                var fm = GameManager.GetFireManagerComponent();
                if (fm == null)
                {
                    Logger?.Warning("[MaxDuration] FireManager not found.");
                    return;
                }

                float vanilla = fm.m_MaxDurationHoursOfFire;
                fm.m_MaxDurationHoursOfFire = maxHours;
                Logger?.Msg($"[MaxDuration] Max fire duration: {vanilla}h (vanilla) -> {maxHours}h (modded)");
            }
            catch (Exception ex)
            {
                Logger?.Warning($"[MaxDuration] Failed: {ex.Message}");
            }
        }

        // Playable scenes use the game mode as a suffix, e.g.:
        //   "ChurchB_SANDBOX", "MysteryLake_SANDBOX", "WintermuteEp1_STORY"
        // Do NOT use StartsWith — the mode token is at the end, not the start.
        private static bool IsPlayableScene(string name)
        {
            return name.Contains("SANDBOX")
                || name.Contains("WINTERMUTE")
                || name.Contains("STORY");
        }
    }
}
