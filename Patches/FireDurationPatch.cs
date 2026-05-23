using Il2Cpp;
using HarmonyLib;

namespace FireImprovementsMod.Patches
{
    /// <summary>
    /// Увеличивает время горения всего топлива (дрова, уголь, ускоритель и пр.).
    ///
    /// Как работает:
    ///   Каждый GearItem с компонентом FuelSourceItem содержит поле m_BurnDurationHours —
    ///   сколько игровых часов этот предмет горит в огне.
    ///   Патч срабатывает один раз при инициализации каждого экземпляра предмета
    ///   и умножает значение на наш множитель.
    ///
    ///   Примеры с множителем 1.5x:
    ///     Firewood (1 ч)  → 1.5 ч
    ///     Coal (3 ч)      → 4.5 ч
    ///     Cedar log (2 ч) → 3.0 ч
    /// </summary>
    [HarmonyPatch(typeof(GearItem), "Awake")]
    internal class FuelDurationPatch
    {
        static void Postfix(GearItem __instance)
        {
            float mult = Settings.instance.burnDurationMultiplier;
            if (mult <= 1.0f)
                return;

            // GearItem.m_FuelSourceItem — ссылка на компонент топлива, null если предмет не топливо
            var fuel = __instance.m_FuelSourceItem;
            if (fuel == null)
                return;

            fuel.m_BurnDurationHours *= mult;
        }
    }
}
