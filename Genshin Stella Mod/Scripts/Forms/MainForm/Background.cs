using System;
using System.Drawing;
using System.IO;
using System.Runtime.Caching;
using System.Windows.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class Background
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        private static readonly string[] BackgroundFiles =
        {
            @"nahida\1",
            @"yaoyao\1", @"yaoyao\2",
            @"ayaka\1", @"ayaka\2", @"ayaka\3", @"ayaka\4",
            @"hutao\1", @"hutao\2", @"hutao\3", @"hutao\4"
        };

        public static Image OnStart(ToolTip toolTip, LinkLabel changeBg)
        {
            // Background
            int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);
            if (bgInt == 0) return null;

            bgInt--;

            string cacheKey = $"background_{bgInt}";
            string localization = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", $"{BackgroundFiles[bgInt]}.png");
            if (!Utils.CheckFileExists(localization))
            {
                MessageBox.Show(string.Format(Resources.Default_Sorry_Background_WasNotFound, BackgroundFiles[bgInt]), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_Sorry_Background_WasNotFound, localization, bgInt)));
                return null;
            }

            Bitmap backgroundImage = new Bitmap(localization);
            Cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });
            Log.Output(string.Format(Resources.Default_CachedAppBackground_ID, localization, bgInt + 1));

            toolTip.SetToolTip(changeBg, string.Format(Resources.Default_CurrentBackground, BackgroundFiles[bgInt]));

            return backgroundImage;
        }

        public static Image Change(Image bgFormImage, ToolTip toolTip, LinkLabel changeBg)
        {
            int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);

            if (bgInt >= BackgroundFiles.Length) return SetDefaultBackground(bgFormImage, toolTip, changeBg);

            Image backgroundImage = GetCachedOrLoadImage(bgInt);
            if (backgroundImage == null) return null;

            toolTip.SetToolTip(changeBg, string.Format(Resources.Default_CurrentBackground, BackgroundFiles[bgInt]));

            bgFormImage = backgroundImage;
            bgInt++;
            Program.Settings.WriteInt("Launcher", "Background", bgInt);
            Program.Settings.Save();

            Log.Output(string.Format(Resources.Default_ChangedTheLauncherBackground_ID, bgInt));
            return bgFormImage;
        }

        private static Image GetCachedOrLoadImage(int bgInt)
        {
            string cacheKey = $"background_{bgInt}";
            if (Cache.Contains(cacheKey) && Cache.Get(cacheKey) is Bitmap cachedImage)
            {
                Log.Output(string.Format(Resources.Default_SuccessfullyRetrievedAndUpdatedTheCachedAppBackgroundWithID_, bgInt + 1));
                return cachedImage;
            }

            string localization = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", $"{BackgroundFiles[bgInt]}.png");
            if (!Utils.CheckFileExists(localization))
            {
                MessageBox.Show(string.Format(Resources.Default_Sorry_Background_WasNotFound, BackgroundFiles[bgInt]), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_Sorry_Background_WasNotFound, localization, bgInt)));
                return null;
            }

            Bitmap backgroundImage = new Bitmap(localization);
            Cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });

            Log.Output(string.Format(Resources.Default_CachedAppBackground_ID, localization, bgInt + 1));
            return backgroundImage;
        }

        private static Image SetDefaultBackground(Image bgFormImage, ToolTip toolTip, Control changeBg)
        {
            bgFormImage = Resources.bg_main;
            Program.Settings.WriteInt("Launcher", "Background", 0);
            Program.Settings.Save();
            toolTip.SetToolTip(changeBg, Resources.Default_CurrentBackground_Default);
            Log.Output(string.Format(Resources.Default_TheApplicationBackgroundHasBeenChangedToDefault_ID, 0));
            return bgFormImage;
        }
    }
}
