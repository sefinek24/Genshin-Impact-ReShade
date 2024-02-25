namespace PrepareStella.Models;

public class PublicResourcesData
{
	public string? Release { get; set; }
	public bool Beta { get; set; }
	public string? Date { get; set; }
}

public class StellaApiVersion
{
	// public bool Success { get; set; }
	// public int Status { get; set; }
	public PublicResourcesData? Resources { get; set; }
}
