using Il2Cpp;
using HarmonyLib;
using UnityEngine;

namespace FireImprovementsMod.Patches
{
    /// <summary>
    /// Reduces the chance that wind extinguishes a fire.
    ///
    /// FireShouldBlowOutFromWind() is an instance method on Fire (not a static GameManager call).
    ///
    /// The Prefix intercepts the call and rolls a dice:
    ///   if roll &lt; windResistancePercent → return false (fire survives, original skipped).
    ///   At 50%  — fires blow out half as often.
    ///   At 100% — wind never extinguishes fires.
    /// </summary>
    [HarmonyPatch(typeof(Fire), nameof(Fire.FireShouldBlowOutFromWind))]
    internal class WindResistancePatch
    {
        static bool Prefix(ref bool __result)
        {
            int resistance = Settings.instance.windResistancePercent;
            if (resistance <= 0)
                return true;  // no protection configured — run original method

            float roll = Random.value * 100f;

            if (roll < resistance)
            {
                __result = false;
                return false;  // skip original method
            }

            return true;  // unlucky — let original method decide
        }
    }
}
