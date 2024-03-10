using System.Text.RegularExpressions;

namespace StellaModLauncher.Scripts;

internal static class StellaVersion
{
	public static string? Parse(string version)
	{
		Match match = Regex.Match(version, @"^\d+\.\d+\.\d+\.?\d*");
		return match.Success ? match.Value : null;
	}
}
