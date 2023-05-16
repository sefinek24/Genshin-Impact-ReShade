using System;
using System.IO;
using System.Threading.Tasks;

namespace Prepare_mod.Scripts.Preparing
{
    internal static class DeleteReShadeCache
    {
        private const int KbInBytes = 1000;
        private const int MbInBytes = 1000000;

        public static async Task Run()
        {
            Console.WriteLine(@"Deleting ReShade cache...");

            int deletedFilesCount = 0;
            long savedSpace = 0;
            string cacheDirectoryPath = Path.Combine(Program.ResourcesGlobal, "Cache");

            if (Directory.Exists(cacheDirectoryPath))
            {
                foreach (string filePath in Directory.EnumerateFiles(cacheDirectoryPath))
                {
                    FileInfo file = new FileInfo(filePath);
                    savedSpace += file.Length;
                    await Task.Run(() => File.Delete(filePath));
                    deletedFilesCount++;
                }

                Directory.Delete(cacheDirectoryPath, true);
            }

            string spaceUnit = savedSpace > KbInBytes ? "MB" : "KB";
            double spaceSaved = savedSpace / (double)(savedSpace > KbInBytes ? MbInBytes : KbInBytes);
            Console.WriteLine($@"Deleted {deletedFilesCount} cache files and saved {spaceSaved} {spaceUnit}.");
        }
    }
}
