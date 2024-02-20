namespace StellaModLauncher.Models;

// Auth
public class VerifyToken
{
	public bool Success { get; set; }
	public int Status { get; set; }
	public string Message { get; set; }
	public bool DeleteBenefits { get; set; }
	public bool DeleteTokens { get; set; }
	public string Token { get; set; }
	public int TierId { get; set; }
	public string Username { get; set; }
	public string AvatarUrl { get; set; }
}

// Stella Mod Plus (remote)
public class StellaPlusBenefits
{
	public string Migoto { get; set; }
	public string Mods { get; set; }
	public string Addons { get; set; }
	public string Presets { get; set; }
	public string Shaders { get; set; }
	public string Cmd { get; set; }
}

public class ResourcesData
{
	public StellaPlusBenefits Resources { get; set; }
}

public class BenefitVersions
{
	public bool Success { get; set; }
	public int Status { get; set; }
	public ResourcesData Message { get; set; }
}

public class GetUpdateUrl
{
	public bool Success { get; set; }
	public int Status { get; set; }
	public string Request { get; set; }
	public string PreparedUrl { get; set; }
}

// Stella Mod Plus (local)
public class LocalBenefitsVersion
{
	public string Version { get; set; }
	public string Date { get; set; }
	public int Tier { get; set; }
}
