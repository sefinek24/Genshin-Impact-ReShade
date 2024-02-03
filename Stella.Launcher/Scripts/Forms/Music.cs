using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using StellaLauncher.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Forms
{
	internal static class Music
	{
		private static readonly Random Random = new Random();

		public static async Task PlayBg()
		{
			if (Program.Settings.ReadInt("Launcher", "EnableMusic", 1) == 0 || Debugger.IsAttached) return;

			string wavPath = GetRandomBgWavPath();
			if (string.IsNullOrEmpty(wavPath)) return;

			await PlaySoundAsync(wavPath, 0.76f);
		}

		private static string GetRandomBgWavPath()
		{
			int randomBgNumber = Random.Next(1, 7);
			string wavPath = Path.Combine(Program.AppPath, "data", "sounds", "bg", $"{randomBgNumber}.wav");
			return File.Exists(wavPath) ? wavPath : null;
		}

		public static void PlaySound(string dir, string fileName)
		{
			if (Program.Settings.ReadInt("Launcher", "EnableBgSounds", 1) == 0) return;

			string wavPath = Path.Combine(Program.AppPath, "data", "sounds", dir, $"{fileName}.wav");
			if (!File.Exists(wavPath))
			{
				Default._status_Label.Text += $"[x] {Resources.Default_TheSoundFileWithMusicWasNotFound}\n";
				Program.Logger.Error($"The sound file with music was not found in the location: {wavPath}");
				return;
			}

			float volume = fileName == "information_bar" ? 0.44f : 1.54f;
			PlaySoundAsync(wavPath, volume).ConfigureAwait(false);
		}

		private static async Task PlaySoundAsync(string wavPath, float volume)
		{
			try
			{
				using (AudioFileReader audioFile = new AudioFileReader(wavPath))
				using (WaveChannel32 volumeStream = new WaveChannel32(audioFile))
				{
					volumeStream.Volume = volume;
					using (WaveOutEvent outputDevice = new WaveOutEvent())
					{
						outputDevice.Init(volumeStream);
						outputDevice.Play();
						await Task.Delay(TimeSpan.FromSeconds(audioFile.TotalTime.TotalSeconds));
					}
				}

				Program.Logger.Info($"Playing sound file: {wavPath}");
			}
			catch (Exception ex)
			{
				Default._status_Label.Text += $"[x] {ex.Message}\n";
				Program.Logger.Error(ex.ToString());
			}
		}
	}
}
