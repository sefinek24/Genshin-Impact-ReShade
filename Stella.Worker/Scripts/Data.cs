using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace GenshinStellaMod.Scripts
{
	internal static class Data
	{
		private const string RegistryPath = @"Software\Stella Mod Launcher";

		public static bool IsUserMyPatron()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
			{
				if (key == null) return false;

				string data = (string)key.GetValue("Secret");
				return !string.IsNullOrEmpty(data);
			}
		}

		public static void CheckProcess(string processName)
		{
			if (string.IsNullOrEmpty(processName))
			{
				Log.ThrowErrorString("[x] Process name cannot be null or empty.");
				return;
			}

			if (processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) processName = processName.Substring(0, processName.Length - 4);

			Process[] processes = Process.GetProcessesByName(processName);
			foreach (Process process in processes)
			{
				Console.WriteLine($"[i] Process {process.ProcessName} is running. Killing...");
				Program.Logger.Info($"Killing process {process.ProcessName}...");

				process.Kill();
				process.WaitForExit();
			}
		}

		public static string GetGamePath()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Secret.RegistryPath, true))
			{
				string gPath = (string)(key?.GetValue("GamePath") ?? "");
				return !string.IsNullOrEmpty(gPath) ? Path.Combine(gPath) : "";
			}
		}
	}
}
