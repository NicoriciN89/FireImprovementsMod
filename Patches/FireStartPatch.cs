using Il2Cpp;
using HarmonyLib;
using UnityEngine;

namespace FireImprovementsMod.Patches
{
    /// <summary>
    /// Добавляет плоский бонус к шансу успешного розжига.
    ///
    /// Как работает:
    ///   FireManager.CalculateFireStartSuccess() вычисляет вероятность [0..1]
    ///   успешного розжига, учитывая навык, инструменты, погоду и т.д.
    ///
    ///   Наш Postfix прибавляет к результату наш бонус (в долях, 10% = 0.10)
    ///   и зажимает итог в диапазоне [0..1].
    ///
    ///   Пример: базовый шанс 45% + бонус 10% = 55%.
    ///
    /// Параметры оригинального метода:
    ///   FireStarterItem fireStarterItem   — зажигалка/спички/кресало
    ///   FuelSourceItem  fuelItem          — топливо
    ///   FireStarterItem tinderItem        — трут (может быть null)
    /// </summary>
    [HarmonyPatch(typeof(FireManager), nameof(FireManager.CalculateFireStartSuccess))]
    internal class FireStartSuccessPatch
    {
        static void Postfix(ref float __result)
        {
            int bonus = Settings.instance.fireStartBonusPercent;
            if (bonus <= 0)
                return;

            __result = Mathf.Clamp01(__result + bonus / 100f);
        }
    }
}
