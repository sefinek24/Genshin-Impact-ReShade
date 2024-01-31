using System;

namespace GenshinStellaMod.Scripts
{
	internal static class Log
	{
		public static void ThrowError(Exception ex)
		{
			Console.WriteLine($"{ex.Message}\n");

			Program.Logger.Error(ex);
		}

		public static void ThrowErrorString(string log)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(log);

			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("\n=========================================================================================");
			Console.WriteLine("[x] Meeow (=〃ﻌ〃=)! We're sorry. It seems like we've encountered an issue.");
			Console.WriteLine("[i] If you require assistance, kindly visit: https://genshin.sefinek.net/support");

			Music.PlaySound("winxp", "exclamation");

			Program.Logger.Error(log);
			Utils.Pause();
		}

		public static void ErrorAndExit(Exception ex)
		{
			ThrowError(ex);

			Environment.Exit(999991000);
		}
	}
}
