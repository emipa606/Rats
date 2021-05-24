using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Rats
{
    [StaticConstructorOnStartup]
    public class Rats
    {
        public static readonly List<PawnKindDef> ValidRatRaces = new List<PawnKindDef>();

        static Rats()
        {
            //var harmony = new Harmony("Mlie.Rats");
            //harmony.PatchAll(Assembly.GetExecutingAssembly());
            updateAvailableRats();
        }

        private static void updateAvailableRats()
        {
            var ratDefs = new List<string>
            {
                "Rat",
                "FancyRat",
                "SnowRat"
            };
            ValidRatRaces.AddRange(from race in DefDatabase<PawnKindDef>.AllDefsListForReading
                where ratDefs.Contains(race.defName)
                select race);
            if (ValidRatRaces.Count == 0)
            {
                LogMessage("Could not find any valid rat-races in game", false, true);
            }
            else
            {
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
}