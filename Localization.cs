using Il2Cpp;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace FireImprovementsMod
{
    /// <summary>
    /// Локализация для ключей FI.* используемых в Settings.
    /// Перехватывает Il2Cpp.Localization.Get() и возвращает
    /// перевод на язык игры, с откатом на английский.
    /// Поддерживаемые языки: English, French, German, Spanish, Brazilian,
    /// Polish, Czech, Russian, Turkish, Italian, Dutch,
    /// Japanese, Korean, ChineseSimplified, ChineseTraditional.
    /// </summary>
    [HarmonyPatch(typeof(Localization), nameof(Localization.Get))]
    internal static class LocalizationPatch
    {
        // Используем __0 (первый аргумент по индексу) вместо именованного key,
        // чтобы обойти возможную разницу имён параметров в Il2Cpp-обёртке.
        static void Postfix(string __0, ref string __result)
        {
            if (__0 == null || !__0.StartsWith("FI."))
                return;

            string lang = Localization.Language ?? "English";
            if (s_translations.TryGetValue(lang, out var dict) && dict.TryGetValue(__0, out string val))
            {
                __result = val;
                return;
            }
            // Fallback to English
            if (s_translations.TryGetValue("English", out var en) && en.TryGetValue(__0, out string enVal))
                __result = enVal;
        }

        // ── master table ───────────────────────────────────────────────
        internal static readonly Dictionary<string, Dictionary<string, string>> s_translations = new()
        {
            // ── English ────────────────────────────────────────────────
            ["English"] = new()
            {
                // Sections
                ["FI.SEC_DURATION"]  = "Fuel Burn Duration",
                ["FI.SEC_WIND"]      = "Wind Resistance",
                ["FI.SEC_FIRESTART"] = "Fire Starting",
                ["FI.SEC_WARMTH"]    = "Fire Warmth",
                // Settings
                ["FI.DURATION_MULT"]  = "Burn Duration Multiplier",
                ["FI.MAX_DURATION"]   = "Max Fire Duration (hours)",
                ["FI.WIND_RESIST"]    = "Wind Blowout Resistance (%)",
                ["FI.START_BONUS"]    = "Fire Start Bonus (%)",
                ["FI.INDOOR_WARMTH"]  = "Indoor Warmth Bonus (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Outdoor Warmth Bonus (\u00b0C)",
                // Descriptions
                ["FI.DESC_DURATION_MULT"]  = "Fuel burn time multiplier.\n1.0 = no change | 1.5 = 50% longer | 2.0 = twice as long.\nAffects all fuel: wood, coal, accelerant.",
                ["FI.DESC_MAX_DURATION"]   = "Maximum possible fire burn time in hours.\n12 = vanilla (no change)\n24 = one day | 72 = three days | 120 = five days | 200 = eight days.",
                ["FI.DESC_WIND_RESIST"]    = "Chance (%) that wind does NOT blow out the fire.\n0 = no protection (vanilla)\n50 = fire goes out half as often\n100 = wind never extinguishes fire.",
                ["FI.DESC_START_BONUS"]    = "Flat bonus added to fire start success chance.\nAdded after all skill, tool and weather calculations.\n0 = no change | 15 = +15% to success.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Extra warmth from fire indoors (stoves, fireplaces).\n0 = no change | 5 = +5\u00b0C to minimum air temperature from fire.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Extra warmth from fire outdoors (campfires).\n0 = no change | 3 = +3\u00b0C to minimum air temperature from fire.",
            },

            // ── Russian ────────────────────────────────────────────────
            ["Russian"] = new()
            {
                // Разделы
                ["FI.SEC_DURATION"]  = "\u0414\u043b\u0438\u0442\u0435\u043b\u044c\u043d\u043e\u0441\u0442\u044c \u0433\u043e\u0440\u0435\u043d\u0438\u044f",
                ["FI.SEC_WIND"]      = "\u0417\u0430\u0449\u0438\u0442\u0430 \u043e\u0442 \u0432\u0435\u0442\u0440\u0430",
                ["FI.SEC_FIRESTART"] = "\u0420\u043e\u0437\u0436\u0438\u0433",
                ["FI.SEC_WARMTH"]    = "\u0422\u0435\u043f\u043b\u043e \u043e\u0442 \u043e\u0433\u043d\u044f",
                // Настройки
                ["FI.DURATION_MULT"]  = "\u041c\u043d\u043e\u0436\u0438\u0442\u0435\u043b\u044c \u0434\u043b\u0438\u0442\u0435\u043b\u044c\u043d\u043e\u0441\u0442\u0438 \u0433\u043e\u0440\u0435\u043d\u0438\u044f",
                ["FI.MAX_DURATION"]   = "\u041c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0435 \u0432\u0440\u0435\u043c\u044f \u0433\u043e\u0440\u0435\u043d\u0438\u044f (\u0447\u0430\u0441\u043e\u0432)",
                ["FI.WIND_RESIST"]    = "\u0417\u0430\u0449\u0438\u0442\u0430 \u043e\u0442 \u0437\u0430\u0434\u0443\u0432\u0430\u043d\u0438\u044f \u0432\u0435\u0442\u0440\u043e\u043c (%)",
                ["FI.START_BONUS"]    = "\u0411\u043e\u043d\u0443\u0441 \u043a \u0440\u043e\u0437\u0436\u0438\u0433\u0443 (%)",
                ["FI.INDOOR_WARMTH"]  = "\u0411\u043e\u043d\u0443\u0441 \u0442\u0435\u043f\u043b\u0430 \u0432 \u043f\u043e\u043c\u0435\u0449\u0435\u043d\u0438\u0438 (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "\u0411\u043e\u043d\u0443\u0441 \u0442\u0435\u043f\u043b\u0430 \u043d\u0430 \u0443\u043b\u0438\u0446\u0435 (\u00b0C)",
                // Описания
                ["FI.DESC_DURATION_MULT"]  = "\u041c\u043d\u043e\u0436\u0438\u0442\u0435\u043b\u044c \u0432\u0440\u0435\u043c\u0435\u043d\u0438 \u0433\u043e\u0440\u0435\u043d\u0438\u044f \u0442\u043e\u043f\u043b\u0438\u0432\u0430.\n1.0 = \u0431\u0435\u0437 \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u0439 | 1.5 = \u043d\u0430 50% \u0434\u043e\u043b\u044c\u0448\u0435 | 2.0 = \u0432\u0434\u0432\u043e\u0435 \u0434\u043e\u043b\u044c\u0448\u0435.\n\u0412\u043b\u0438\u044f\u0435\u0442 \u043d\u0430 \u0434\u0440\u043e\u0432\u0430, \u0443\u0433\u043e\u043b\u044c, \u0443\u0441\u043a\u043e\u0440\u0438\u0442\u0435\u043b\u044c \u2014 \u0432\u0441\u0451 \u0442\u043e\u043f\u043b\u0438\u0432\u043e.",
                ["FI.DESC_MAX_DURATION"]   = "\u041c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u043e \u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e\u0435 \u0432\u0440\u0435\u043c\u044f \u0433\u043e\u0440\u0435\u043d\u0438\u044f \u043e\u0433\u043d\u044f \u0432 \u0447\u0430\u0441\u0430\u0445.\n12 = \u0432\u0430\u043d\u0438\u043b\u044c\u043d\u043e\u0435 (\u0431\u0435\u0437 \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u0439)\n24 = \u0441\u0443\u0442\u043a\u0438 | 72 = \u0442\u0440\u043e\u0435 \u0441\u0443\u0442\u043e\u043a | 120 = \u043f\u044f\u0442\u044c \u0441\u0443\u0442\u043e\u043a | 200 = \u0432\u043e\u0441\u0435\u043c\u044c \u0441 \u043b\u0438\u0448\u043d\u0438\u043c.",
                ["FI.DESC_WIND_RESIST"]    = "\u0428\u0430\u043d\u0441 (%) \u0442\u043e\u0433\u043e, \u0447\u0442\u043e \u0432\u0435\u0442\u0435\u0440 \u041d\u0415 \u043f\u043e\u0442\u0443\u0448\u0438\u0442 \u043e\u0433\u043e\u043d\u044c.\n0 = \u0431\u0435\u0437 \u0437\u0430\u0449\u0438\u0442\u044b (\u0432\u0430\u043d\u0438\u043b\u044c\u043d\u043e)\n50 = \u043e\u0433\u043e\u043d\u044c \u0433\u0430\u0441\u043d\u0435\u0442 \u0432\u0434\u0432\u043e\u0435 \u0440\u0435\u0436\u0435\n100 = \u0432\u0435\u0442\u0435\u0440 \u043d\u0435 \u0442\u0443\u0448\u0438\u0442 \u043e\u0433\u043e\u043d\u044c \u043d\u0438\u043a\u043e\u0433\u0434\u0430.",
                ["FI.DESC_START_BONUS"]    = "\u0414\u043e\u043f\u043e\u043b\u043d\u0438\u0442\u0435\u043b\u044c\u043d\u044b\u0439 % \u043a \u0448\u0430\u043d\u0441\u0443 \u0440\u043e\u0437\u0436\u0438\u0433\u0430.\n\u041f\u0440\u0438\u0431\u0430\u0432\u043b\u044f\u0435\u0442\u0441\u044f \u043f\u043e\u0441\u043b\u0435 \u0432\u0441\u0435\u0445 \u0440\u0430\u0441\u0447\u0451\u0442\u043e\u0432 \u043d\u0430\u0432\u044b\u043a\u0430, \u0438\u043d\u0441\u0442\u0440\u0443\u043c\u0435\u043d\u0442\u043e\u0432 \u0438 \u043f\u043e\u0433\u043e\u0434\u044b.\n0 = \u0431\u0435\u0437 \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u0439 | 15 = +15% \u043a \u0443\u0441\u043f\u0435\u0445\u0443.",
                ["FI.DESC_INDOOR_WARMTH"]  = "\u0414\u043e\u043f\u043e\u043b\u043d\u0438\u0442\u0435\u043b\u044c\u043d\u044b\u0435 \u0433\u0440\u0430\u0434\u0443\u0441\u044b \u0442\u0435\u043f\u043b\u0430 \u043e\u0442 \u043e\u0433\u043d\u044f \u0432 \u043f\u043e\u043c\u0435\u0449\u0435\u043d\u0438\u0438 (\u043f\u0435\u0447\u043a\u0438, \u043a\u0430\u043c\u0438\u043d\u044b).\n0 = \u0431\u0435\u0437 \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u0439 | 5 = +5\u00b0C \u043a \u043c\u0438\u043d\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0439 \u0442\u0435\u043c\u043f\u0435\u0440\u0430\u0442\u0443\u0440\u0435 \u043e\u0442 \u043e\u0433\u043d\u044f.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "\u0414\u043e\u043f\u043e\u043b\u043d\u0438\u0442\u0435\u043b\u044c\u043d\u044b\u0435 \u0433\u0440\u0430\u0434\u0443\u0441\u044b \u0442\u0435\u043f\u043b\u0430 \u043e\u0442 \u043e\u0433\u043d\u044f \u043d\u0430 \u043e\u0442\u043a\u0440\u044b\u0442\u043e\u043c \u0432\u043e\u0437\u0434\u0443\u0445\u0435 (\u043a\u043e\u0441\u0442\u0440\u044b).\n0 = \u0431\u0435\u0437 \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u0439 | 3 = +3\u00b0C \u043a \u043c\u0438\u043d\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0439 \u0442\u0435\u043c\u043f\u0435\u0440\u0430\u0442\u0443\u0440\u0435 \u043e\u0442 \u043e\u0433\u043d\u044f.",
            },

            // ── French ─────────────────────────────────────────────────
            ["French"] = new()
            {
                ["FI.SEC_DURATION"]  = "Dur\u00e9e de Combustion",
                ["FI.SEC_WIND"]      = "R\u00e9sistance au Vent",
                ["FI.SEC_FIRESTART"] = "Allumage du Feu",
                ["FI.SEC_WARMTH"]    = "Chaleur du Feu",
                ["FI.DURATION_MULT"]  = "Multiplicateur de Dur\u00e9e",
                ["FI.MAX_DURATION"]   = "Dur\u00e9e Max du Feu (heures)",
                ["FI.WIND_RESIST"]    = "R\u00e9sistance \u00e0 l\u2019extinction par le vent (%)",
                ["FI.START_BONUS"]    = "Bonus d\u2019Allumage (%)",
                ["FI.INDOOR_WARMTH"]  = "Bonus de Chaleur Int\u00e9rieure (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Bonus de Chaleur Ext\u00e9rieure (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Multiplicateur du temps de combustion du combustible.\n1.0 = inchang\u00e9 | 1.5 = 50% plus long | 2.0 = deux fois plus long.\nAffecte bois, charbon et acc\u00e9l\u00e9rant.",
                ["FI.DESC_MAX_DURATION"]   = "Dur\u00e9e maximale de combustion en heures.\n12 = valeur d\u2019origine (sans changement)\n24 = un jour | 72 = trois jours | 120 = cinq jours | 200 = huit jours.",
                ["FI.DESC_WIND_RESIST"]    = "Chance (%) que le vent N\u2019\u00e9teigne PAS le feu.\n0 = sans protection\n50 = feu \u00e9teint deux fois moins souvent\n100 = le vent n\u2019\u00e9teint jamais le feu.",
                ["FI.DESC_START_BONUS"]    = "Bonus fixe \u00e0 la chance d\u2019allumage.\nAjout\u00e9 apr\u00e8s tous les calculs de comp\u00e9tence, d\u2019outil et de m\u00e9t\u00e9o.\n0 = inchang\u00e9 | 15 = +15% de succ\u00e8s.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Chaleur suppl\u00e9mentaire du feu en int\u00e9rieur (po\u00eales, chemin\u00e9es).\n0 = inchang\u00e9 | 5 = +5\u00b0C \u00e0 la temp\u00e9rature minimale du feu.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Chaleur suppl\u00e9mentaire du feu en ext\u00e9rieur (feux de camp).\n0 = inchang\u00e9 | 3 = +3\u00b0C \u00e0 la temp\u00e9rature minimale du feu.",
            },

            // ── German ─────────────────────────────────────────────────
            ["German"] = new()
            {
                ["FI.SEC_DURATION"]  = "Brenndauer",
                ["FI.SEC_WIND"]      = "Windschutz",
                ["FI.SEC_FIRESTART"] = "Feuer Anz\u00fcnden",
                ["FI.SEC_WARMTH"]    = "Feuerw\u00e4rme",
                ["FI.DURATION_MULT"]  = "Brenndauer-Multiplikator",
                ["FI.MAX_DURATION"]   = "Max. Brenndauer (Stunden)",
                ["FI.WIND_RESIST"]    = "Wind-L\u00f6sch-Schutz (%)",
                ["FI.START_BONUS"]    = "Feuer-Anz\u00fcnd-Bonus (%)",
                ["FI.INDOOR_WARMTH"]  = "Innenw\u00e4rme-Bonus (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Au\u00dfen w\u00e4rme-Bonus (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Multiplikator f\u00fcr die Brennzeit des Brennstoffs.\n1.0 = unver\u00e4ndert | 1.5 = 50% l\u00e4nger | 2.0 = doppelt so lang.\nGilt f\u00fcr Holz, Kohle und Brandbeschleuniger.",
                ["FI.DESC_MAX_DURATION"]   = "Maximale Brenndauer des Feuers in Stunden.\n12 = Originalwert (unver\u00e4ndert)\n24 = ein Tag | 72 = drei Tage | 120 = f\u00fcnf Tage | 200 = acht Tage.",
                ["FI.DESC_WIND_RESIST"]    = "Chance (%), dass der Wind das Feuer NICHT l\u00f6scht.\n0 = kein Schutz\n50 = Feuer erlischt halb so oft\n100 = Wind l\u00f6scht das Feuer nie.",
                ["FI.DESC_START_BONUS"]    = "Fester Bonus auf die Erfolgswahrscheinlichkeit beim Feuer anz\u00fcnden.\nNach allen Berechnungen von Fertigkeit, Werkzeug und Wetter.\n0 = unver\u00e4ndert | 15 = +15% Erfolg.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Zus\u00e4tzliche W\u00e4rme vom Feuer in Innenr\u00e4umen (\u00d6fen, Kamine).\n0 = unver\u00e4ndert | 5 = +5\u00b0C zur minimalen Lufttemperatur.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Zus\u00e4tzliche W\u00e4rme vom Feuer im Freien (Lagerfeuer).\n0 = unver\u00e4ndert | 3 = +3\u00b0C zur minimalen Lufttemperatur.",
            },

            // ── Spanish ────────────────────────────────────────────────
            ["Spanish"] = new()
            {
                ["FI.SEC_DURATION"]  = "Duraci\u00f3n de la Combusti\u00f3n",
                ["FI.SEC_WIND"]      = "Resistencia al Viento",
                ["FI.SEC_FIRESTART"] = "Encender Fuego",
                ["FI.SEC_WARMTH"]    = "Calor del Fuego",
                ["FI.DURATION_MULT"]  = "Multiplicador de Duraci\u00f3n",
                ["FI.MAX_DURATION"]   = "Duraci\u00f3n M\u00e1x. del Fuego (horas)",
                ["FI.WIND_RESIST"]    = "Resistencia al Apagado por Viento (%)",
                ["FI.START_BONUS"]    = "Bonus de Encendido (%)",
                ["FI.INDOOR_WARMTH"]  = "Bonus de Calor Interior (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Bonus de Calor Exterior (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Multiplicador del tiempo de combusti\u00f3n del combustible.\n1.0 = sin cambio | 1.5 = 50% m\u00e1s largo | 2.0 = el doble.\nAfecta madera, carb\u00f3n y acelerante.",
                ["FI.DESC_MAX_DURATION"]   = "Tiempo m\u00e1ximo de combusti\u00f3n en horas.\n12 = valor original (sin cambio)\n24 = un d\u00eda | 72 = tres d\u00edas | 120 = cinco d\u00edas | 200 = ocho d\u00edas.",
                ["FI.DESC_WIND_RESIST"]    = "Probabilidad (%) de que el viento NO apague el fuego.\n0 = sin protecci\u00f3n\n50 = el fuego se apaga la mitad de veces\n100 = el viento nunca apaga el fuego.",
                ["FI.DESC_START_BONUS"]    = "Bonus fijo a la probabilidad de \u00e9xito al encender fuego.\nAplicado tras todos los c\u00e1lculos de habilidad, herramientas y clima.\n0 = sin cambio | 15 = +15% de \u00e9xito.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Calor extra del fuego en interiores (estufas, chimeneas).\n0 = sin cambio | 5 = +5\u00b0C a la temperatura m\u00ednima del fuego.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Calor extra del fuego en exteriores (hogueras).\n0 = sin cambio | 3 = +3\u00b0C a la temperatura m\u00ednima del fuego.",
            },

            // ── Brazilian (Portuguese-BR) ──────────────────────────────
            ["Brazilian"] = new()
            {
                ["FI.SEC_DURATION"]  = "Dura\u00e7\u00e3o da Combust\u00e3o",
                ["FI.SEC_WIND"]      = "Resist\u00eancia ao Vento",
                ["FI.SEC_FIRESTART"] = "Acender Fogo",
                ["FI.SEC_WARMTH"]    = "Calor do Fogo",
                ["FI.DURATION_MULT"]  = "Multiplicador de Dura\u00e7\u00e3o",
                ["FI.MAX_DURATION"]   = "Dura\u00e7\u00e3o M\u00e1x. do Fogo (horas)",
                ["FI.WIND_RESIST"]    = "Resist\u00eancia ao Apagamento pelo Vento (%)",
                ["FI.START_BONUS"]    = "B\u00f4nus de Acendimento (%)",
                ["FI.INDOOR_WARMTH"]  = "B\u00f4nus de Calor Interno (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "B\u00f4nus de Calor Externo (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Multiplicador do tempo de combust\u00e3o do combust\u00edvel.\n1.0 = sem mudan\u00e7a | 1.5 = 50% mais longo | 2.0 = duas vezes mais longo.\nAfeta madeira, carv\u00e3o e acelerante.",
                ["FI.DESC_MAX_DURATION"]   = "Tempo m\u00e1ximo de combust\u00e3o em horas.\n12 = valor original (sem mudan\u00e7a)\n24 = um dia | 72 = tr\u00eas dias | 120 = cinco dias | 200 = oito dias.",
                ["FI.DESC_WIND_RESIST"]    = "Chance (%) de que o vento N\u00c3O apague o fogo.\n0 = sem prote\u00e7\u00e3o\n50 = fogo apaga metade das vezes\n100 = o vento nunca apaga o fogo.",
                ["FI.DESC_START_BONUS"]    = "B\u00f4nus fixo \u00e0 chance de sucesso ao acender fogo.\nAplicado ap\u00f3s todos os c\u00e1lculos de habilidade, ferramentas e clima.\n0 = sem mudan\u00e7a | 15 = +15% de sucesso.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Calor extra do fogo em ambientes internos (fog\u00f5es, lareiras).\n0 = sem mudan\u00e7a | 5 = +5\u00b0C \u00e0 temperatura m\u00ednima do fogo.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Calor extra do fogo em ambientes externos (fogueiras).\n0 = sem mudan\u00e7a | 3 = +3\u00b0C \u00e0 temperatura m\u00ednima do fogo.",
            },

            // ── Polish ─────────────────────────────────────────────────
            ["Polish"] = new()
            {
                ["FI.SEC_DURATION"]  = "Czas Palenia",
                ["FI.SEC_WIND"]      = "Odporno\u015b\u0107 na Wiatr",
                ["FI.SEC_FIRESTART"] = "Rozpalanie Ognia",
                ["FI.SEC_WARMTH"]    = "Ciep\u0142o od Ognia",
                ["FI.DURATION_MULT"]  = "Mno\u017cnik Czasu Palenia",
                ["FI.MAX_DURATION"]   = "Maks. Czas Palenia (godziny)",
                ["FI.WIND_RESIST"]    = "Odporno\u015b\u0107 na Zg\u0105szenie przez Wiatr (%)",
                ["FI.START_BONUS"]    = "Bonus do Rozpalania (%)",
                ["FI.INDOOR_WARMTH"]  = "Bonus Ciep\u0142a w Pomieszczeniu (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Bonus Ciep\u0142a na Zewn\u0105trz (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Mno\u017cnik czasu spalania paliwa.\n1.0 = bez zmian | 1.5 = 50% d\u0142u\u017cej | 2.0 = dwa razy d\u0142u\u017cej.\nDotyczy drewna, w\u0119gla i przy\u015bpieszacza.",
                ["FI.DESC_MAX_DURATION"]   = "Maksymalny czas palenia ognia w godzinach.\n12 = warto\u015b\u0107 oryginalna (bez zmian)\n24 = jeden dzie\u0144 | 72 = trzy dni | 120 = pi\u0119\u0107 dni | 200 = osiem dni.",
                ["FI.DESC_WIND_RESIST"]    = "Szansa (%), \u017ce wiatr NIE zga\u015bnie ognia.\n0 = brak ochrony\n50 = ogie\u0144 ga\u015bnie dwa razy rzadziej\n100 = wiatr nigdy nie ga\u015bnie ognia.",
                ["FI.DESC_START_BONUS"]    = "Sta\u0142y bonus do szansy powodzenia rozpalania.\nDodawany po wszystkich obliczeniach umiej\u0119tno\u015bci, narz\u0119dzi i pogody.\n0 = bez zmian | 15 = +15% sukcesu.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Dodatkowe ciep\u0142o od ognia w pomieszczeniu (piece, kominki).\n0 = bez zmian | 5 = +5\u00b0C do minimalnej temperatury od ognia.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Dodatkowe ciep\u0142o od ognia na zewn\u0105trz (ogniska).\n0 = bez zmian | 3 = +3\u00b0C do minimalnej temperatury od ognia.",
            },

            // ── Czech ──────────────────────────────────────────────────
            ["Czech"] = new()
            {
                ["FI.SEC_DURATION"]  = "Doba Ho\u0159en\u00ed",
                ["FI.SEC_WIND"]      = "Odolnost V\u016f\u010di V\u011btru",
                ["FI.SEC_FIRESTART"] = "Zalo\u017een\u00ed Oh\u0148e",
                ["FI.SEC_WARMTH"]    = "Teplo z Oh\u0148e",
                ["FI.DURATION_MULT"]  = "N\u00e1sobitel Doby Ho\u0159en\u00ed",
                ["FI.MAX_DURATION"]   = "Max. Doba Ho\u0159en\u00ed (hodiny)",
                ["FI.WIND_RESIST"]    = "Odolnost V\u016f\u010di Uha\u0161en\u00ed V\u011btrem (%)",
                ["FI.START_BONUS"]    = "Bonus k Zalo\u017een\u00ed Oh\u0148e (%)",
                ["FI.INDOOR_WARMTH"]  = "Bonus Tepla Uvnit\u0159 (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Bonus Tepla Venku (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "N\u00e1sobitel doby ho\u0159en\u00ed paliva.\n1.0 = beze zm\u011bny | 1.5 = o 50% d\u00e9le | 2.0 = dvakr\u00e1t d\u00e9le.\nVztahuje se na d\u0159evo, uh\u00ed a urychlova\u010d.",
                ["FI.DESC_MAX_DURATION"]   = "Maxim\u00e1ln\u00ed doba ho\u0159en\u00ed oh\u0148e v hodin\u00e1ch.\n12 = p\u016fvodn\u00ed hodnota (beze zm\u011bny)\n24 = jeden den | 72 = t\u0159i dny | 120 = p\u011bt dn\u00ed | 200 = osm dn\u00ed.",
                ["FI.DESC_WIND_RESIST"]    = "\u0160ance (%), \u017ee v\u00edtr ohe\u0148 NEUHAS\u00cd.\n0 = \u017e\u00e1dn\u00e1 ochrana\n50 = ohe\u0148 hasne dvakr\u00e1t m\u00e9n\u011b\n100 = v\u00edtr nikdy neuhas\u00ed ohe\u0148.",
                ["FI.DESC_START_BONUS"]    = "Pevn\u00fd bonus k \u0161anci \u00fasp\u011bchu p\u0159i zalo\u017een\u00ed oh\u0148e.\nP\u0159id\u00e1n po v\u0161ech v\u00fdpo\u010dtech dovednost\u00ed, n\u00e1stroj\u016f a po\u010das\u00ed.\n0 = beze zm\u011bny | 15 = +15% k \u00fasp\u011bchu.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Extra teplo z oh\u0148e uvnit\u0159 (kamna, krby).\n0 = beze zm\u011bny | 5 = +5\u00b0C k minim\u00e1ln\u00ed teplot\u011b od oh\u0148e.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Extra teplo z oh\u0148e venku (t\u00e1bor\u00e1ky).\n0 = beze zm\u011bny | 3 = +3\u00b0C k minim\u00e1ln\u00ed teplot\u011b od oh\u0148e.",
            },

            // ── Turkish ────────────────────────────────────────────────
            ["Turkish"] = new()
            {
                ["FI.SEC_DURATION"]  = "Yanma S\u00fcresi",
                ["FI.SEC_WIND"]      = "R\u00fczgar Direnci",
                ["FI.SEC_FIRESTART"] = "Ate\u015f Yakma",
                ["FI.SEC_WARMTH"]    = "Ate\u015f Is\u0131s\u0131",
                ["FI.DURATION_MULT"]  = "Yanma S\u00fcresi \u00c7arpan\u0131",
                ["FI.MAX_DURATION"]   = "Maks. Yanma S\u00fcresi (saat)",
                ["FI.WIND_RESIST"]    = "R\u00fczgarla S\u00f6nd\u00fcrme Direnci (%)",
                ["FI.START_BONUS"]    = "Ate\u015f Yakma Bonusu (%)",
                ["FI.INDOOR_WARMTH"]  = "\u0130\u00e7 Mekan Is\u0131 Bonusu (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "D\u0131\u015f Mekan Is\u0131 Bonusu (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Yak\u0131t yanma s\u00fcresi \u00e7arpan\u0131.\n1.0 = de\u011fi\u015fmez | 1.5 = %50 daha uzun | 2.0 = iki kat uzun.\nOdun, k\u00f6m\u00fcr ve h\u0131zland\u0131r\u0131c\u0131 dahil t\u00fcm yak\u0131tlar\u0131 etkiler.",
                ["FI.DESC_MAX_DURATION"]   = "Ate\u015fin maksimum yanma s\u00fcresi (saat).\n12 = varsay\u0131lan (de\u011fi\u015fmez)\n24 = bir g\u00fcn | 72 = \u00fc\u00e7 g\u00fcn | 120 = be\u015f g\u00fcn | 200 = sekiz g\u00fcn.",
                ["FI.DESC_WIND_RESIST"]    = "R\u00fczgar\u0131n ate\u015fi S\u00d6ND\u00dcRMEME \u015fans\u0131 (%).\n0 = koruma yok\n50 = ate\u015f yar\u0131 s\u0131kl\u0131kta s\u00f6ner\n100 = r\u00fczgar asla ate\u015fi s\u00f6nd\u00fcrmez.",
                ["FI.DESC_START_BONUS"]    = "Ate\u015f yakma ba\u015far\u0131 \u015fans\u0131na sabit bonus.\nBeceri, alet ve hava hesaplamalar\u0131ndan sonra uygulan\u0131r.\n0 = de\u011fi\u015fmez | 15 = +%15 ba\u015far\u0131.",
                ["FI.DESC_INDOOR_WARMTH"]  = "\u0130\u00e7 mekanda (soba, \u015f\u00f6mine) ate\u015ften ekstra \u0131s\u0131.\n0 = de\u011fi\u015fmez | 5 = ate\u015ften minimum hava s\u0131cakl\u0131\u011f\u0131na +5\u00b0C.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "D\u0131\u015f mekanda (kamp ate\u015fi) ate\u015ften ekstra \u0131s\u0131.\n0 = de\u011fi\u015fmez | 3 = ate\u015ften minimum hava s\u0131cakl\u0131\u011f\u0131na +3\u00b0C.",
            },

            // ── Italian ────────────────────────────────────────────────
            ["Italian"] = new()
            {
                ["FI.SEC_DURATION"]  = "Durata della Combustione",
                ["FI.SEC_WIND"]      = "Resistenza al Vento",
                ["FI.SEC_FIRESTART"] = "Accensione del Fuoco",
                ["FI.SEC_WARMTH"]    = "Calore del Fuoco",
                ["FI.DURATION_MULT"]  = "Moltiplicatore della Durata",
                ["FI.MAX_DURATION"]   = "Durata Max del Fuoco (ore)",
                ["FI.WIND_RESIST"]    = "Resistenza allo Spegnimento dal Vento (%)",
                ["FI.START_BONUS"]    = "Bonus di Accensione (%)",
                ["FI.INDOOR_WARMTH"]  = "Bonus Calore Interno (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Bonus Calore Esterno (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Moltiplicatore del tempo di combustione del combustibile.\n1.0 = invariato | 1.5 = 50% pi\u00f9 lungo | 2.0 = il doppio.\nInfluisce su legna, carbone e accelerante.",
                ["FI.DESC_MAX_DURATION"]   = "Durata massima di combustione in ore.\n12 = valore originale (invariato)\n24 = un giorno | 72 = tre giorni | 120 = cinque giorni | 200 = otto giorni.",
                ["FI.DESC_WIND_RESIST"]    = "Probabilit\u00e0 (%) che il vento NON spenga il fuoco.\n0 = nessuna protezione\n50 = il fuoco si spegne la met\u00e0 delle volte\n100 = il vento non spegne mai il fuoco.",
                ["FI.DESC_START_BONUS"]    = "Bonus fisso alla probabilit\u00e0 di accendere il fuoco.\nAggiunto dopo tutti i calcoli di abilit\u00e0, strumenti e meteo.\n0 = invariato | 15 = +15% di successo.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Calore extra dal fuoco in ambienti chiusi (stufe, camini).\n0 = invariato | 5 = +5\u00b0C alla temperatura minima dal fuoco.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Calore extra dal fuoco all\u2019aperto (fal\u00f2).\n0 = invariato | 3 = +3\u00b0C alla temperatura minima dal fuoco.",
            },

            // ── Dutch ──────────────────────────────────────────────────
            ["Dutch"] = new()
            {
                ["FI.SEC_DURATION"]  = "Brandtijd",
                ["FI.SEC_WIND"]      = "Windweerstand",
                ["FI.SEC_FIRESTART"] = "Vuur Aansteken",
                ["FI.SEC_WARMTH"]    = "Vuurwarmte",
                ["FI.DURATION_MULT"]  = "Brandtijd-vermenigvuldiger",
                ["FI.MAX_DURATION"]   = "Max. Brandtijd (uren)",
                ["FI.WIND_RESIST"]    = "Weerstand tegen Uitblazen door Wind (%)",
                ["FI.START_BONUS"]    = "Vuur Aansteken Bonus (%)",
                ["FI.INDOOR_WARMTH"]  = "Binnense Warmtebonus (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "Buitense Warmtebonus (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "Vermenigvuldiger voor de brandtijd van brandstof.\n1.0 = ongewijzigd | 1.5 = 50% langer | 2.0 = twee keer zo lang.\nGeldt voor hout, kool en brandversneller.",
                ["FI.DESC_MAX_DURATION"]   = "Maximale brandtijd in uren.\n12 = oorspronkelijke waarde (ongewijzigd)\n24 = \u00e9\u00e9n dag | 72 = drie dagen | 120 = vijf dagen | 200 = acht dagen.",
                ["FI.DESC_WIND_RESIST"]    = "Kans (%) dat wind het vuur NIET uitblaast.\n0 = geen bescherming\n50 = vuur gaat half zo vaak uit\n100 = wind blaast het vuur nooit uit.",
                ["FI.DESC_START_BONUS"]    = "Vaste bonus op de slaagkans van vuur aansteken.\nNa alle vaardigheids-, gereedschaps- en weerberekeningen.\n0 = ongewijzigd | 15 = +15% op succes.",
                ["FI.DESC_INDOOR_WARMTH"]  = "Extra warmte van vuur binnenshuis (kachels, open haarden).\n0 = ongewijzigd | 5 = +5\u00b0C op de minimale luchttemperatuur van vuur.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "Extra warmte van vuur buitenshuis (kampvuren).\n0 = ongewijzigd | 3 = +3\u00b0C op de minimale luchttemperatuur van vuur.",
            },

            // ── Japanese ───────────────────────────────────────────────
            ["Japanese"] = new()
            {
                ["FI.SEC_DURATION"]  = "\u71c3\u713c\u6642\u9593",
                ["FI.SEC_WIND"]      = "\u98a8\u3078\u306e\u8010\u6027",
                ["FI.SEC_FIRESTART"] = "\u706b\u8d77\u3053\u3057",
                ["FI.SEC_WARMTH"]    = "\u706b\u306e\u6696\u304b\u3055",
                ["FI.DURATION_MULT"]  = "\u71c3\u713c\u6642\u9593\u500d\u7387",
                ["FI.MAX_DURATION"]   = "\u6700\u5927\u71c3\u713c\u6642\u9593\uff08\u6642\u9593\uff09",
                ["FI.WIND_RESIST"]    = "\u98a8\u306b\u3088\u308b\u6d88\u706b\u8010\u6027\uff08%\uff09",
                ["FI.START_BONUS"]    = "\u706b\u8d77\u3053\u3057\u30dc\u30fc\u30ca\u30b9\uff08%\uff09",
                ["FI.INDOOR_WARMTH"]  = "\u5c4b\u5185\u6696\u304b\u3055\u30dc\u30fc\u30ca\u30b9\uff08\u00b0C\uff09",
                ["FI.OUTDOOR_WARMTH"] = "\u5c4b\u5916\u6696\u304b\u3055\u30dc\u30fc\u30ca\u30b9\uff08\u00b0C\uff09",
                ["FI.DESC_DURATION_MULT"]  = "\u71c3\u6599\u306e\u71c3\u713c\u6642\u9593\u500d\u7387\u3002\n1.0 = \u5909\u5316\u306a\u3057 | 1.5 = 50%\u9577\u304f | 2.0 = 2\u500d\u3002\n\u6728\u6750\u3001\u77f3\u70ad\u3001\u52a0\u901f\u5264\u3059\u3079\u3066\u306b\u9069\u7528\u3002",
                ["FI.DESC_MAX_DURATION"]   = "\u706b\u306e\u6700\u5927\u71c3\u713c\u6642\u9593\uff08\u6642\u9593\uff09\u3002\n12 = \u30c7\u30d5\u30a9\u30eb\u30c8\uff08\u5909\u5316\u306a\u3057\uff09\n24 = 1\u65e5 | 72 = 3\u65e5 | 120 = 5\u65e5 | 200 = 8\u65e5\u3002",
                ["FI.DESC_WIND_RESIST"]    = "\u98a8\u304c\u706b\u3092\u6d88\u3055\u306a\u3044\u78ba\u7387\uff08%\uff09\u3002\n0 = \u4fdd\u8b77\u306a\u3057\n50 = \u6d88\u706b\u983b\u5ea6\u304c\u534a\u5206\n100 = \u98a8\u3067\u7d76\u5bfe\u306b\u6d88\u3048\u306a\u3044\u3002",
                ["FI.DESC_START_BONUS"]    = "\u706b\u8d77\u3053\u3057\u6210\u529f\u7387\u3078\u306e\u56fa\u5b9a\u30dc\u30fc\u30ca\u30b9\u3002\n\u30b9\u30ad\u30eb\u30fb\u9053\u5177\u30fb\u5929\u5019\u306e\u8a08\u7b97\u5f8c\u306b\u52a0\u7b97\u3002\n0 = \u5909\u5316\u306a\u3057 | 15 = +15%\u6210\u529f\u7387\u3002",
                ["FI.DESC_INDOOR_WARMTH"]  = "\u5c4b\u5185\uff08\u30b9\u30c8\u30fc\u30d6\u30fb\u6691\u7089\uff09\u3067\u306e\u706b\u304b\u3089\u306e\u8ffd\u52a0\u6696\u304b\u3055\u3002\n0 = \u5909\u5316\u306a\u3057 | 5 = \u706b\u304b\u3089\u306e\u6700\u4f4e\u6c17\u6e29\u306b+5\u00b0C\u3002",
                ["FI.DESC_OUTDOOR_WARMTH"] = "\u5c4b\u5916\uff08\u305f\u304d\u7099\uff09\u3067\u306e\u706b\u304b\u3089\u306e\u8ffd\u52a0\u6696\u304b\u3055\u3002\n0 = \u5909\u5316\u306a\u3057 | 3 = \u706b\u304b\u3089\u306e\u6700\u4f4e\u6c17\u6e29\u306b+3\u00b0C\u3002",
            },

            // ── Korean ─────────────────────────────────────────────────
            ["Korean"] = new()
            {
                ["FI.SEC_DURATION"]  = "\uc5f0\uc18c \uc2dc\uac04",
                ["FI.SEC_WIND"]      = "\ubc14\ub78c \uc800\ud56d",
                ["FI.SEC_FIRESTART"] = "\ubd88 \ud53c\uc6b0\uae30",
                ["FI.SEC_WARMTH"]    = "\ubd88\uc758 \uc628\uae30",
                ["FI.DURATION_MULT"]  = "\uc5f0\uc18c \uc2dc\uac04 \ubc30\uc728",
                ["FI.MAX_DURATION"]   = "\ucd5c\ub300 \uc5f0\uc18c \uc2dc\uac04 (\uc2dc\uac04)",
                ["FI.WIND_RESIST"]    = "\ubc14\ub78c \uc18c\ud654 \uc800\ud56d (%)",
                ["FI.START_BONUS"]    = "\ubd88 \ud53c\uc6b0\uae30 \ubcf4\ub108\uc2a4 (%)",
                ["FI.INDOOR_WARMTH"]  = "\uc2e4\ub0b4 \uc628\uae30 \ubcf4\ub108\uc2a4 (\u00b0C)",
                ["FI.OUTDOOR_WARMTH"] = "\uc2e4\uc678 \uc628\uae30 \ubcf4\ub108\uc2a4 (\u00b0C)",
                ["FI.DESC_DURATION_MULT"]  = "\uc5f0\ub8cc \uc5f0\uc18c \uc2dc\uac04 \ubc30\uc728.\n1.0 = \ubcc0\ud654 \uc5c6\uc74c | 1.5 = 50% \uae38\uac8c | 2.0 = \ub450 \ubc30.\n\uc7a5\uc791, \uc11d\ud0ed, \uac00\uc18d\uc81c \ub4f1 \ubaa8\ub4e0 \uc5f0\ub8cc\uc5d0 \uc801\uc6a9.",
                ["FI.DESC_MAX_DURATION"]   = "\ubd88\uc758 \ucd5c\ub300 \uc5f0\uc18c \uc2dc\uac04(\uc2dc\uac04).\n12 = \uae30\ubcf8\uac12 (\ubcc0\ud654 \uc5c6\uc74c)\n24 = \ud558\ub8e8 | 72 = 3\uc77c | 120 = 5\uc77c | 200 = 8\uc77c.",
                ["FI.DESC_WIND_RESIST"]    = "\ubc14\ub78c\uc774 \ubd88\uc744 \ub044\uc9c0 \uc54a\uc744 \ud655\ub960(%).\n0 = \ubcf4\ud638 \uc5c6\uc74c\n50 = \uc18c\ud654 \ube48\ub3c4\uac00 \uc808\ubc18\uc73c\ub85c \uac10\uc18c\n100 = \ubc14\ub78c\uc774 \ubd88\uc744 \uc808\ub300 \ub044\uc9c0 \uc54a\uc74c.",
                ["FI.DESC_START_BONUS"]    = "\ubd88 \ud53c\uc6b0\uae30 \uc131\uacf5 \ud655\ub960\uc5d0 \ub354\ud574\uc9c0\ub294 \uace0\uc815 \ubcf4\ub108\uc2a4.\n\uc2a4\ud0ac, \ub3c4\uad6c, \ub0a0\uc528 \uacc4\uc0b0 \ud6c4 \ucd94\uac00.\n0 = \ubcc0\ud654 \uc5c6\uc74c | 15 = \uc131\uacf5\ub960 +15%.",
                ["FI.DESC_INDOOR_WARMTH"]  = "\uc2e4\ub0b4(\uc2a4\ud1a0\ube0c, \ube7c\ub09c\ub85c)(\uc2e4\ub0b4) \ubd88\uc758 \ucd94\uac00 \uc628\uae30.\n0 = \ubcc0\ud654 \uc5c6\uc74c | 5 = \ubd88\uc758 \ucd5c\uc800 \uae30\uc628 +5\u00b0C.",
                ["FI.DESC_OUTDOOR_WARMTH"] = "\uc2e4\uc678(\ubaa8\ub2e5\ubd88) \ubd88\uc758 \ucd94\uac00 \uc628\uae30.\n0 = \ubcc0\ud654 \uc5c6\uc74c | 3 = \ubd88\uc758 \ucd5c\uc800 \uae30\uc628 +3\u00b0C.",
            },

            // ── Chinese Simplified ─────────────────────────────────────
            ["ChineseSimplified"] = new()
            {
                ["FI.SEC_DURATION"]  = "\u71c3\u70e7\u65f6\u957f",
                ["FI.SEC_WIND"]      = "\u6297\u98ce\u80fd\u529b",
                ["FI.SEC_FIRESTART"] = "\u751f\u706b",
                ["FI.SEC_WARMTH"]    = "\u706b\u5806\u6e29\u6696\u5ea6",
                ["FI.DURATION_MULT"]  = "\u71c3\u70e7\u65f6\u957f\u500d\u7387",
                ["FI.MAX_DURATION"]   = "\u6700\u5927\u71c3\u70e7\u65f6\u957f\uff08\u5c0f\u65f6\uff09",
                ["FI.WIND_RESIST"]    = "\u6297\u98ce\u606f\u706b\u6982\u7387\uff08%\uff09",
                ["FI.START_BONUS"]    = "\u751f\u706b\u6210\u529f\u52a0\u6210\uff08%\uff09",
                ["FI.INDOOR_WARMTH"]  = "\u5ba4\u5185\u6e29\u6696\u52a0\u6210\uff08\u00b0C\uff09",
                ["FI.OUTDOOR_WARMTH"] = "\u5ba4\u5916\u6e29\u6696\u52a0\u6210\uff08\u00b0C\uff09",
                ["FI.DESC_DURATION_MULT"]  = "\u71c3\u6599\u71c3\u70e7\u65f6\u957f\u500d\u7387\u3002\n1.0 = \u4e0d\u53d8 | 1.5 = \u5ef6\u957f50% | 2.0 = \u5ef6\u957f\u4e00\u500d\u3002\n\u5f71\u54cd\u6240\u6709\u71c3\u6599\uff1a\u6728\u67f4\u3001\u7164\u70ad\u3001\u52a9\u71c3\u5242\u3002",
                ["FI.DESC_MAX_DURATION"]   = "\u706b\u5806\u6700\u5927\u71c3\u70e7\u65f6\u957f\uff08\u5c0f\u65f6\uff09\u3002\n12 = \u9ed8\u8ba4\u503c\uff08\u4e0d\u53d8\uff09\n24 = \u4e00\u5929 | 72 = \u4e09\u5929 | 120 = \u4e94\u5929 | 200 = \u516b\u5929\u3002",
                ["FI.DESC_WIND_RESIST"]    = "\u98ce\u4e0d\u5439\u7070\u706b\u7130\u7684\u6982\u7387\uff08%\uff09\u3002\n0 = \u65e0\u4fdd\u62a4\n50 = \u606f\u706b\u9891\u7387\u964d\u4f4e\u4e00\u534a\n100 = \u98ce\u6c38\u8fdc\u4e0d\u4f1a\u606f\u706b\u3002",
                ["FI.DESC_START_BONUS"]    = "\u751f\u706b\u6210\u529f\u7387\u7684\u56fa\u5b9a\u52a0\u6210\u3002\n\u5728\u6280\u80fd\u3001\u5de5\u5177\u548c\u5929\u6c14\u8ba1\u7b97\u540e\u53e0\u52a0\u3002\n0 = \u4e0d\u53d8 | 15 = \u6210\u529f\u7387+15%\u3002",
                ["FI.DESC_INDOOR_WARMTH"]  = "\u5ba4\u5185\uff08\u7089\u5177\u3001\u58c1\u7089\uff09\u706b\u5806\u7684\u989d\u5916\u6e29\u6696\u5ea6\u3002\n0 = \u4e0d\u53d8 | 5 = \u6700\u4f4e\u6c14\u6e29+5\u00b0C\u3002",
                ["FI.DESC_OUTDOOR_WARMTH"] = "\u5ba4\u5916\uff08\u7bf9\u706b\uff09\u706b\u5806\u7684\u989d\u5916\u6e29\u6696\u5ea6\u3002\n0 = \u4e0d\u53d8 | 3 = \u6700\u4f4e\u6c17\u6e29+3\u00b0C\u3002",
            },

            // ── Chinese Traditional ────────────────────────────────────
            ["ChineseTraditional"] = new()
            {
                ["FI.SEC_DURATION"]  = "\u71c3\u71d2\u6642\u9577",
                ["FI.SEC_WIND"]      = "\u6297\u98a8\u80fd\u529b",
                ["FI.SEC_FIRESTART"] = "\u751f\u706b",
                ["FI.SEC_WARMTH"]    = "\u706b\u5806\u6eab\u6696\u5ea6",
                ["FI.DURATION_MULT"]  = "\u71c3\u71d2\u6642\u9577\u500d\u7387",
                ["FI.MAX_DURATION"]   = "\u6700\u5927\u71c3\u71d2\u6642\u9577\uff08\u5c0f\u6642\uff09",
                ["FI.WIND_RESIST"]    = "\u6297\u98a8\u606f\u706b\u6a5f\u7387\uff08%\uff09",
                ["FI.START_BONUS"]    = "\u751f\u706b\u6210\u529f\u52a0\u6210\uff08%\uff09",
                ["FI.INDOOR_WARMTH"]  = "\u5ba4\u5167\u6eab\u6696\u52a0\u6210\uff08\u00b0C\uff09",
                ["FI.OUTDOOR_WARMTH"] = "\u5ba4\u5916\u6eab\u6696\u52a0\u6210\uff08\u00b0C\uff09",
                ["FI.DESC_DURATION_MULT"]  = "\u71c3\u6599\u71c3\u71d2\u6642\u9577\u500d\u7387\u3002\n1.0 = \u4e0d\u8b8a | 1.5 = \u5ef6\u957f50% | 2.0 = \u5ef6\u957f\u4e00\u500d\u3002\n\u5f71\u97ff\u6240\u6709\u71c3\u6599\uff1a\u6728\u67f4\u3001\u714e\u70ad\u3001\u52a9\u71c3\u5291\u3002",
                ["FI.DESC_MAX_DURATION"]   = "\u706b\u5806\u6700\u5927\u71c3\u71d2\u6642\u9577\uff08\u5c0f\u6642\uff09\u3002\n12 = \u9810\u8a2d\u5024\uff08\u4e0d\u8b8a\uff09\n24 = \u4e00\u5929 | 72 = \u4e09\u5929 | 120 = \u4e94\u5929 | 200 = \u516b\u5929\u3002",
                ["FI.DESC_WIND_RESIST"]    = "\u98a8\u4e0d\u5439\u7089\u706b\u7130\u7684\u6a5f\u7387\uff08%\uff09\u3002\n0 = \u7121\u4fdd\u8b77\n50 = \u606f\u706b\u983b\u7387\u964d\u4f4e\u4e00\u534a\n100 = \u98a8\u6c38\u9060\u4e0d\u6703\u606f\u706b\u3002",
                ["FI.DESC_START_BONUS"]    = "\u751f\u706b\u6210\u529f\u7387\u7684\u56fa\u5b9a\u52a0\u6210\u3002\n\u5728\u6280\u80fd\u3001\u5de5\u5177\u548c\u5929\u6c23\u8a08\u7b97\u5f8c\u758a\u52a0\u3002\n0 = \u4e0d\u8b8a | 15 = \u6210\u529f\u7387+15%\u3002",
                ["FI.DESC_INDOOR_WARMTH"]  = "\u5ba4\u5167\uff08\u7210\u5177\u3001\u58c1\u7210\uff09\u706b\u5806\u7684\u984d\u5916\u6eab\u6696\u5ea6\u3002\n0 = \u4e0d\u8b8a | 5 = \u6700\u4f4e\u6c23\u6eab+5\u00b0C\u3002",
                ["FI.DESC_OUTDOOR_WARMTH"] = "\u5ba4\u5916\uff08\u7bf9\u706b\uff09\u706b\u5806\u7684\u984d\u5916\u6eab\u6696\u5ea6\u3002\n0 = \u4e0d\u8b8a | 3 = \u6700\u4f4e\u6c23\u6eab+3\u00b0C\u3002",
            },
        };
    }

    /// <summary>
    /// Патч на DescriptionHolder.get_Text (ModSettings) —
    /// перехватывает чтение описания в момент показа, когда язык уже установлен.
    /// Работает для любого Panel — не привязан к конкретному экрану.
    /// </summary>
    [HarmonyPatch]
    internal static class DescriptionTextTranslatePatch
    {
        static MethodBase TargetMethod() =>
            AccessTools.PropertyGetter(
                AccessTools.TypeByName("ModSettings.DescriptionHolder"), "Text");

        static void Postfix(ref string __result)
        {
            if (__result == null || !__result.StartsWith("FI."))
                return;

            string lang = Localization.Language ?? "English";
            if (LocalizationPatch.s_translations.TryGetValue(lang, out var dict) && dict.TryGetValue(__result, out string val))
            {
                __result = val;
                return;
            }
            if (LocalizationPatch.s_translations.TryGetValue("English", out var en) && en.TryGetValue(__result, out string enVal))
                __result = enVal;
        }
    }
}
