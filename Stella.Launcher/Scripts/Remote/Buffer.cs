namespace StellaModLauncher.Scripts.Remote;

internal static class Buffer
{
	/*
	 *    65536 = 64 KB
	 *     8192 = 8 KB
	 *  1048576 = 1 MB
	 */

	public static byte[] Get()
	{
		int bufferCfg = Program.Settings.ReadInt("Launcher", "BufferValue", 65536);
		return new byte[bufferCfg];
	}
}
