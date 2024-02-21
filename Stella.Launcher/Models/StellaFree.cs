namespace StellaModLauncher.Models;

// Stella version & resources
public class LauncherData
{
	public string? Release { get; set; }
	public bool Beta { get; set; }
	public string? Date { get; set; }
}

public class PublicResourcesData
{
	public string? Release { get; set; }
	public bool Beta { get; set; }
	public string? Date { get; set; }
}

public class StellaApiVersion
{
	public bool Success { get; set; }
	public int Status { get; set; }
	public LauncherData? Launcher { get; set; }
	public PublicResourcesData? Resources { get; set; }
}

// Resources
public class LocalResources
{
	public string? Version { get; set; }
	public string? Date { get; set; }
}
