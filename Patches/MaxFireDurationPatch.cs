// MaxFireDurationPatch.cs
// The getter patch approach (get_m_MaxDurationHoursOfFire) does not work in Il2Cpp
// because field-accessor methods cannot be patched via HarmonyLib, and native code
// bypasses managed getters entirely.
//
// The working approach is in Core.ApplyMaxFireDuration():
//   fm.m_MaxDurationHoursOfFire = maxHours;
// This calls the Il2Cpp property setter which writes directly to native memory.
// The setter is called in OnSceneWasInitialized for every playable scene.
