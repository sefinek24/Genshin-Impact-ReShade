namespace StellaModLauncher.Models;

public class CategoryData
{
	public string Category { get; set; }
	public string Endpoint { get; set; }
}

public class SefinekApi
{
	public string Success { get; set; }
	public string Status { get; set; }
	public CategoryData Info { get; set; }
	public string Endpoint { get; set; }
	public string Message { get; set; }
}

public class ResultData
{
	public string Anime_name { get; set; }
	public string Source_url { get; set; }
	public string Url { get; set; }
}

public class NekosBest
{
	public List<ResultData> Results { get; set; }
}

public class PurrBot
{
	public string Link { get; set; }
}

public class NekoBot
{
	public string Message { get; set; }
	public string Color { get; set; }
}
