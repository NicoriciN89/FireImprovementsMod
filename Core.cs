using MelonLoader;
using Il2Cpp;
using Il2CppTLD.Gameplay;
using System;

// ===== Метаданные мода =====
[assembly: MelonInfo(typeof(FireImprovementsMod.Core), "FireImprovementsMod", "1.0.0", "Marvin")]
[assembly: MelonGame("Hinterland", "TheLongDark")]
[assembly: MelonColor(255, 255, 120, 20)]  // Оранжевый — цвет огня

namespace FireImprovementsMod
{
    internal sealed class Core : MelonMod
    {
        public const string Version = "1.0.0";
        internal static MelonLogger.Instance Logger;

        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance;
            Settings.OnLoad();

            Logger.Msg(System.ConsoleColor.Yellow, "╔══════════════════════════════════════════╗");
            Logger.Msg(System.ConsoleColor.Yellow, "║      Fire Improvements Mod  v1.0.0      ║");
            Logger.Msg(System.ConsoleColor.Yellow, "╚══════════════════════════════════════════╝");
            Logger.Msg($"  Топливо горит дольше x{Settings.instance.burnDurationMultiplier}");
            Logger.Msg($"  Макс. время горения: {Settings.instance.maxFireDurationHours}ч (было 12ч)");
            Logger.Msg($"  Защита от ветра: {Settings.instance.windResistancePercent}%");
            Logger.Msg($"  Бонус к розжигу: +{Settings.instance.fireStartBonusPercent}%");
            Logger.Msg($"  Тепло в помещении: +{Settings.instance.indoorWarmthBonus}°C");
            Logger.Msg($"  Тепло на улице: +{Settings.instance.outdoorWarmthBonus}°C");
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

        // Сбрасываем кэш базовых значений при загрузке новой сцены
        private static bool IsPlayableScene(string name)
        {
            return name.StartsWith("LV_")
                || name.StartsWith("SANDBOX")
                || name == "MainMenu"
                || name.StartsWith("WINTERMUTE");
        }
    }
}
