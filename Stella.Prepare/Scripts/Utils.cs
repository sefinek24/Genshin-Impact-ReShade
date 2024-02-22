using System.Diagnostics;
using System.Security.Principal;

namespace PrepareStella.Scripts;

internal static class Utils
{
	public static bool IsRunAsAdmin()
	{
		WindowsIdentity identity = WindowsIdentity.GetCurrent();
		WindowsPrincipal principal = new(identity);
		return principal.IsInRole(WindowsBuiltInRole.Administrator);
	}

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
			Start.Logger.Info($"Opened '{url}' in default browser.");
		}
		catch (Exception ex)
		{
			Log.ThrowError(new Exception($"Failed to open '{url}' in default browser.\n{ex}"), false);
		}
	}

	public static string GetWtProgramFiles()
	{
		if (!Directory.Exists(Program.WindowsApps))
		{
			Start.Logger.Info($"{Program.WindowsApps} was not found.");
			return null;
		}

		string[] dirs = Directory.GetDirectories(Program.WindowsApps, "Microsoft.WindowsTerminal_*", SearchOption.AllDirectories);

		string path = "";
		foreach (string dir in dirs) path = dir;

		return path;
	}
}
