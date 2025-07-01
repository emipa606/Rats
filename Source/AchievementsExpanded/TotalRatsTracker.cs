using System;
using AchievementsExpanded;
using Verse;

namespace Rats;

public class TotalRatsTracker : TrackerBase
{
    private int count = 1;

    [Unsaved] private int triggeredCount; //Only for display

    public TotalRatsTracker()
    {
    }

    public TotalRatsTracker(TotalRatsTracker reference) : base(reference)
    {
        count = reference.count;
    }

    public override string Key => "TotalRatsCurrentTracker";

    public override Func<bool> AttachToLongTick => Trigger;

    protected override string[] DebugText => [$"Count: {count}"];

    public override (float percent, string text) PercentComplete => count > 1
        ? ((float)triggeredCount / count,
            $"{(float)triggeredCount} / {count}")
        : base.PercentComplete;


    public override bool UnlockOnStartup => Trigger();


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref count, "count", 1);
    }

    public override bool Trigger()
    {
        base.Trigger();
        triggeredCount = Current.Game.GetComponent<GameComponent_TotalRatTracker>().ratsSpawned;

        return triggeredCount >= count;
    }
}