using System;
using Mlie;
using UnityEngine;
using Verse;

namespace Rats
{
    [StaticConstructorOnStartup]
    internal class RatsMod : Mod
    {
        /// <summary>
        ///     The instance of the settings to be read by the mod
        /// </summary>
        public static RatsMod instance;

        private static string currentVersion;


        /// <summary>
        ///     The private settings
        /// </summary>
        private RatsModSettings settings;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="content"></param>
        public RatsMod(ModContentPack content)
            : base(content)
        {
            instance = this;

            currentVersion =
                VersionFromManifest.GetVersionFromModMetaData(
                    ModLister.GetActiveModWithIdentifier("Mlie.Rats"));
        }

        /// <summary>
        ///     The instance-settings for the mod
        /// </summary>
        internal RatsModSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GetSettings<RatsModSettings>();
                }

                return settings;
            }

            set => settings = value;
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
            listing_Standard.Gap();
            Settings.MaxRats = (int)Widgets.HorizontalSlider(listing_Standard.GetRect(20),
                Settings.MaxRats, 1, 20, false,
                "Rats.maxrats.label".Translate(Settings.MaxRats), null, null, 1f);
            listing_Standard.Gap();
            Settings.MaxPerDay = Math.Max(Settings.MaxRats, (int)Widgets.HorizontalSlider(listing_Standard.GetRect(20),
                Settings.MaxPerDay, Settings.MaxRats, 50, false,
                "Rats.maxperday.label".Translate(Settings.MaxPerDay), null, null, 1f));
            listing_Standard.Gap();
            Settings.MaxTotalRats = (int)Widgets.HorizontalSlider(listing_Standard.GetRect(20),
                Settings.MaxTotalRats, 0, 100, false,
                "Rats.maxtotalrats.label".Translate(Settings.MaxTotalRats), null, null, 1f);
            listing_Standard.Gap();
            Settings.PercentScaria = Widgets.HorizontalSlider(listing_Standard.GetRect(20),
                Settings.PercentScaria, 0, 1f, false,
                "Rats.percentscaria.label".Translate(Settings.PercentScaria * 100), null, null, 0.01f);
            listing_Standard.Gap();
            Settings.MinDays = Widgets.HorizontalSlider(listing_Standard.GetRect(20),
                Settings.MinDays, 0, 30, false,
                "Rats.mindays.label".Translate(Settings.MinDays), null, null, 0.1f);
            listing_Standard.Gap();
            Settings.RotDays = (int)Widgets.HorizontalSlider(listing_Standard.GetRect(20),
                Settings.RotDays, 1, 30, false,
                "Rats.rotdays.label".Translate(Settings.RotDays), null, null, 1f);
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

            listing_Standard.End();
        }
    }
}