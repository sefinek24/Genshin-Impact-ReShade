using DeviceId;
using Microsoft.Win32;
using StellaModLauncher.Properties;

namespace StellaModLauncher.Scripts;

internal static class Secret
{
	public static bool IsStellaPlusSubscriber = false;
	public static string? BearerToken;
	private static string? _deviceId;

	public static string? GetTokenFromRegistry()
	{
		using RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(Program.RegistryPath);
		object? value = registryKey?.GetValue("Secret");
		if (value is not string token) return null;

		Program.Logger.Info("Found a `Secret` token used by subscribers in the registry");
		return token;
	}

	public static async Task<string?> VerifyToken(string? registrySecret)
	{
		try
		{
			List<KeyValuePair<string, string?>> postData =
			[
				new KeyValuePair<string, string?>("token", registrySecret),

				new KeyValuePair<string, string?>("deviceId", _deviceId),
				new KeyValuePair<string, string?>("motherboardId", ComputerInfo.GetMotherboardSerialNumber()),
				new KeyValuePair<string, string?>("cpuId", ComputerInfo.GetCpuSerialNumber()),
				new KeyValuePair<string, string?>("diskId", ComputerInfo.GetHardDriveSerialNumber())
			];

			FormUrlEncodedContent content = new(postData);
			HttpResponseMessage response = await Program.WbClient.Value.PostAsync($"{Program.WebApi}/stella-mod-plus/launcher/verify", content).ConfigureAwait(false);

			string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			if (!response.IsSuccessStatusCode) Program.Logger.Error("Secret.VerifyToken(): " + json);

			return json;
		}
		catch (Exception ex)
		{
			MessageBox.Show(string.Format(Resources.Secret_VerifyToken_Exception, ex.InnerException ?? ex), Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

			Log.ErrorAndExit(ex, false);
			return null;
		}
	}

	public static void GetDeviceId()
	{
		_deviceId = new DeviceIdBuilder()
			.UseFormatter(DeviceIdFormatters.DefaultV6)
			.OnWindows(windows => windows
				.AddProcessorId()
				.AddMotherboardSerialNumber()
				.AddSystemDriveSerialNumber()
				.AddWindowsDeviceId()
				.AddWindowsProductId()
				.AddSystemUuid()
				.AddMachineGuid())
			.ToString();
	}
}
