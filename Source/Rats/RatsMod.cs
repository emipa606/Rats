using System;
using System.Linq;
using Mlie;
using UnityEngine;
using Verse;

namespace Rats;

[StaticConstructorOnStartup]
internal class RatsMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static RatsMod instance;

    private static string currentVersion;
    private static readonly Vector2 searchSize = new Vector2(200f, 25f);
    private static readonly Vector2 iconSize = new Vector2(58f, 58f);
    private static string searchText = "";
    private static Vector2 scrollPosition;
    private static readonly Color alternateBackground = new Color(0.2f, 0.2f, 0.2f, 0.5f);


    /// <summary>
    ///     The private settings
    /// </summary>
    public readonly RatsModSettings Settings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public RatsMod(ModContentPack content)
        : base(content)
    {
        instance = this;
        Settings = GetSettings<RatsModSettings>();

        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "Rats!";
    }

    /// <summary>
    ///     The settings-window
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        base.DoSettingsWindowContents(rect);

        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);
        listing_Standard.ColumnWidth = rect.width * 0.95f / 2f;
        listing_Standard.Gap();
        Settings.MaxRats = (int)Widgets.HorizontalSlider_NewTemp(listing_Standard.GetRect(20),
            Settings.MaxRats, 1, 20, false,
            "Rats.maxrats.label".Translate(Settings.MaxRats), null, null, 1f);
        listing_Standard.Gap();
        Settings.MaxPerDay = Math.Max(Settings.MaxRats, (int)Widgets.HorizontalSlider_NewTemp(
            listing_Standard.GetRect(20),
            Settings.MaxPerDay, Settings.MaxRats, 50, false,
            "Rats.maxperday.label".Translate(Settings.MaxPerDay), null, null, 1f));
        listing_Standard.Gap();
        Settings.MaxTotalRats = (int)Widgets.HorizontalSlider_NewTemp(listing_Standard.GetRect(20),
            Settings.MaxTotalRats, 0, 100, false,
            "Rats.maxtotalrats.label".Translate(Settings.MaxTotalRats), null, null, 1f);
        listing_Standard.Gap();
        Settings.PercentScaria = Widgets.HorizontalSlider_NewTemp(listing_Standard.GetRect(20),
            Settings.PercentScaria, 0, 1f, false,
            "Rats.percentscaria.label".Translate(Settings.PercentScaria * 100), null, null, 0.01f);
        listing_Standard.Gap();
        Settings.MinDays = Widgets.HorizontalSlider_NewTemp(listing_Standard.GetRect(20),
            Settings.MinDays, 0, 30, false,
            "Rats.mindays.label".Translate(Settings.MinDays), null, null, 0.1f);
        listing_Standard.Gap();

        var lastRect = listing_Standard.GetRect(20);
        Settings.RotDays = (int)Widgets.HorizontalSlider_NewTemp(lastRect,
            Settings.RotDays, 1, 30, false,
            "Rats.rotdays.label".Translate(Settings.RotDays), null, null, 1f);
        listing_Standard.NewColumn();
        listing_Standard.CheckboxLabeled("Rats.dessicated.label".Translate(), ref Settings.Dessicated,
            "Rats.dessicated.tooltip".Translate());
        listing_Standard.CheckboxLabeled("Rats.biome.label".Translate(), ref Settings.Biome,
            "Rats.biome.tooltip".Translate());
        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("Rats.showmessages.label".Translate(), ref Settings.ShowMessages,
            "Rats.showmessages.tooltip".Translate());
        listing_Standard.CheckboxLabeled("Rats.logging.label".Translate(), ref Settings.VerboseLogging,
            "Rats.logging.tooltip".Translate());
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("Rats.version.label".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        searchText =
            Widgets.TextField(
                new Rect(
                    lastRect.position +
                    new Vector2(rect.width - searchSize.x - (iconSize.x * 3), 0),
                    searchSize),
                searchText);
        TooltipHandler.TipRegion(new Rect(
            lastRect.position + new Vector2(rect.width - searchSize.x - (iconSize.x * 3), 0),
            searchSize), "Rats.search".Translate());
        Text.Font = GameFont.Tiny;
        Widgets.Label(new Rect(
            lastRect.position +
            new Vector2(rect.width - (iconSize.x * 2.4f), lastRect.height / 2f),
            searchSize), "Rats.spawn.label".Translate());
        Widgets.Label(new Rect(
            lastRect.position +
            new Vector2(rect.width - (iconSize.x * 1.4f), lastRect.height / 2f),
            searchSize), "Rats.inside.label".Translate());

        Text.Font = GameFont.Small;
        listing_Standard.End();


        var allAnimals = Rats.AllAnimals;
        if (!string.IsNullOrEmpty(searchText))
        {
            allAnimals = Rats.AllAnimals.Where(def =>
                    def.label.ToLower().Contains(searchText.ToLower()) || def.modContentPack?.Name.ToLower()
                        .Contains(searchText.ToLower()) == true)
                .ToList();
        }

        var borderRect = rect;
        borderRect.y += lastRect.y + 30;
        borderRect.height -= lastRect.y + 30;
        var scrollContentRect = rect;
        scrollContentRect.height = allAnimals.Count * 61f;
        scrollContentRect.width -= 20;
        scrollContentRect.x = 0;
        scrollContentRect.y = 0;


        var scrollListing = new Listing_Standard();
        Widgets.BeginScrollView(borderRect, ref scrollPosition, scrollContentRect);
        scrollListing.Begin(scrollContentRect);
        var alternate = false;
        foreach (var animal in allAnimals)
        {
            var modInfo = animal.modContentPack?.Name;
            var rowRect = scrollListing.GetRect(60);
            alternate = !alternate;
            if (alternate)
            {
                Widgets.DrawBoxSolid(rowRect.ExpandedBy(10, 0), alternateBackground);
            }

            var raceLabel = $"{animal.label.CapitalizeFirst()} ({animal.defName}) - {modInfo}";
            DrawIcon(animal,
                new Rect(rowRect.position, iconSize));
            var nameRect = new Rect(rowRect.position + new Vector2(iconSize.x, 0),
                rowRect.size - new Vector2(iconSize.x * 2, 0));
            var spawnAtAll = instance.Settings.ManualRats.Contains(animal.defName);
            var inside = instance.Settings.SpawnInside.Contains(animal.defName);
            var wasSpawnAtAll = spawnAtAll;
            var wasInside = inside;
            Widgets.Label(nameRect, raceLabel);
            Widgets.Checkbox(rowRect.position + new Vector2(rowRect.width, 0) - new Vector2(iconSize.x * 2, 0),
                ref spawnAtAll);
            Widgets.Checkbox(rowRect.position + new Vector2(rowRect.width, 0) - new Vector2(iconSize.x, 0), ref inside);
            if (spawnAtAll != wasSpawnAtAll)
            {
                if (spawnAtAll)
                {
                    instance.Settings.ManualRats.Add(animal.defName);
                }
                else
                {
                    instance.Settings.ManualRats.Remove(animal.defName);
                }
            }

            if (inside == wasInside)
            {
                continue;
            }

            if (inside)
            {
                instance.Settings.SpawnInside.Add(animal.defName);
            }
            else
            {
                instance.Settings.SpawnInside.Remove(animal.defName);
            }
        }

        scrollListing.End();
        Widgets.EndScrollView();
    }

    private void DrawIcon(ThingDef animal, Rect rect)
    {
        var pawnKind = DefDatabase<PawnKindDef>.GetNamedSilentFail(animal.defName);

        var texture2D = pawnKind?.lifeStages?.Last()?.bodyGraphicData?.Graphic?.MatSingle?.mainTexture;

        if (texture2D == null)
        {
            return;
        }

        var toolTip = $"{pawnKind.LabelCap}\n{pawnKind.race?.description}";
        if (texture2D.width != texture2D.height)
        {
            var ratio = (float)texture2D.width / texture2D.height;

            if (ratio < 1)
            {
                rect.x += (rect.width - (rect.width * ratio)) / 2;
                rect.width *= ratio;
            }
            else
            {
                rect.y += (rect.height - (rect.height / ratio)) / 2;
                rect.height /= ratio;
            }
        }

        GUI.DrawTexture(rect, texture2D);
        TooltipHandler.TipRegion(rect, toolTip);
    }

    public override void WriteSettings()
    {
        base.WriteSettings();
        Rats.UpdateAvailableRats();
    }
}