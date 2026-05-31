# Changelog — Fire Improvements Mod

## v1.2.5 — Bug fixes (2026-05-31)

### Bug fixes
- **Warmth bonuses not restored when set to 0** — if a player applied indoor/outdoor warmth
  bonuses and then changed both back to 0 in settings, the `ExperienceMode` component kept
  the old values until the next scene load. Now setting either bonus to 0 correctly writes
  the vanilla value back immediately.
- **Max fire duration not restored when set to 0** — same issue with `maxFireDurationHours`.
  Setting it to 0 ("don't change") after a prior non-zero value now properly restores the
  vanilla `FireManager.m_MaxDurationHoursOfFire` within the current session.
- **Wind resistance at 100% was not truly absolute** — `UnityEngine.Random.value` can return
  exactly `1.0`, making the internal roll equal to `100` which failed the `roll < 100` check,
  allowing a fire to be blown out even at the 100 % setting. Fixed with an explicit
  `resistance >= 100` short-circuit.

---

## v1.2.4 (2026-05-28)

### Bug fixes
- Settings were not persisted after game exit — `OnConfirm()` was missing the
  `base.OnConfirm()` call that triggers the save to disk.

---

## v1.2.3

### Bug fixes
- Fixed warmth sentinel bug: experience modes that use `-1E+38` as a sentinel for
  "fire warmth disabled" no longer produce extreme negative temperatures when a bonus
  is added. The sentinel is now detected and treated as `0` base.
- Removed verbose per-frame logging that caused minor performance degradation.

---

## v1.2.2

- Added value/calculation logging for easier debugging.
- Translated all source comments to English.

---

## v1.2.1

### Bug fixes
- `maxFireDurationHours` now applies correctly (was silently ignored in v1.2.0).

---

## v1.2.0

- Localisation system: all UI strings are now translated into all 15 game languages.
  The `localization.json` is embedded in the DLL; a user-override file is supported
  at `UserData/FireImprovementsMod/localization.json`.
- Added **Max Fire Duration** setting.
- Added compatibility notes for the Skill-Adjustment mod.

---

## v1.1.0

- Moved translations to external `localization.json`.
- Fixed fire-start probability always being 1 % too low due to an off-by-one in the
  vanilla return range assumption.

---

## v1.0.0 — Initial release

- Burn Duration Multiplier
- Wind Blowout Resistance
- Fire Start Bonus
- Indoor / Outdoor Warmth Bonuses
