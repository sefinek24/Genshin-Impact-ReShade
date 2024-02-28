using System.Diagnostics;

namespace BuildStellaMod;

internal static class Utils
{
	private static readonly string DestinationFolder1 = @"C:\GitHub\VS\Stella\Genshin-Impact-ReShade\Build\GitHub";

	public static async Task DeleteDirectoryIfExistsAsync(string directoryPath)
	{
		if (!Directory.Exists(directoryPath)) return;

		Console.WriteLine($"Deleting folder: {directoryPath}");
		try
		{
			await Task.Run(() => Directory.Delete(directoryPath, true)).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred while deleting folder: {ex.Message}");
		}
	}

	public static async Task RestorePackages()
	{
		Process buildProcess = new()
		{
			StartInfo =
			{
				FileName = "dotnet.exe",
				Arguments = "restore",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				WorkingDirectory = Program.RootPath
			},
			EnableRaisingEvents = true
		};

		buildProcess.OutputDataReceived += (_, args) =>
		{
			if (!string.IsNullOrEmpty(args.Data)) Console.WriteLine(args.Data);
		};

		buildProcess.Start();
		buildProcess.BeginOutputReadLine();
		await buildProcess.WaitForExitAsync().ConfigureAwait(false);
	}


	public static bool CompileProject(string solutionFilePath)
	{
		string? workingDir = Path.GetDirectoryName(solutionFilePath);
		if (!Directory.Exists(workingDir))
		{
			Console.WriteLine($"Directory {workingDir} was not found.");
			return false;
		}

		Console.WriteLine($"----------------------------- COMPILING {Path.GetFileName(solutionFilePath)} -----------------------------");

		try
		{
			Process buildProcess = new()
			{
				StartInfo =
				{
					FileName = "MSBuild.exe",
					Arguments = $"\"{solutionFilePath}\" /p:Configuration=Release",
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

	public static void CopyFiles(string sourceFolderPath)
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
