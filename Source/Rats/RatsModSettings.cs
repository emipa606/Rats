using Verse;

namespace Rats
{
    /// <summary>
    ///     Definition of the settings for the mod
    /// </summary>
    internal class RatsModSettings : ModSettings
    {
        public int MaxPerDay = 5;
        public int MaxRats = 3;
        public int MaxTotalRats;
        public float MinDays = 1f;
        public int RotDays = 5;
        public bool ShowMessages = true;
        public bool VerboseLogging;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref VerboseLogging, "VerboseLogging");
            Scribe_Values.Look(ref ShowMessages, "ShowMessages", true);
            Scribe_Values.Look(ref MaxRats, "MaxRats", 3);
            Scribe_Values.Look(ref MaxPerDay, "MaxPerDay", 5);
            Scribe_Values.Look(ref RotDays, "RotDays", 5);
            Scribe_Values.Look(ref MinDays, "MinDays", 1f);
            Scribe_Values.Look(ref MaxTotalRats, "MaxTotalRats");
        }
    }
}