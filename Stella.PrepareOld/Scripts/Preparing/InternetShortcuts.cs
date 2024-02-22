using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PrepareStella.Scripts.Preparing
{
	internal static class InternetShortcuts
	{
		public static async Task RunAsync()
		{
			string appStartMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Stella Mod Launcher");

			try
			{
				// Create Internet shortcuts
				await CreateInternetShortcutsAsync(appStartMenuPath);
			}
			catch (Exception e)
			{
				Log.ThrowError(e, false);
			}
		}


		private static async Task CreateInternetShortcutsAsync(string appStartMenuPath)
		{
			await Task.Run(() =>
			{
				try
				{
					Dictionary<string, string> urls = new Dictionary<string, string>
					{
						{ "Official website", "https://genshin.sefinek.net" },
						{ "Donate", "https://sefinek.net/support-me" },
						{ "Gallery", "https://sefinek.net/genshin-impact-reshade/gallery?page=1" },
						{ "Support", "https://sefinek.net/genshin-impact-reshade/support" },
						{ "Leave feedback", "https://sefinek.net/genshin-impact-reshade/feedback" }
					};

					foreach (KeyValuePair<string, string> kvp in urls)
					{
						string url = Path.Combine(appStartMenuPath, $"{kvp.Key} - Genshin Stella Mod.url");
						using (StreamWriter writer = new StreamWriter(url))
						{
							writer.WriteLineAsync($"[InternetShortcut]\nURL={kvp.Value}");
						}
					}
				}
				catch (Exception e)
				{
					Log.ThrowError(e, false);
				}
			});
		}
	}
}
