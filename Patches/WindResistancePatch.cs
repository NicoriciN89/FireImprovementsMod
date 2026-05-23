using Il2Cpp;
using HarmonyLib;
using UnityEngine;

namespace FireImprovementsMod.Patches
{
    /// <summary>
    /// Снижает вероятность того, что ветер потушит огонь.
    ///
    /// В текущей версии игры FireShouldBlowOutFromWind() —
    /// метод экземпляра класса Fire (не статический GameManager).
    ///
    /// Prefix перехватывает вызов и бросает кубик:
    ///   если выпало &lt; windResistancePercent — возвращаем false (огонь устоял).
    ///   При 50% — костёр гаснет вдвое реже.
    ///   При 100% — ветер не гасит огонь никогда.
    /// </summary>
    [HarmonyPatch(typeof(Fire), nameof(Fire.FireShouldBlowOutFromWind))]
    internal class WindResistancePatch
    {
        static bool Prefix(ref bool __result)
        {
            int resistance = Settings.instance.windResistancePercent;
            if (resistance <= 0)
                return true;  // нет защиты — выполнить оригинальный метод

            // Бросаем кубик: если удача — огонь устоял
            if (Random.value * 100f < resistance)
            {
                __result = false;
                return false;   // пропустить оригинальный метод
            }

            return true;  // не повезло — выполнить оригинальный метод
        }
    }
}
