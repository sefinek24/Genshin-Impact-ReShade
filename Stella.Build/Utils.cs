using System.Diagnostics;

namespace BuildStellaMod;

internal static class Utils
{
	public static async Task DeleteDirectory(string directoryPath)
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

	public static void KillProcess(string processName)
	{
		foreach (Process process in Process.GetProcessesByName(processName))
			try
			{
				process.Kill();
				Console.WriteLine($"Process {process.ProcessName} (ID: {process.Id}) has been terminated");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to terminate {process.ProcessName} (ID: {process.Id}): {ex.Message}");
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
}
