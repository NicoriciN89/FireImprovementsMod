using ModSettings;

namespace FireImprovementsMod
{
    internal class Settings : JsonModSettings
    {
        internal static Settings instance = new();

        // ═══════════════════════════════════════════
        //   ДЛИТЕЛЬНОСТЬ ГОРЕНИЯ
        // ═══════════════════════════════════════════
        [Section("FI.SEC_DURATION", Localize = true)]

        [Name("FI.DURATION_MULT", Localize = true)]
        [Description("FI.DESC_DURATION_MULT", Localize = true)]
        [Slider(1.0f, 3.0f, 21)]
        public float burnDurationMultiplier = 1.5f;

        [Name("FI.MAX_DURATION", Localize = true)]
        [Description("FI.DESC_MAX_DURATION", Localize = true)]
        [Slider(0, 200, 201)]
        public int maxFireDurationHours = 24;

        // ═══════════════════════════════════════════
        //   ЗАЩИТА ОТ ВЕТРА
        // ═══════════════════════════════════════════
        [Section("FI.SEC_WIND", Localize = true)]

        [Name("FI.WIND_RESIST", Localize = true)]
        [Description("FI.DESC_WIND_RESIST", Localize = true)]
        [Slider(0, 100)]
        public int windResistancePercent = 50;

        // ═══════════════════════════════════════════
        //   РОЗЖИГ
        // ═══════════════════════════════════════════
        [Section("FI.SEC_FIRESTART", Localize = true)]

        [Name("FI.START_BONUS", Localize = true)]
        [Description("FI.DESC_START_BONUS", Localize = true)]
        [Slider(0, 50)]
        public int fireStartBonusPercent = 10;

        // ═══════════════════════════════════════════
        //   ТЕПЛО ОТ ОГНЯ
        // ═══════════════════════════════════════════
        [Section("FI.SEC_WARMTH", Localize = true)]

        [Name("FI.INDOOR_WARMTH", Localize = true)]
        [Description("FI.DESC_INDOOR_WARMTH", Localize = true)]
        [Slider(0, 15)]
        public int indoorWarmthBonus = 3;

        [Name("FI.OUTDOOR_WARMTH", Localize = true)]
        [Description("FI.DESC_OUTDOOR_WARMTH", Localize = true)]
        [Slider(0, 10)]
        public int outdoorWarmthBonus = 2;

        // ───────────────────────────────────────────
        protected override void OnConfirm()
        {
            // Применяем настройки сразу после нажатия «Применить»
            Core.ApplyFireManagerWarmth();
            Core.ApplyMaxFireDuration();
        }

        public static void OnLoad()
        {
            instance = new Settings();
            instance.AddToModSettings("Fire Improvements");
        }
    }
}
