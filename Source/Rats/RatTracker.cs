﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Rats
{
    public class RatTracker : MapComponent
    {
        private static int SpawnedToday;
        private static int DaysPassed;

        public RatTracker(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

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

            if (GenDate.DaysPassed != DaysPassed)
            {
                DaysPassed = GenDate.DaysPassed;
                SpawnedToday = 0;
            }

            if (SpawnedToday >= RatsMod.instance.Settings.MaxPerDay)
            {
                return;
            }

            var validThings = GetRottenThings();
            if (!validThings.Any())
            {
                Rats.LogMessage("Could not find any rotting things on the map");
                return;
            }

            var item = validThings.RandomElementByWeight(WeightSelector);
            var ratDef = Rats.ValidRatRaces.RandomElement();
            var ratsToSpawn = Rand.RangeInclusive(1, RatsMod.instance.Settings.MaxRats);
            Rats.LogMessage($"Spawning {ratsToSpawn} rats at position of {item}");
            for (var i = 0; i < ratsToSpawn; i++)
            {
                var loc = CellFinder.RandomClosewalkCellNear(item.Position, map,
                    2);
                var spawnedRat = (Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(ratDef), loc, map);
                spawnedRat.needs.food.CurLevelPercentage = 1f;
                spawnedRat.jobs.TryTakeOrderedJob_NewTemp(new Job(JobDefOf.Ingest, item));
                SpawnedToday++;
            }

            if (!RatsMod.instance.Settings.ShowMessages || !map.areaManager.Home.ActiveCells.Contains(item.Position))
            {
                return;
            }

            var message = new Message("Rats.message".Translate(item.Label), MessageTypeDefOf.NeutralEvent,
                new LookTargets(item));
            Messages.Message(message);
        }

        public static Dictionary<Thing, CompRottable> rottableThings = new Dictionary<Thing, CompRottable>();
        private List<Thing> GetRottenThings()
        {
            // old version
            //var validThings = from rotting in map.listerThings.AllThings
            //                  where rotting.def.defName == "MeatRotten" &&
            //                        rotting.AmbientTemperature >= 10f
            //                        ||
            //                        rotting.TryGetComp<CompRottable>() != null &&
            //                        GenTemperature.RotRateAtTemperature(
            //                            Mathf.RoundToInt(rotting.TryGetComp<CompRottable>().parent.AmbientTemperature)) >= 0.999f &&
            //                        rotting.def.GetCompProperties<CompProperties_Rottable>().daysToRotStart <=
            //                        RatsMod.instance.Settings.RotDays &&
            //                        rotting.TryGetComp<CompRottable>().RotProgress > RatsMod.instance.Settings.MinDays * 60000 &&
            //                        (!rotting.def.IsCorpse || !rotting.ParentHolder.IsEnclosingContainer())
            //                  select rotting;

            // new version here
            var rottenThings = new List<Thing>();
            foreach (var thing in map.listerThings.AllThings)
            {
                if (thing.def != null && thing.def == Rats.MeatRotten && thing.AmbientTemperature >= 10f)
                {
                    rottenThings.Add(thing);
                }
                else
                {
                    if (!rottableThings.TryGetValue(thing, out var compRottable))
                    {
                        compRottable = thing.TryGetComp<CompRottable>();
                        rottableThings[thing] = compRottable;
                    }
                    if (compRottable != null && GenTemperature.RotRateAtTemperature(Mathf.RoundToInt(compRottable.parent.AmbientTemperature)) >= 0.999f
                        && thing.def.GetCompProperties<CompProperties_Rottable>().daysToRotStart <= RatsMod.instance.Settings.RotDays
                        && compRottable.RotProgress > RatsMod.instance.Settings.MinDays * 60000 && (!thing.def.IsCorpse || !thing.ParentHolder.IsEnclosingContainer()))
                    {
                        rottenThings.Add(thing);
                    }
                }
            }
            return rottenThings;
        }
        private float WeightSelector(Thing arg)
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
}