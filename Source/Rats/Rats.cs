﻿using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Rats;

[StaticConstructorOnStartup]
public class Rats
{
    public static List<PawnKindDef> ValidRatRaces;
    public static List<PawnKindDef> InsideRatRaces;
    public static List<PawnKindDef> CorpseRatRaces;
    public static List<PawnKindDef> FoodRatRaces;
    public static readonly ThingDef MeatRotten;
    public static readonly List<ThingDef> AllAnimals;

    static Rats()
    {
        AllAnimals = DefDatabase<ThingDef>.AllDefsListForReading
            .Where(def => def.race is { Animal: true } && !def.IsCorpse)
            .OrderBy(def => def.label).ToList();
        MeatRotten = DefDatabase<ThingDef>.GetNamedSilentFail("MeatRotten");
        ValidRatRaces = [];
        RatsMod.instance.Settings.ManualRats ??= [];

        RatsMod.instance.Settings.SpawnInside ??= [];

        RatsMod.instance.Settings.SpawnCorpseOnly ??= [];

        RatsMod.instance.Settings.SpawnFoodOnly ??= [];

        UpdateAvailableRats();
    }

    public static void UpdateAvailableRats()
    {
        InsideRatRaces = [];
        if (RatsMod.instance.Settings.SpawnInside.Any())
        {
            RatsMod.instance.Settings.SpawnInside.ForEach(s =>
                InsideRatRaces.Add(DefDatabase<PawnKindDef>.GetNamedSilentFail(s)));
        }

        CorpseRatRaces = [];
        if (RatsMod.instance.Settings.SpawnCorpseOnly.Any())
        {
            RatsMod.instance.Settings.SpawnCorpseOnly.ForEach(s =>
                CorpseRatRaces.Add(DefDatabase<PawnKindDef>.GetNamedSilentFail(s)));
        }

        FoodRatRaces = [];
        if (RatsMod.instance.Settings.SpawnFoodOnly.Any())
        {
            RatsMod.instance.Settings.SpawnFoodOnly.ForEach(s =>
                FoodRatRaces.Add(DefDatabase<PawnKindDef>.GetNamedSilentFail(s)));
        }

        if (RatsMod.instance.Settings.ManualRats?.Any() == true)
        {
            ValidRatRaces = [];
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
            RatsMod.instance.Settings.ManualRats ??= [];

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