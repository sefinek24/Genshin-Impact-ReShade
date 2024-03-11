using System.Diagnostics;

namespace BuildStellaMod;

internal static class Program
{
	public static readonly string RootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".."));
	private static readonly string InnoSetupCompiler = Path.Combine(@"C:\Program Files (x86)\Inno Setup 6\Compil32.exe");
	private static readonly string InnoSetupPath = Path.GetFullPath(Path.Combine(RootPath, "InnoSetup", "Setup.iss"));
	private static readonly string ReleaseBuildPath = Path.Combine(RootPath, "Build", "Release");

	private static async Task Main()
	{
		Console.WriteLine($"* Current directory: {Directory.GetCurrentDirectory()}");
		Console.WriteLine($"* Release folder path: {ReleaseBuildPath}\n");


		// Kill inject64
		Utils.KillProcess("inject64");

		// Delete Build\Release
		await Utils.DeleteDirectory(ReleaseBuildPath).ConfigureAwait(false);

		// Prepare
		string[] projects =
		[
			@"Stella.Launcher\1. Stella Mod Launcher.csproj",
			@"Stella.Worker\2. Genshin Stella Mod.csproj",
			@"Stella.Configuration\3. Configuration .NET 8.csproj",
			@"Stella.Prepare\4. Prepare Stella.csproj",
			@"Stella.Info-48793142\5. Information 48793142.csproj",
			@"Stella.DeviceIdentifier\6. Device Identifier.csproj"
		];

		foreach (string project in projects) await CleanupSolution(Path.Combine(RootPath, project)).ConfigureAwait(false);

		// Restore Nuget packages
		Console.WriteLine("\nRestore Nuget packages...");
		await Utils.RestorePackages().ConfigureAwait(false);
		Console.WriteLine();

		// Compile
		foreach (string project in projects) await PrepareCompilation(Path.Combine(RootPath, project)).ConfigureAwait(false);


		// Done!
		Console.Write("Would you like to run InnoSetup? [Yes/no]: ");
		string? response1 = Console.ReadLine()?.ToLower();
		if (response1 is "yes" or "y")
		{
			if (File.Exists(InnoSetupCompiler) || File.Exists(InnoSetupPath))
			{
				Console.WriteLine($"Running {InnoSetupPath}...");
				Process.Start(InnoSetupCompiler, InnoSetupPath);
			}
			else
			{
				Console.WriteLine("InnoSetup exe or iss file was not found");
			}
		}

		Console.Write("\nWould you like to open InnoSetup directory? [Yes/no]: ");
		string? response2 = Console.ReadLine()?.ToLower();
		if (response2 is "yes" or "y")
		{
			string? dir = Path.GetDirectoryName(InnoSetupPath);
			if (Directory.Exists(dir))
				Process.Start("explorer.exe", dir);
			else
				Console.WriteLine($"InnoSetup path directory was not found: {dir}");
		}

		Console.WriteLine("\nPress ENTER to exit the program...");
		Console.ReadLine();
	}

	private static async Task CleanupSolution(string projectPath)
	{
		string binPath = Path.Combine(Path.GetDirectoryName(projectPath)!, "bin");
		await Utils.DeleteDirectory(binPath).ConfigureAwait(false);

		string objPath = Path.Combine(Path.GetDirectoryName(projectPath)!, "obj");
		await Utils.DeleteDirectory(objPath).ConfigureAwait(false);
	}

	private static Task PrepareCompilation(string projectPath)
	{
		bool compilationSuccess = Utils.CompileProject(projectPath);
		Console.WriteLine();

		if (!compilationSuccess) Console.WriteLine($"----------------------------- FAILED TO COMPILE {Path.GetFileName(projectPath)} -----------------------------\n\n");

		return Task.CompletedTask;
	}
}
