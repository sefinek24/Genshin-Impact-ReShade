using System.Runtime.InteropServices;
using System.Text;

namespace PrepareStella.Scripts
{
    public class IniFile
    {
        private readonly string _path;

        public IniFile(string path)
        {
            _path = path;
        }

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public int ReadInt(string section, string key, int defaultValue)
        {
            int.TryParse(ReadString(section, key, defaultValue.ToString()), out int value);
            return value;
        }

        private string ReadString(string section, string key, string defaultValue)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, sb, 255, _path);
            return sb.ToString();
        }
    }
}
