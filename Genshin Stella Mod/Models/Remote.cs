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
    public class GetToken
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }
}
