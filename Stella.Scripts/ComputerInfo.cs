using System.Globalization;
using System.Management;
using Microsoft.Win32;
using NLog;

namespace StellaUtils;

public static class ComputerInfo
{
	private static readonly Logger Logger = LogManagerHelper.GetLogger();

	private static string Identifier(string wmiClass, string wmiProperty, string? wmiMustBeTrue = null)
	{
		try
		{
			if (!ManagementClassExists(wmiClass))
			{
				Logger.Error($"WMI class '{wmiClass}' does not exist");
				return string.Empty;
			}

			SelectQuery query = new($"SELECT {wmiProperty} FROM {wmiClass}");
			using ManagementObjectSearcher searcher = new(query);
			foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
				if (wmiMustBeTrue == null || obj[wmiMustBeTrue]?.ToString() == "True")
					return obj[wmiProperty]?.ToString() ?? string.Empty;
		}
		catch (ManagementException ex)
		{
			Logger.Error($"ManagementException occurred: {ex.Message}");
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception occurred: {ex.Message}");
		}

		Logger.Error($"GET {wmiClass}");

		return string.Empty;
	}

	private static bool ManagementClassExists(string className)
	{
		try
		{
			using ManagementClass mc = new(className);
			mc.GetInstances().Dispose();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static string GetMotherboardSerialNumber()
	{
		return Identifier("Win32_BaseBoard", "SerialNumber");
	}

	public static string GetCpuSerialNumber()
	{
		string result = Identifier("Win32_Processor", "ProcessorId");
		if (string.IsNullOrEmpty(result)) result = Identifier("Win32_Processor", "UniqueId");

		return result;
	}

	public static string GetHardDriveSerialNumber()
	{
		return Identifier("Win32_PhysicalMedia", "SerialNumber");
	}

	public static string GetSystemRegion()
	{
		return RegionInfo.CurrentRegion.TwoLetterISORegionName;
	}

	public static string GetFullBuildNumber()
	{
		RegistryKey? registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

		string? ubr = registryKey?.GetValue("UBR")?.ToString();
		string? currentBuild = registryKey?.GetValue("CurrentBuild")?.ToString();

		return currentBuild + "." + ubr;
	}

	public static string? GetSystemVersion()
	{
		string? displayVersion = null;

		using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
		object? value = key?.GetValue("DisplayVersion");
		if (value != null) displayVersion = value.ToString();

		return displayVersion;
	}
}
