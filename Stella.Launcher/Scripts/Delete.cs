using System.Security;
using Microsoft.Win32;

namespace StellaModLauncher.Scripts;

internal static class Delete
{
	public static void RegistryKey(string key)
	{
		try
		{
			using (RegistryKey subKey = Registry.CurrentUser.OpenSubKey(Secret.RegistryKeyPath, true))
			{
				if (subKey != null)
				{
					object value = subKey.GetValue(key);
					if (value != null)
					{
						subKey.DeleteValue(key);
						Program.Logger.Info($"Deleted REG_SZ value '{key}' from the registry.");
					}
					else
					{
						Program.Logger.Info($"REG_SZ value '{key}' not found in the registry.");
					}
				}
				else
				{
					Program.Logger.Info($"Registry key '{key}' not found.");
				}
			}
		}
		catch (SecurityException securityEx)
		{
			Program.Logger.Error($"Permission error while deleting registry value '{key}': {securityEx.Message}");
		}
		catch (UnauthorizedAccessException unauthorizedEx)
		{
			Program.Logger.Error($"Unauthorized access error while deleting registry value '{key}': {unauthorizedEx.Message}");
		}
		catch (IOException ioEx)
		{
			Program.Logger.Error($"I/O error while deleting registry value '{key}': {ioEx.Message}");
		}
		catch (Exception ex)
		{
			Log.ErrorAndExit(new Exception($"An error occurred while deleting registry value '{key}': {ex.Message}"), true);
		}
	}
}
