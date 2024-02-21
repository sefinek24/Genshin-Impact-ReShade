using System.Runtime.InteropServices;
using System.Text;

namespace StellaPLFNetF
{
	public class IniFile
	{
		private const int BufferSize = 1024;
		private readonly string _path;

		public IniFile(string path)
		{
			_path = path;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		public bool WriteInt(string section, string key, int value)
		{
			return WriteString(section, key, value.ToString());
		}

		public int ReadInt(string section, string key, int defaultValue)
		{
			string result = ReadString(section, key, defaultValue.ToString());
			return int.TryParse(result, out int value) ? value : defaultValue;
		}

		public bool WriteString(string section, string key, string value)
		{
			return WritePrivateProfileString(section, key, value, _path) != 0;
		}

		public string ReadString(string section, string key, string defaultValue)
		{
			StringBuilder sb = new StringBuilder(BufferSize);
			GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, _path);
			return sb.ToString();
		}

		public void Save()
		{
			WritePrivateProfileString(null, null, null, _path);
		}
	}
}
