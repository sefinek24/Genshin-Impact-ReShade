using System.Diagnostics;

namespace BuildStellaMod;

internal static class Program
{
	private static readonly string CurrentDir = Directory.GetCurrentDirectory();
	private static readonly string SolutionFilePath = Path.Combine(CurrentDir, "..", "..", "Genshin Stella Mod made by Sefinek.sln");
	private static readonly string SmPlusApp = Path.Combine(CurrentDir, "..", "..", "Stella.ConfigurationOld", "3-2. Configuration .NET Framework.csproj");
	private static readonly string ReleaseBuildPath = Path.Combine(CurrentDir, "..", "..", "Build", "Release", "net8.0-windows10.0.20348.0");
	private static readonly string ObjBuildPath = Path.Combine(CurrentDir, "..", "..", "Stella.ConfigurationOld", "obj", "Release");

	private static readonly string DestinationFolder1 = @"C:\GitHub\VS\Stella\Genshin-Impact-ReShade\Build\GitHub";

	private static void Main()
	{
		Console.WriteLine("Current directory: " + CurrentDir);
		Console.WriteLine("Solution file path: " + SolutionFilePath);
		Console.WriteLine("Project path: " + SmPlusApp);
		Console.WriteLine("Release folder path: " + ReleaseBuildPath);

		Console.WriteLine();

		DeleteDirectoryIfExists(ReleaseBuildPath);
		DeleteDirectoryIfExists(ObjBuildPath);

		Console.WriteLine();

		bool compilationSuccess = CompileProject(SolutionFilePath);
		Console.WriteLine();
		if (compilationSuccess)
		{
			Console.WriteLine("----------------------------- Compilation successful -----------------------------");
			CopyFiles(ReleaseBuildPath);
		}
		else
		{
			Console.WriteLine("----------------------------- Compilation failed! -----------------------------");
		}

		Console.WriteLine("\nPress ENTER to exit the program...");
		Console.ReadLine();
	}

	private static void DeleteDirectoryIfExists(string directoryPath)
	{
		if (!Directory.Exists(directoryPath)) return;

		Console.WriteLine($"Deleting folder: {directoryPath}");
		try
		{
			Directory.Delete(directoryPath, true);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred while deleting folder: {ex.Message}");
		}
	}

	private static bool CompileProject(string solutionFilePath)
	{
		string? workingDir = Path.GetDirectoryName(solutionFilePath);
		if (!Directory.Exists(workingDir))
		{
			Console.WriteLine($"Directory {workingDir} was not found.");
			return false;
		}

		Console.WriteLine("----------------------------- Starting compilation -----------------------------");

		try
		{
			Process buildProcess = new()
			{
				StartInfo =
				{
					FileName = "MSBuild.exe",
					Arguments = $"\"{SmPlusApp}\" /p:Configuration=Release",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					WorkingDirectory = workingDir
				},
				EnableRaisingEvents = true
			};

			buildProcess.OutputDataReceived += (_, args) =>
			{
				if (!string.IsNullOrEmpty(args.Data)) Console.WriteLine(args.Data);
			};

			buildProcess.Start();
			buildProcess.BeginOutputReadLine();
			buildProcess.WaitForExit();

			return buildProcess.ExitCode == 0;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"----------------------------- Error occurred during compilation -----------------------------\n\n{ex}");
			return false;
		}
	}

	private static void CopyFiles(string sourceFolderPath)
	{
		Console.WriteLine("\nCopying files...");

		try
		{
			string[] files = Directory.GetFiles(sourceFolderPath, "*.*", SearchOption.AllDirectories);

			foreach (string file in files)
			{
				string relativePath = file[(sourceFolderPath.Length + 1)..];
				string destinationPath1 = Path.Combine(DestinationFolder1, relativePath);

				string? dirNameDestinationPath1 = Path.GetDirectoryName(destinationPath1);
				if (string.IsNullOrEmpty(dirNameDestinationPath1))
					Console.WriteLine($"dirNameDestinationPath1 is {dirNameDestinationPath1}");
				else
					Directory.CreateDirectory(dirNameDestinationPath1);

				File.Copy(file, destinationPath1, true);
			}

			Console.WriteLine("Source files have been copied to the specified folders:");
			Console.WriteLine(DestinationFolder1);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred while copying files: {ex.Message}");
		}
	}
}
