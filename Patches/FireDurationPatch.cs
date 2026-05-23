using Il2Cpp;
using HarmonyLib;

namespace FireImprovementsMod.Patches
{
    /// <summary>
    /// Multiplies the burn duration of every fuel item (firewood, coal, accelerant, etc.).
    ///
    /// How it works:
    ///   Every GearItem that can be used as fuel has a FuelSourceItem component with the
    ///   field m_BurnDurationHours — how many in-game hours that item burns.
    ///   The patch fires once during each item instance's Awake() and multiplies the
    ///   vanilla value by the configured multiplier.
    ///
    ///   Examples at x1.5:
    ///     Firewood  1.0h → 1.5h
    ///     Coal      3.0h → 4.5h
    ///     Cedar log 2.0h → 3.0h
    /// </summary>
    [HarmonyPatch(typeof(GearItem), "Awake")]
    internal class FuelDurationPatch
    {
        static void Postfix(GearItem __instance)
        {
            float mult = Settings.instance.burnDurationMultiplier;
            if (mult <= 1.0f)
                return;

            // m_FuelSourceItem is null when the GearItem is not a fuel type
            var fuel = __instance.m_FuelSourceItem;
            if (fuel == null)
                return;

            float before = fuel.m_BurnDurationHours;
            fuel.m_BurnDurationHours *= mult;
            float after = fuel.m_BurnDurationHours;

            Core.Logger?.Msg($"[FuelDuration] {__instance.name}: {before:F2}h x{mult} = {after:F2}h");
        }
    }
}
