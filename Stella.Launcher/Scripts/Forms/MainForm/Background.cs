using System;
using System.Drawing;
using System.IO;
using System.Runtime.Caching;
using System.Windows.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Forms.MainForm
{
	/// <summary>
	///    Utility class for managing background images in the main form.
	/// </summary>
	internal static class Background
	{
		private static readonly ObjectCache Cache = MemoryCache.Default;

		// Array of background image file names
		private static readonly string[] BackgroundFiles =
		{
			@"nahida\1",
			@"yaoyao\1", @"yaoyao\2",
			@"ayaka\1", @"ayaka\2", @"ayaka\3", @"ayaka\4",
			@"hutao\1", @"hutao\2", @"hutao\3", @"hutao\4"
		};

		/// <summary>
		///    Sets the initial background image for the main form.
		/// </summary>
		/// <param name="toolTip">ToolTip instance to show tooltip text.</param>
		/// <param name="changeBg">LinkLabel control for changing the background.</param>
		/// <returns>The initial background image or null if it's not found.</returns>
		public static Image OnStart(ToolTip toolTip, LinkLabel changeBg)
		{
			// Read the index of the last used background from settings
			int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);
			if (bgInt == 0) return null;

			bgInt--;

			string cacheKey = $"background_{bgInt}";
			if (Cache.Contains(cacheKey) && Cache.Get(cacheKey) is Bitmap cachedImage)
			{
				Program.Logger.Info(string.Format(Resources.Default_SuccessfullyRetrievedAndUpdatedTheCachedAppBackgroundWithID_, bgInt + 1));
				return cachedImage;
			}

			// Get the localization path of the background image
			string localization = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", $"{BackgroundFiles[bgInt]}.png");
			if (!Utils.CheckFileExists(localization))
			{
				MessageBox.Show(string.Format(Resources.Default_Sorry_Background_WasNotFound, BackgroundFiles[bgInt]), Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Program.Logger.Error($"Sorry. Background {Path.GetFileName(localization)} was not found in: {Path.GetDirectoryName(localization)}");
				return null;
			}

			Bitmap backgroundImage = new Bitmap(localization);
			Cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });
			Program.Logger.Info($"Cached app background in RAM memory '{localization}'; ID: {bgInt + 1};");

			toolTip.SetToolTip(changeBg, string.Format(Resources.Default_CurrentBackground, BackgroundFiles[bgInt]));
			return backgroundImage;
		}

		/// <summary>
		/// Changes the background image for the main form.
		/// </summary>
		/// <param name="bgFormImage"> Current background image of the main form.</param>
		/// <param name="toolTip">ToolTip instance to show tooltip text.</param>
		/// <param name="changeBg">LinkLabel control for changing the background.</param>
		/// <returns>The new background image after the change or null if the image is not found.</returns>
		public static Image Change(Image bgFormImage, ToolTip toolTip, LinkLabel changeBg)
		{
			int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);
			if (bgInt >= BackgroundFiles.Length) return SetDefaultBackground(bgFormImage, toolTip, changeBg);

			Image backgroundImage = GetCachedOrLoadImage(bgInt);
			if (backgroundImage == null) return null;

			toolTip.SetToolTip(changeBg, string.Format(Resources.Default_CurrentBackground, BackgroundFiles[bgInt]));

			// Increment the index and save it to settings
			bgInt++;
			Program.Settings.WriteInt("Launcher", "Background", bgInt);
			Program.Settings.Save();

			Program.Logger.Info(string.Format(Resources.Default_ChangedTheLauncherBackground_ID, bgInt));
			return backgroundImage;
		}

		/// <summary>
		///    Gets the cached background image for the given index or loads it from file if not found in the cache.
		/// </summary>
		/// <param name="bgInt"> The index of the background image.</param>
		/// <returns>The cached background image or the loaded image from file.</returns>
		private static Image GetCachedOrLoadImage(int bgInt)
		{
			// Create a cache key based on the background index
			string cacheKey = $"background_{bgInt}";
			if (Cache.Contains(cacheKey) && Cache.Get(cacheKey) is Bitmap cachedImage)
			{
				Program.Logger.Info(string.Format(Resources.Default_SuccessfullyRetrievedAndUpdatedTheCachedAppBackgroundWithID_, bgInt + 1));
				return cachedImage;
			}

			string localization = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", $"{BackgroundFiles[bgInt]}.png");
			if (!Utils.CheckFileExists(localization))
			{
				// If the file does not exist, show an error message and log the exception
				MessageBox.Show(string.Format(Resources.Default_Sorry_Background_WasNotFound, BackgroundFiles[bgInt]), Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Program.Logger.Error(string.Format(Resources.Default_Sorry_Background_WasNotFound, localization, bgInt));
				return null;
			}

			Bitmap backgroundImage = new Bitmap(localization);
			Cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });

			Program.Logger.Info(string.Format(Resources.Default_CachedAppBackground_ID, localization, bgInt + 1));
			return backgroundImage;
		}

		/// <summary>
		///    Sets the default background image for the main form.
		/// </summary>
		/// <param name="bgFormImage"> Current background image of the main form.</param>
		/// <param name="toolTip">ToolTip instance to show tooltip text.</param>
		/// <param name="changeBg">LinkLabel control for changing the background.</param>
		/// <returns>The default background image.</returns>
		private static Image SetDefaultBackground(Image bgFormImage, ToolTip toolTip, Control changeBg)
		{
			bgFormImage = Resources.bg_main;
			Program.Settings.WriteInt("Launcher", "Background", 0);
			Program.Settings.Save();
			toolTip.SetToolTip(changeBg, Resources.Default_CurrentBackground_Default);
			Program.Logger.Info(string.Format(Resources.Default_TheApplicationBackgroundHasBeenChangedToDefault_ID, 0));
			return bgFormImage;
		}
	}
}
