using System;
using System.IO;
using System.Threading.Tasks;

namespace PrepareStella.Scripts.Preparing
{
    /// <summary>
    ///     Deletes the ReShade cache files and displays the space saved.
    /// </summary>
    internal static class DeleteReShadeCache
    {
        private const int KilobyteInBytes = 1000;
        private const int MegabyteInBytes = 1000000;

        public static async Task Run()
        {
            Console.WriteLine(@"Deleting ReShade cache...");

            int deletedFilesCount = 0;
            long savedSpace = 0;
            string cacheDirectoryPath = Path.Combine(Program.ResourcesGlobal, "Cache");

            if (Directory.Exists(cacheDirectoryPath))
                foreach (string filePath in Directory.EnumerateFiles(cacheDirectoryPath))
                {
                    FileInfo file = new FileInfo(filePath);
                    savedSpace += file.Length;
                    await Task.Run(() => File.Delete(filePath));
                    deletedFilesCount++;
                }

            string spaceUnit = savedSpace > KilobyteInBytes ? "MB" : "KB";
            double spaceSaved = savedSpace / (double)(savedSpace > KilobyteInBytes ? MegabyteInBytes : KilobyteInBytes);
            Console.WriteLine($@"Deleted {deletedFilesCount} cache files and saved {spaceSaved} {spaceUnit}.");
        }
    }
}
