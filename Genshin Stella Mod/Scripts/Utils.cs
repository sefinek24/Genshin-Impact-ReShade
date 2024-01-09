using System;
using System.Security.Principal;
using Microsoft.Win32;

namespace GenshinStellaMod.Scripts
{
	internal static class Utils
	{
		public static bool IsRunningWithAdminPrivileges()
		{
			using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
			{
				WindowsPrincipal principal = new WindowsPrincipal(identity);
				return principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}

		public static string GetResourcesPath()
		{
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Secret.RegistryPath))
			{
				object value = registryKey?.GetValue("ResourcesPath");
				if (!(value is string path)) return null;

				Program.Logger.Info($"Found resources path: {path}");
				return path;
			}
		}

		public static bool IsArrayEmpty(string[] obj)
		{
			return obj.Length == 0;
		}

		public static void Pause()
		{
			const string text = "Press any key to close this application...";
			Console.Write($"\n{text}");
			Program.Logger.Info(text);

			Console.ReadKey();

			Program.Logger.Info("Exiting...");
			Environment.Exit(0);
		}
	}
}
