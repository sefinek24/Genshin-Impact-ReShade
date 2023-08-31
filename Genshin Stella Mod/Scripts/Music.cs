using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace GenshinStellaMod.Scripts
{
    internal static class Music
    {
        public static void PlaySound(string dir, string fileName)
        {
            string wavPath = Path.Combine(Program.AppPath, "data", "sounds", dir, $"{fileName}.wav");
            if (!File.Exists(wavPath))
            {
                Log.SaveError($"The sound file with music was not found in the location: {wavPath}");
                return;
            }

            Task.Run(() => PlaySoundAsync(wavPath, fileName == "information_bar" ? 0.59f : 1.61f));
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

                Log.Output($"Playing sound file: {wavPath}");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex.ToString());
            }
        }
    }
}
