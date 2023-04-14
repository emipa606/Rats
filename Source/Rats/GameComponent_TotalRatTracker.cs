using Verse;

namespace Rats;

public class GameComponent_TotalRatTracker : GameComponent
{
    public int ratsSpawned;

    public GameComponent_TotalRatTracker(Game game)
    {
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ratsSpawned, "ratsSpawned");
    }
}