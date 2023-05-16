using System;
using System.IO;

namespace Prepare_mod.Scripts.Preparing
{
    internal static class DeleteReShadeCache
    {
        public static void Run()
        {
            Console.WriteLine(@"Deleting ReShade cache...");

            int numFilesDeleted = 0;
            long spaceSaved = 0;
            DirectoryInfo cacheDir = new DirectoryInfo(Path.Combine(Program.ResourcesGlobal, "Cache"));
            if (cacheDir.Exists)
                foreach (FileInfo file in cacheDir.EnumerateFiles())
                {
                    spaceSaved += file.Length;
                    file.Delete();
                    numFilesDeleted++;
                }

            Console.WriteLine(spaceSaved > 1000 ? $@"Deleted {numFilesDeleted} cache files and saved {spaceSaved / 1000000} MB." : $@"Deleted {numFilesDeleted} cache files and saved {spaceSaved} KB.");
        }
    }
}
