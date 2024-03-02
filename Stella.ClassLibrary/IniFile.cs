using System.Runtime.InteropServices;
using System.Text;

namespace StellaPLFNet;

public class IniFile(string path)
{
	private const int BufferSize = 1024;

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
	private static extern long WritePrivateProfileString(string? section, string? key, string? val, string filePath);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
	private static extern int GetPrivateProfileString(string? section, string? key, string? def, StringBuilder retVal, int size, string filePath);

	public void WriteInt(string? section, string? key, int value)
	{
		WriteString(section, key, value.ToString());
	}

	public int ReadInt(string? section, string? key, int defaultValue)
	{
		string result = ReadString(section, key);
		if (!string.IsNullOrEmpty(result)) return int.TryParse(result, out int value) ? value : defaultValue;

		WriteString(section, key, defaultValue.ToString());
		return defaultValue;
	}

	public bool WriteString(string? section, string? key, string? value)
	{
		return WritePrivateProfileString(section, key, value, path) != 0;
	}

	public string ReadString(string? section, string? key, string? defaultValue = null)
	{
		StringBuilder sb = new(BufferSize);
		GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, path);

		return sb.ToString();
	}

	public void Save()
	{
		WritePrivateProfileString(null, null, null, path);
	}
}
