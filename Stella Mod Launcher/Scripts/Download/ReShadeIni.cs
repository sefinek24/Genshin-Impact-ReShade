using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Forms.MainForm;

namespace StellaLauncher.Scripts.Download
{
	internal static class ReShadeIni
	{
		public static async Task CheckIfExists()
		{
			string gameDir = await Utils.GetGame("giGameDir");
			string reShadePath = Path.Combine(gameDir, "ReShade.ini");
			if (File.Exists(reShadePath)) return;

			try
			{
				HttpResponseMessage response = await Program.SefinWebClient.GetAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini");
				if (!response.IsSuccessStatusCode)
				{
					Default._status_Label.Text += $"[x] {Resources.Default_FileWasNotFound}\n";
					Program.Logger.Error("Failed to download ReShade.ini from the server.");
					return;
				}

				using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(reShadePath, FileMode.Create, FileAccess.Write))
				{
					await contentStream.CopyToAsync(fileStream);
				}

				await RsConfig.Prepare();

				Default._status_Label.Text += $"[✓] {Resources.Default_SuccessfullyDownloadedReShadeCfg}\n";
				Program.Logger.Info($"Successfully downloaded ReShade.ini and saved in: {reShadePath}");

				await CheckForUpdates.Analyze();
			}
			catch (Exception ex)
			{
				Default._status_Label.Text += $"[x] {Resources.Default_Meeow_FailedToDownloadReShadeIni_TryAgain}\n";
				Program.Logger.Error(ex.ToString());
				if (!File.Exists(reShadePath)) Program.Logger.Info(Resources.Default_TheReShadeIniFileStillDoesNotExist);
			}
		}
	}
}
