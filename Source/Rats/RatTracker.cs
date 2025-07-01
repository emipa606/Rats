using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Rats;

public class RatTracker(Map map) : MapComponent(map)
{
    private static readonly Dictionary<Thing, CompRottable> rottableThings = new();
    private BiomeDef currentBiome;
    private int daysPassed;
    private int spawnedToday;

    public override void MapComponentTick()
    {
        base.MapComponentTick();

        if (currentBiome == null)
        {
            if (map?.Biome == null)
            {
                return;
            }

            currentBiome = map.Biome;
        }

        if (Rats.ValidRatRaces == null || Rats.ValidRatRaces.Count == 0)
        {
            return;
        }

        if (GenTicks.TicksGame % (GenTicks.TickLongInterval * 2) != 0)
        {
            return;
        }

        if (Rand.Bool)
        {
            return;
        }

        if (GenDate.DaysPassed != daysPassed)
        {
            daysPassed = GenDate.DaysPassed;
            spawnedToday = 0;
        }

        if (spawnedToday >= RatsMod.instance.Settings.MaxPerDay)
        {
            Rats.LogMessage("Maximum rats already spawned per day");
            return;
        }

        if (RatsMod.instance.Settings.MaxTotalRats > 0)
        {
            if ((from pawn in map.mapPawns.AllPawnsSpawned
                    where Rats.ValidRatRaces.Contains(pawn.def.race.AnyPawnKind)
                    select pawn).Count() >= RatsMod.instance.Settings.MaxTotalRats)
            {
                Rats.LogMessage("Maximum rats already spawned on this map");
                return;
            }
        }

        var validThings = getRottenThings();
        if (!validThings.Any())
        {
            Rats.LogMessage("Could not find any rotting things on the map");
            return;
        }

        Rats.LogMessage($"Found the following rotting items: {string.Join(", ", validThings)}");

        var item = validThings.RandomElementByWeight(WeightSelector);
        var currentValidRats = Rats.ValidRatRaces;
        if (RatsMod.instance.Settings.Biome)
        {
            var allBiomeAnimals =
                DefDatabase<PawnKindDef>.AllDefs.Where(def => currentBiome.CommonalityOfAnimal(def) > 0);
            if (Find.WorldGrid[map.Tile].pollution > 0)
            {
                allBiomeAnimals = currentBiome.AllWildAnimals;
            }

            currentValidRats = currentValidRats.Intersect(allBiomeAnimals).ToList();
            if (!currentValidRats.Any())
            {
                Rats.LogMessage("No valid rats found for this biome");
                return;
            }
        }

        if (item.GetRoom()?.PsychologicallyOutdoors == false)
        {
            currentValidRats = currentValidRats.Intersect(Rats.InsideRatRaces).ToList();
            if (!currentValidRats.Any())
            {
                Rats.LogMessage($"No valid rats found to spawn inside at {item}");
                return;
            }
        }

        if (item is Corpse)
        {
            currentValidRats.RemoveAll(Rats.FoodRatRaces.Contains);
        }
        else
        {
            currentValidRats.RemoveAll(Rats.CorpseRatRaces.Contains);
        }

        if (!currentValidRats.Any())
        {
            Rats.LogMessage($"No valid rats found to spawn at {item}");
            return;
        }

        var ratDef = currentValidRats.RandomElement();
        var ratsToSpawn = Rand.RangeInclusive(1, RatsMod.instance.Settings.MaxRats);
        Rats.LogMessage($"Spawning {ratsToSpawn} rats at position of {item}");
        for (var i = 0; i < ratsToSpawn; i++)
        {
            var loc = CellFinder.RandomClosewalkCellNear(item.Position, map,
                2);
            var spawnedRat = (Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(ratDef), loc, map);
            if (Rand.Chance(RatsMod.instance.Settings.PercentScaria))
            {
                spawnedRat.health.AddHediff(HediffDefOf.Scaria);
            }

            if (Rand.Chance(RatsMod.instance.Settings.PercentSterile))
            {
                spawnedRat.health.AddHediff(HediffDefOf.Sterilized);
            }

            spawnedRat.needs.food.CurLevelPercentage = 1f - RatsMod.instance.Settings.PercentHungry;
            if (RatsMod.instance.Settings.PercentHungry > 0.5f)
            {
                spawnedRat.jobs.TryTakeOrderedJob(new Job(JobDefOf.Ingest, item));
            }

            spawnedToday++;
            Current.Game.GetComponent<GameComponent_TotalRatTracker>().ratsSpawned++;
        }

        if (!RatsMod.instance.Settings.ShowMessages || !map.areaManager.Home.ActiveCells.Contains(item.Position))
        {
            return;
        }

        var message = new Message(
            "Rats.message_new".Translate(ratsToSpawn, ratDef.label, item.Label),
            MessageTypeDefOf.NeutralEvent, new LookTargets(item));
        Messages.Message(message);
    }

    private List<Thing> getRottenThings()
    {
        var rottenThings = new List<Thing>();
        foreach (var thing in map.listerThings.AllThings)
        {
            if (thing.def == null)
            {
                continue;
            }

            if (thing.def != null && thing.def == Rats.MeatRotten && thing.AmbientTemperature >= 10f)
            {
                rottenThings.Add(thing);
                continue;
            }

            if (!rottableThings.TryGetValue(thing, out var compRottable))
            {
                compRottable = thing.TryGetComp<CompRottable>();
                rottableThings[thing] = compRottable;
            }

            if (compRottable == null)
            {
                //Rats.LogMessage($"{thing} is not rottable");
                continue;
            }

            if (GenTemperature.RotRateAtTemperature(Mathf.RoundToInt(compRottable.parent.AmbientTemperature)) < 0.999f)
            {
                Rats.LogMessage($"{thing} has too low temp");
                continue;
            }

            if (compRottable.PropsRot.daysToRotStart > RatsMod.instance.Settings.RotDays)
            {
                Rats.LogMessage($"{thing} rots in too many days: {compRottable.PropsRot.daysToRotStart}");
                continue;
            }

            if (compRottable.RotProgress <= RatsMod.instance.Settings.MinDays * GenDate.TicksPerDay)
            {
                Rats.LogMessage($"{thing} has too small rot progress: {compRottable.RotProgress}");
                continue;
            }

            if (thing.def.IsCorpse && thing.ParentHolder.IsEnclosingContainer())
            {
                Rats.LogMessage($"{thing} is a corpse in a grave");
                continue;
            }

            if (!RatsMod.instance.Settings.Desiccated && thing is Corpse corpse && corpse.IsDessicated())
            {
                Rats.LogMessage($"{thing} is a dessicated corpse and not allowed to spawn from");
                continue;
            }

            rottenThings.Add(thing);
        }

        return rottenThings;
    }

    private static float WeightSelector(Thing arg)
    {
        if (arg.def != null && arg.def == Rats.MeatRotten)
        {
            return 10;
        }

        if (!rottableThings.TryGetValue(arg, out var compRottable))
        {
            compRottable = arg.TryGetComp<CompRottable>();
            rottableThings[arg] = compRottable;
        }

        if (compRottable == null)
        {
            return 0;
        }

        return compRottable.RotProgress;
    }
}