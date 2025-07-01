using System.Collections.Generic;
using Verse;

namespace Rats;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class RatsModSettings : ModSettings
{
    public bool Biome;
    public bool Desiccated = true;
    public List<string> ManualRats = [];
    public int MaxPerDay = 5;
    public int MaxRats = 3;
    public int MaxTotalRats;
    public float MinDays = 1f;
    public float PercentHungry = 1f;
    public float PercentScaria;
    public float PercentSterile;
    public int RotDays = 5;
    public bool ShowMessages = true;
    public List<string> SpawnCorpseOnly = [];
    public List<string> SpawnFoodOnly = [];
    public List<string> SpawnInside = [];
    public bool VerboseLogging;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref VerboseLogging, "VerboseLogging");
        Scribe_Values.Look(ref Biome, "Biome");
        Scribe_Values.Look(ref ShowMessages, "ShowMessages", true);
        Scribe_Values.Look(ref Desiccated, "Dessicated", true);
        Scribe_Values.Look(ref MaxRats, "MaxRats", 3);
        Scribe_Values.Look(ref MaxPerDay, "MaxPerDay", 5);
        Scribe_Values.Look(ref RotDays, "RotDays", 5);
        Scribe_Values.Look(ref MinDays, "MinDays", 1f);
        Scribe_Values.Look(ref MaxTotalRats, "MaxTotalRats");
        Scribe_Values.Look(ref PercentScaria, "PercentScaria");
        Scribe_Values.Look(ref PercentSterile, "PercentSterile");
        Scribe_Values.Look(ref PercentHungry, "PercentHungry", 1f);
        Scribe_Collections.Look(ref ManualRats, "ManualRats");
        Scribe_Collections.Look(ref SpawnInside, "SpawnInside");
        Scribe_Collections.Look(ref SpawnFoodOnly, "SpawnFoodOnly");
        Scribe_Collections.Look(ref SpawnCorpseOnly, "SpawnCorpseOnly");
    }
}