using Il2Cpp;
using HarmonyLib;

namespace FireImprovementsMod.Patches
{
    // Skill-Adjustment compatibility:
    //   Skill-Adjustment patches SkillsManager.Awake and edits m_BaseSuccessChance[] and
    //   m_StartPercentIncrease[] for each Firestarting skill level.
    //   Our Postfix adds a flat bonus AFTER CalculateFireStartSuccess has already run
    //   with those modified values, so both mods' effects are fully applied.
    //   Order: Skill-Adjustment configures the skill → vanilla calculation uses those values
    //          → our Postfix adds our flat bonus on top.
    //   Result: bonuses stack additively — intended behaviour.
    //
    // [HarmonyAfter] is not needed: Skill-Adjustment does not patch CalculateFireStartSuccess,
    // so there is no execution-order conflict.
    /// <summary>
    /// Adds a flat percentage bonus to the fire-start success chance.
    ///
    /// How it works:
    ///   FireManager.CalculateFireStartSuccess() returns a chance in the range 0–100
    ///   (whole percentages, NOT 0.0–1.0). Our Postfix adds the configured bonus and
    ///   clamps the result to 100.
    ///
    ///   Example: base 65% + bonus 10% = 75%.
    ///
    /// Original method parameters:
    ///   FireStarterItem fireStarterItem  — lighter / matches / firesteel
    ///   FuelSourceItem  fuelItem         — fuel being used
    ///   FireStarterItem tinderItem       — tinder (may be null)
    /// </summary>
    [HarmonyPatch(typeof(FireManager), nameof(FireManager.CalculateFireStartSuccess))]
    internal class FireStartSuccessPatch
    {
        static void Postfix(ref float __result)
        {
            int bonus = Settings.instance.fireStartBonusPercent;
            if (bonus <= 0)
                return;

            // The method returns 0–100, not 0.0–1.0, so add the bonus directly.
            __result = System.Math.Min(__result + bonus, 100f);
        }
    }
}
