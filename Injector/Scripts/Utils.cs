using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;

namespace Injector.Scripts;

internal static class Utils
{
    public static void OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Log.ThrowError(new Exception("URL is null or empty."), false);
            return;
        }

        try
        {
            Process.Start(url);
            Log.Output($"Opened '{url}' in default browser.");
        }
        catch (Exception ex)
        {
            Log.ThrowError(new Exception($"Failed to open '{url}' in default browser.\n{ex}"), false);
        }
    }

    public static string GetAppData()
    {
        try
        {
            return Path.Combine(ApplicationData.Current?.LocalFolder?.Path);
        }
        catch (InvalidOperationException)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Genshin Stella Mod");
        }
    }
}
