using Il2Cpp;
using HarmonyLib;

namespace FireImprovementsMod.Patches
{
    // Совместимость со Skill-Adjustment:
    //   SkillAdj патчит SkillsManager.Awake — изменяет m_BaseSuccessChance[] и
    //   m_StartPercentIncrease[] для каждого уровня навыка Firestarting.
    //   Наш Postfix добавляет плоский бонус УЖЕ ПОСЛЕ того, как CalculateFireStartSuccess
    //   посчитал результат с учётом изменённых SkillAdj значений.
    //   Порядок: SkillAdj настраивает скилл → ванильный расчёт использует эти данные
    //            → наш Postfix добавляет flat бонус сверху.
    //   Эффект: bonuses stack additively — это желаемое поведение.
    /// <summary>
    /// Добавляет плоский бонус к шансу успешного розжига.
    ///
    /// Как работает:
    ///   FireManager.CalculateFireStartSuccess() возвращает шанс в диапазоне
    ///   0–100 (целые проценты). Наш Postfix прибавляет бонус и ограничивает
    ///   итог максимумом 100.
    ///
    ///   Пример: базовый шанс 65% + бонус 10% = 75%.
    ///
    /// Параметры оригинального метода:
    ///   FireStarterItem fireStarterItem   — зажигалка/спички/кресало
    ///   FuelSourceItem  fuelItem          — топливо
    ///   FireStarterItem tinderItem        — трут (может быть null)
    /// </summary>
    // [HarmonyAfter] не нужен: SkillAdj не патчит CalculateFireStartSuccess,
    // поэтому конфликта порядка нет.
    [HarmonyPatch(typeof(FireManager), nameof(FireManager.CalculateFireStartSuccess))]
    internal class FireStartSuccessPatch
    {
        static void Postfix(ref float __result)
        {
            int bonus = Settings.instance.fireStartBonusPercent;
            if (bonus <= 0)
                return;

            // Метод возвращает значение в диапазоне 0–100 (не 0.0–1.0),
            // поэтому прибавляем бонус напрямую и ограничиваем сотней.
            __result = System.Math.Min(__result + bonus, 100f);
        }
    }
}
