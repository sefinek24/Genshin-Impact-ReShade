namespace StellaLauncher.Models
{
    public class LauncherData
    {
        public string Version { get; set; }
        public bool Beta { get; set; }
        public string Size { get; set; }
        public string ReleaseDate { get; set; }
    }

    public class StellaApiVersion
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public LauncherData Launcher { get; set; }
    }

    public class StellaResources
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
    }

    // Token
    public class VerifyToken
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public bool DeleteBenefits { get; set; }
        public bool DeleteToken { get; set; }
        public string Token { get; set; }
        public int TierId { get; set; }
    }

    // Benefits
    public class PatronResources
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
        public PatronResources Resources { get; set; }
    }

    public class BenefitVersions
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public ResourcesData Message { get; set; }
        public int Tier { get; set; }
    }

    // Version of benefits
    public class BenefitsJsonVersion
    {
        public string Version { get; set; }

        public string Date { get; set; }
        // public int Tier { get; set; }
    }
}
