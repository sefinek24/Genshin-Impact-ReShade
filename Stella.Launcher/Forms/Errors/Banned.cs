using LibVLCSharp.Shared;

namespace StellaModLauncher.Forms.Errors;

public partial class Banned : Form
{
	private readonly LibVLC _libVlc;
	private readonly MediaPlayer _mp;

	public Banned()
	{
		InitializeComponent();

		_libVlc = new LibVLC();
		_mp = new MediaPlayer(_libVlc);
		videoView1.MediaPlayer = _mp;

		Program.Logger.Error("You are banned ):");
	}

	private void Banned_Load(object sender, EventArgs e)
	{
		string path = Path.Combine(Program.AppPath, "data", "videos", "banned.mov");
		if (File.Exists(path))
			_mp.Play(new Media(_libVlc, path));
		else
			Program.Logger.Warn($"File {path} was not found");
	}

	private void Banned_FormClosing(object sender, FormClosingEventArgs e)
	{
		Application.Exit();
	}
}
