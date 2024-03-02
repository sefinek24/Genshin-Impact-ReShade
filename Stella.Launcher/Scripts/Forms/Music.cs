using NAudio.Wave;
using StellaModLauncher.Properties;

namespace StellaModLauncher.Scripts.Forms;

internal static class Music
{
	private static readonly Random Random = new();

	public static async void PlayBg()
	{
		if (Program.Settings.ReadInt("Launcher", "EnableMusic", 1) == 0) return;

		string? wavPath = GetRandomBgWavPath();
		if (string.IsNullOrEmpty(wavPath)) return;

		await PlaySoundAsync(wavPath, 0.73f).ConfigureAwait(false);
	}

	private static string? GetRandomBgWavPath()
	{
		int randomBgNumber = Random.Next(1, 7);
		string wavPath = Path.Combine(Program.AppPath, "data", "sounds", "bg", $"{randomBgNumber}.wav");
		return File.Exists(wavPath) ? wavPath : null;
	}

	public static async void PlaySound(string dir, string fileName, float? volume = 0.55f)
	{
		if (Program.Settings.ReadInt("Launcher", "EnableBgSounds", 1) == 0) return;

		string wavPath = Path.Combine(Program.AppPath, "data", "sounds", dir, $"{fileName}.wav");
		if (!File.Exists(wavPath))
		{
			Utils.UpdateStatusLabel(Resources.Default_TheSoundFileWithMusicWasNotFound, Utils.StatusType.Error);
			Program.Logger.Error($"The sound file with music was not found in the location: {wavPath}");
			return;
		}

		await PlaySoundAsync(wavPath, (float)volume!).ConfigureAwait(false);
	}

	private static async Task PlaySoundAsync(string? wavPath, float volume)
	{
		try
		{
			using AudioFileReader audioFile = new(wavPath);
			using WaveChannel32 volumeStream = new(audioFile);
			volumeStream.Volume = volume;

			using WaveOutEvent outputDevice = new();
			outputDevice.Init(volumeStream);
			outputDevice.Play();
			await Task.Delay(TimeSpan.FromSeconds(audioFile.TotalTime.TotalSeconds)).ConfigureAwait(false);

			Program.Logger.Info($"Playing sound file: {wavPath}");
		}
		catch (Exception ex)
		{
			Utils.UpdateStatusLabel(ex.Message, Utils.StatusType.Error);
			Program.Logger.Error(ex.ToString());
		}
	}

	public static void LinkLabelSfx(Form form)
	{
		foreach (Control ctrl in form.Controls)
			if (ctrl is LinkLabel)
				ctrl.MouseHover += LinkLabel_MouseHover;
	}

	private static void LinkLabel_MouseHover(object? sender, EventArgs e)
	{
		PlaySound("other", "menu-button", 0.25f);
	}
}
