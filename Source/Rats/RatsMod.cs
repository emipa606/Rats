﻿using System;
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
    private static readonly Vector2 searchSize = new(200f, 25f);
    private static readonly Vector2 iconSize = new(58f, 58f);
    private static string searchText = "";
    private static Vector2 scrollPosition;
    private static readonly Color alternateBackground = new(0.2f, 0.2f, 0.2f, 0.5f);


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

        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.ColumnWidth = rect.width * 0.95f / 2f;
        Settings.MaxRats = (int)Widgets.HorizontalSlider(listingStandard.GetRect(25),
            Settings.MaxRats, 1, 20, false,
            "Rats.maxrats.label".Translate(Settings.MaxRats), null, null, 1f);
        listingStandard.Gap();
        Settings.MaxPerDay = Math.Max(Settings.MaxRats, (int)Widgets.HorizontalSlider(
            listingStandard.GetRect(20),
            Settings.MaxPerDay, Settings.MaxRats, 50, false,
            "Rats.maxperday.label".Translate(Settings.MaxPerDay), null, null, 1f));
        listingStandard.Gap();
        Settings.MaxTotalRats = (int)Widgets.HorizontalSlider(listingStandard.GetRect(20),
            Settings.MaxTotalRats, 0, 100, false,
            "Rats.maxtotalrats.label".Translate(Settings.MaxTotalRats), null, null, 1f);
        listingStandard.Gap();
        Settings.PercentScaria = Widgets.HorizontalSlider(listingStandard.GetRect(20),
            Settings.PercentScaria, 0, 1f, false,
            "Rats.percentscaria.label".Translate(Settings.PercentScaria * 100), null, null, 0.01f);
        listingStandard.Gap();
        Settings.PercentSterile = Widgets.HorizontalSlider(listingStandard.GetRect(20),
            Settings.PercentSterile, 0, 1f, false,
            "Rats.percentsterile.label".Translate(Settings.PercentSterile * 100), null, null, 0.01f);
        listingStandard.Gap();
        Settings.PercentHungry = Widgets.HorizontalSlider(listingStandard.GetRect(20),
            Settings.PercentHungry, 0, 1f, false,
            "Rats.percenthungry.label".Translate(Settings.PercentHungry * 100), null, null, 0.01f);
        listingStandard.Gap();
        Settings.MinDays = Widgets.HorizontalSlider(listingStandard.GetRect(20),
            Settings.MinDays, 0, 30, false,
            "Rats.mindays.label".Translate(Settings.MinDays), null, null, 0.1f);
        listingStandard.Gap();
        var lastRect = listingStandard.GetRect(20);
        Settings.RotDays = (int)Widgets.HorizontalSlider(lastRect,
            Settings.RotDays, 1, 30, false,
            "Rats.rotdays.label".Translate(Settings.RotDays), null, null, 1f);
        listingStandard.NewColumn();
        listingStandard.CheckboxLabeled("Rats.dessicated.label".Translate(), ref Settings.Desiccated,
            "Rats.dessicated.tooltip".Translate());
        listingStandard.CheckboxLabeled("Rats.biome.label".Translate(), ref Settings.Biome,
            "Rats.biome.tooltip".Translate());
        listingStandard.CheckboxLabeled("Rats.showmessages.label".Translate(), ref Settings.ShowMessages,
            "Rats.showmessages.tooltip".Translate());
        listingStandard.CheckboxLabeled("Rats.logging.label".Translate(), ref Settings.VerboseLogging,
            "Rats.logging.tooltip".Translate());
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("Rats.version.label".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        searchText =
            Widgets.TextField(
                new Rect(
                    lastRect.position +
                    new Vector2(rect.width - searchSize.x - (iconSize.x * 3), -25f),
                    searchSize),
                searchText);
        TooltipHandler.TipRegion(new Rect(
            lastRect.position + new Vector2(rect.width - searchSize.x - (iconSize.x * 3), -25f),
            searchSize), "Rats.search".Translate());
        Text.Font = GameFont.Tiny;
        Widgets.Label(new Rect(
            lastRect.position +
            new Vector2(rect.width - (iconSize.x * 4.4f), lastRect.height / 6f),
            searchSize), "Rats.spawn.label".Translate());
        Widgets.Label(new Rect(
            lastRect.position +
            new Vector2(rect.width - (iconSize.x * 3.4f), lastRect.height / 1.5f),
            searchSize), "Rats.inside.label".Translate());
        Widgets.Label(new Rect(
            lastRect.position +
            new Vector2(rect.width - (iconSize.x * 2.7f), lastRect.height / 6f),
            searchSize), "Rats.foodonly.label".Translate());
        Widgets.Label(new Rect(
            lastRect.position +
            new Vector2(rect.width - (iconSize.x * 1.4f), lastRect.height / 1.5f),
            searchSize), "Rats.corpseonly.label".Translate());

        Text.Font = GameFont.Small;
        listingStandard.End();


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
            drawIcon(animal,
                new Rect(rowRect.position, iconSize));
            var nameRect = new Rect(rowRect.position + new Vector2(iconSize.x, 0),
                rowRect.size - new Vector2(iconSize.x * 2, 0));
            var spawnAtAll = instance.Settings.ManualRats.Contains(animal.defName);
            var inside = instance.Settings.SpawnInside.Contains(animal.defName);
            var corpseOnly = instance.Settings.SpawnCorpseOnly.Contains(animal.defName);
            var foodOnly = instance.Settings.SpawnFoodOnly.Contains(animal.defName);
            var wasSpawnAtAll = spawnAtAll;
            var wasInside = inside;
            var wasCorpseOnly = corpseOnly;
            var wasFoodOnly = foodOnly;
            Widgets.Label(nameRect, raceLabel);
            Widgets.Checkbox(rowRect.position + new Vector2(rowRect.width, 0) - new Vector2(iconSize.x * 4, 0),
                ref spawnAtAll);
            Widgets.Checkbox(rowRect.position + new Vector2(rowRect.width, 0) - new Vector2(iconSize.x * 3, 0),
                ref inside, 24F, !spawnAtAll);
            Widgets.Checkbox(rowRect.position + new Vector2(rowRect.width, 0) - new Vector2(iconSize.x * 2, 0),
                ref corpseOnly, 24F, !spawnAtAll);
            Widgets.Checkbox(rowRect.position + new Vector2(rowRect.width, 0) - new Vector2(iconSize.x, 0),
                ref foodOnly, 24F, !spawnAtAll);

            if (spawnAtAll != wasSpawnAtAll)
            {
                if (spawnAtAll)
                {
                    instance.Settings.ManualRats.Add(animal.defName);
                }
                else
                {
                    instance.Settings.ManualRats.Remove(animal.defName);
                    inside = false;
                    corpseOnly = false;
                    foodOnly = false;
                }
            }

            if (inside != wasInside)
            {
                if (inside)
                {
                    instance.Settings.SpawnInside.Add(animal.defName);
                }
                else
                {
                    instance.Settings.SpawnInside.Remove(animal.defName);
                }
            }

            if (corpseOnly != wasCorpseOnly)
            {
                if (corpseOnly)
                {
                    instance.Settings.SpawnCorpseOnly.Add(animal.defName);
                    if (instance.Settings.SpawnFoodOnly.Contains(animal.defName))
                    {
                        instance.Settings.SpawnFoodOnly.Remove(animal.defName);
                    }
                }
                else
                {
                    if (instance.Settings.SpawnCorpseOnly.Contains(animal.defName))
                    {
                        instance.Settings.SpawnCorpseOnly.Remove(animal.defName);
                    }
                }
            }

            if (foodOnly == wasFoodOnly)
            {
                continue;
            }

            if (foodOnly)
            {
                instance.Settings.SpawnFoodOnly.Add(animal.defName);
                if (instance.Settings.SpawnCorpseOnly.Contains(animal.defName))
                {
                    instance.Settings.SpawnCorpseOnly.Remove(animal.defName);
                }
            }
            else
            {
                if (instance.Settings.SpawnFoodOnly.Contains(animal.defName))
                {
                    instance.Settings.SpawnFoodOnly.Remove(animal.defName);
                }
            }
        }

        scrollListing.End();
        Widgets.EndScrollView();
    }

    private static void drawIcon(ThingDef animal, Rect rect)
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