using Microsoft.Win32;

namespace ConfigurationNC.Scripts;

internal static class CheckData
{
	private const string RegistryPath = @"Software\Stella Mod Launcher";

	public static bool IsUserMyPatron()
	{
		return TryGetRegistryValue("Secret", out string? data) && !string.IsNullOrEmpty(data);
	}

	public static bool ResourcesPath()
	{
		return TryGetRegistryValue("ResourcesPath", out string? data) && Directory.Exists(data);
	}

	private static bool TryGetRegistryValue(string keyName, out string? value)
	{
		try
		{
			using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryPath);
			if (key is not null)
			{
				value = key.GetValue(keyName) as string;
				return !string.IsNullOrEmpty(value);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Sorry, but something went wrong. Please report this issue.\n\n{ex.Message}", @"Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		value = null;
		return false;
	}
}
