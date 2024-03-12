namespace StellaModLauncher.Scripts.Helpers;

internal static class BufferHelper
{
	/*
	 *    65536 = 64 KB
	 *     8192 = 8 KB
	 *  1048576 = 1 MB
	 */
	public static byte[] Get()
	{
		int bufferCfg = Program.Settings.ReadInt("Launcher", "BufferValue", 1048576);
		return new byte[bufferCfg];
	}
}
