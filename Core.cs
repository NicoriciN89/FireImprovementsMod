using MelonLoader;
using Il2Cpp;
using Il2CppTLD.Gameplay;
using System;
using System.Linq;

// ===== Метаданные мода =====
[assembly: MelonInfo(typeof(FireImprovementsMod.Core), "FireImprovementsMod", "1.2.1", "NnicolaeN")]
[assembly: MelonGame("Hinterland", "TheLongDark")]
[assembly: MelonColor(255, 255, 120, 20)]  // Оранжевый — цвет огня

namespace FireImprovementsMod
{
    internal sealed class Core : MelonMod
    {
        public const string Version = "1.2.1";
        internal static MelonLogger.Instance Logger;

        /// <summary>true если мод Skill-Adjustment загружен одновременно с нашим.</summary>
        internal static bool SkillAdjustmentPresent { get; private set; } = false;

        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance;
            Settings.OnLoad();

            // Определяем, установлен ли мод Skill-Adjustment
            SkillAdjustmentPresent = MelonMod.RegisteredMelons
                .Any(m => m.Info.Name == "Skill-Adjustment");

            Logger.Msg(System.ConsoleColor.Yellow, "╔══════════════════════════════════════════╗");
            Logger.Msg(System.ConsoleColor.Yellow, "║      Fire Improvements Mod  v1.2.1      ║");
            Logger.Msg(System.ConsoleColor.Yellow, "╚══════════════════════════════════════════╝");
            Logger.Msg($"  Топливо горит дольше x{Settings.instance.burnDurationMultiplier}");
            Logger.Msg($"  Макс. время горения: {Settings.instance.maxFireDurationHours}ч (было 12ч)");
            Logger.Msg($"  Защита от ветра: {Settings.instance.windResistancePercent}%");
            Logger.Msg($"  Бонус к розжигу: +{Settings.instance.fireStartBonusPercent}%");
            Logger.Msg($"  Тепло в помещении: +{Settings.instance.indoorWarmthBonus}°C");
            Logger.Msg($"  Тепло на улице: +{Settings.instance.outdoorWarmthBonus}°C");

            if (SkillAdjustmentPresent)
            {
                Logger.Msg(System.ConsoleColor.Cyan,
                    "  [Совм.] Skill-Adjustment обнаружен.");
                Logger.Msg(System.ConsoleColor.Cyan,
                    "    • Бонус к розжигу (+" + Settings.instance.fireStartBonusPercent +
                    "%) суммируется с базовыми шансами из Skill-Adjustment.");
                Logger.Msg(System.ConsoleColor.Cyan,
                    "    • Множитель горения топлива (x" + Settings.instance.burnDurationMultiplier +
                    ") перемножается с бонусом длительности из Skill-Adjustment.");
            }
        }

        /// <summary>
        /// Применяем бонус тепла к FireManager при загрузке игровой сцены.
        /// </summary>
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            // Игровые сцены начинаются с "LV_" (локации) или "SANDBOX"
            if (!IsPlayableScene(sceneName))
                return;

            // Сбрасываем кэш при смене локации — ExperienceMode мог пересоздаться
            s_baseIndoorWarmth  = float.NaN;
            s_baseOutdoorWarmth = float.NaN;

            ApplyFireManagerWarmth();
            ApplyMaxFireDuration();
        }

        // Базовые (ванильные) значения — сохраняем при первом применении
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
                    Logger?.Warning("[FireWarmth] ExperienceModeManager не найден.");
                    return;
                }

                ExperienceMode em = emm.GetCurrentExperienceMode();
                if (em == null)
                {
                    Logger?.Warning("[FireWarmth] ExperienceMode не найден.");
                    return;
                }

                // Запоминаем ванильные значения один раз
                if (float.IsNaN(s_baseIndoorWarmth))  s_baseIndoorWarmth  = em.m_MinAirTemperatureFromFireIndoors;
                if (float.IsNaN(s_baseOutdoorWarmth)) s_baseOutdoorWarmth = em.m_MinAirTemperatureFromFireOutdoors;

                // Всегда считаем от базы — не накапливаемся при повторных вызовах
                em.m_MinAirTemperatureFromFireIndoors  = s_baseIndoorWarmth  + Settings.instance.indoorWarmthBonus;
                em.m_MinAirTemperatureFromFireOutdoors = s_baseOutdoorWarmth + Settings.instance.outdoorWarmthBonus;

                Logger?.Msg($"[FireWarmth] Тепло в помещении: {em.m_MinAirTemperatureFromFireIndoors}°C  (улица: {em.m_MinAirTemperatureFromFireOutdoors}°C)");
            }
            catch (Exception ex)
            {
                Logger?.Warning($"[FireWarmth] Ошибка применения бонуса тепла: {ex.Message}");
            }
        }

        internal static void ApplyMaxFireDuration()
        {
            int maxHours = Settings.instance.maxFireDurationHours;
            if (maxHours == 0)
                return;  // 0 = ванильное значение, не трогаем

            try
            {
                var fm = GameManager.GetFireManagerComponent();
                if (fm == null)
                {
                    Logger?.Warning("[MaxDuration] FireManager не найден.");
                    return;
                }

                fm.m_MaxDurationHoursOfFire = maxHours;
                Logger?.Msg($"[MaxDuration] Максимальное время горения: {maxHours}ч");
            }
            catch (Exception ex)
            {
                Logger?.Warning($"[MaxDuration] Ошибка: {ex.Message}");
            }
        }

        // Playable scenes are e.g. "ChurchB_SANDBOX", "MysteryLake_SANDBOX", "WintermuteEp1_STORY" …
        // Do NOT use StartsWith — scene names have the mode suffix, not a prefix.
        private static bool IsPlayableScene(string name)
        {
            return name.Contains("SANDBOX")
                || name.Contains("WINTERMUTE")
                || name.Contains("STORY");
        }
    }
}
