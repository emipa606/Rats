using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Rats;

[StaticConstructorOnStartup]
public class Rats
{
    public static List<PawnKindDef> ValidRatRaces = new List<PawnKindDef>();
    public static readonly ThingDef MeatRotten;
    public static readonly List<ThingDef> AllAnimals;

    static Rats()
    {
        AllAnimals = DefDatabase<ThingDef>.AllDefsListForReading
            .Where(def => def.race is { Animal: true })
            .OrderBy(def => def.label).ToList();
        MeatRotten = DefDatabase<ThingDef>.GetNamedSilentFail("MeatRotten");
        ValidRatRaces = new List<PawnKindDef>();
        if (RatsMod.instance.Settings.ManualRats == null)
        {
            RatsMod.instance.Settings.ManualRats = new List<string>();
        }

        UpdateAvailableRats();
    }

    public static void UpdateAvailableRats()
    {
        if (RatsMod.instance.Settings.ManualRats?.Any() == true)
        {
            ValidRatRaces = new List<PawnKindDef>();
            foreach (var settingsManualRat in RatsMod.instance.Settings.ManualRats)
            {
                ValidRatRaces.Add(PawnKindDef.Named(settingsManualRat));
            }

            if (ValidRatRaces.Count == 0)
            {
                LogMessage("Could not find any valid rat-races in game", false, true);
            }
            else
            {
                LogMessage($"Found {ValidRatRaces.Count} valid rat-races in game", true);
                LogMessage(string.Join(", ", ValidRatRaces));
            }

            return;
        }

        ValidRatRaces.AddRange(from race in DefDatabase<PawnKindDef>.AllDefsListForReading
            where race.HasModExtension<RatExtension>() &&
                  race.GetModExtension<RatExtension>().IsRat
            select race);
        if (ValidRatRaces.Count == 0)
        {
            LogMessage("Could not find any valid rat-races in game", false, true);
        }
        else
        {
            if (RatsMod.instance.Settings.ManualRats == null)
            {
                RatsMod.instance.Settings.ManualRats = new List<string>();
            }

            foreach (var validRatRace in ValidRatRaces)
            {
                RatsMod.instance.Settings.ManualRats?.Add(validRatRace.defName);
            }

            LogMessage($"Found {ValidRatRaces.Count} valid rat-races in game", true);
            LogMessage(string.Join(", ", ValidRatRaces));
        }
    }

    public static void LogMessage(string message, bool forced = false, bool warning = false)
    {
        if (warning)
        {
            Log.Warning($"[Rats]: {message}");
            return;
        }

        if (!forced && !RatsMod.instance.Settings.VerboseLogging)
        {
            return;
        }

        Log.Message($"[Rats!]: {message}");
    }
}