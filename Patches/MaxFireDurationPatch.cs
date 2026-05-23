using Il2Cpp;
using HarmonyLib;

namespace FireImprovementsMod.Patches
{
    /// <summary>
    /// Устанавливает максимальную длительность горения костра.
    ///
    /// Как работает:
    ///   FireManager хранит поле m_MaxDurationHoursOfFire — верхняя граница,
    ///   до которой игра разрешает набивать костёр топливом.
    ///   Ванильное значение ~12 ч. Наш Postfix на геттере этого свойства
    ///   подменяет возвращаемое значение настройкой игрока (0–200 ч).
    ///
    ///   Установка 0 означает «не ограничивать» — используется значение
    ///   не менее ванильного (чтобы не сломать логику игры).
    /// </summary>
    [HarmonyPatch(typeof(FireManager), "get_m_MaxDurationHoursOfFire")]
    internal class MaxFireDurationPatch
    {
        static void Postfix(ref float __result)
        {
            int maxHours = Settings.instance.maxFireDurationHours;
            if (maxHours <= 0)
                return; // 0 = не менять ванильное значение

            __result = (float)maxHours;
        }
    }
}
