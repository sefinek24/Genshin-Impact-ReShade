using System;
using System.IO;
using System.Media;
using StellaLauncher.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class Music
    {
        public static void Play()
        {
            int muteBgMusic = Program.Settings.ReadInt("Launcher", "EnableMusic", 1);
            if (muteBgMusic != 1) return;

            Random random = new Random();
            string wavPath = Path.Combine(Program.AppPath, "data", "sounds", "bg", $"{random.Next(1, 6 + 1)}.wav");
            if (!File.Exists(wavPath))
            {
                Default._status_Label.Text += $"[x]: {Resources.Default_TheSoundFileWithMusicWasNotFound}\n";
                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_TheSoundFileWithMusicWasNotFoundInTheLocalization_, wavPath)));
                return;
            }

            try
            {
                new SoundPlayer { SoundLocation = wavPath }.Play();
                Log.Output(string.Format(Resources.Default_PlayingSoundFile_, wavPath));
            }
            catch (Exception ex)
            {
                Default._status_Label.Text += $"[x]: {ex.Message}\n";
                Log.SaveErrorLog(ex);
            }
        }
    }
}
