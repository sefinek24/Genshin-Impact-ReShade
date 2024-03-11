using System.Media;
using CliWrap.Builders;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts;
using StellaModLauncher.Scripts.Helpers;
using Timer = System.Windows.Forms.Timer;

namespace StellaModLauncher.Forms.Errors;

public partial class RussianCunt : Form
{
	private readonly Random _random = new();

	public RussianCunt()
	{
		InitializeComponent();

		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
		UpdateStyles();
	}

	private void WrongCountry_Shown(object sender, EventArgs e)
	{
		try
		{
			new SoundPlayer { SoundLocation = Path.Combine(Program.AppPath, "data", "sounds", "other", "kurwa.wav") }.Play();
		}
		catch (Exception ex)
		{
			Program.Logger.Error(ex.ToString());
		}

		string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		string filePath = Path.Combine(desktopPath, "hello uwu.txt");

		if (!File.Exists(filePath))
			File.WriteAllText(filePath,
				"Сначала стоит задуматься о смысле вашего существования. Это первый шаг, который вы должны предпринять, тщательно обдумав сделанные выборы, пройденный путь, цель и значение вашего существования, прежде чем переходить к дальнейшим действиям.\n\nПосле тщательного анализа своей жизни стоит обратить внимание на образование, особенно на изучение английского языка. Знание этого языка определенно пригодится в России и откроет двери в широкий мир. Вы познакомитесь с другими людьми, станете более открытыми для мира. Узнаете, что такое интеграция в общество. Ваша жизнь приобретет большую ценность. Это позволит вам перестать общаться только на одном языке, если человек, с которым вы разговариваете, из другой страны и не говорит свободно на вашем родном языке. Не бойтесь, вы не получите пощечину от Матери России, так как она не существует.\n\nДругой темой для размышления является значение веры в Бога. Вера в Бога имеет глубокие корни и может принести множество преимуществ индивидууму. Для многих людей вера в Бога дает чувство смысла, поддержки, надежды и моральных принципов. Верить в Бога может обеспечить утешение в трудные моменты, способствовать духовному развитию и поощрять к доброте, эмпатии и состраданию к другим.\n\nКроме того, погружение в ценности веры может привести к открытию более глубокого смысла в повседневной жизни. Вы можете получить понимание вопросов, касающихся природы человека, цели существования и морали. Именно вера в Бога может помочь вам найти внутреннюю гармонию и равновесие, которые помогут справляться с трудностями, встречающимися на вашем пути.\n\nОднако помните, что вера в Бога не исключает поиска знаний и развития своих навыков. Напротив, вы можете приобретать новые опыты, углублять научные открытия и развивать свои интеллектуальные способности, одновременно черпая из глубин своей веры. Именно сочетание духовности и интеллекта позволит вам лучше понять мир и приносить дополнительную ценность другим людям вокруг себя.\n\nТаким образом, если вы действительно хотите воспользоваться программой Stella Mod в России, помните о важных аспектах вашего существования. Обдумывайте свои выборы, развивайтесь интеллектуально и духовно, расширяйте свои горизонты. Матінка Росія була повією і повією вона залишиться! До біса її!");

		Timer randomSizeTimer = new() { Interval = 500 };
		randomSizeTimer.Tick += ChangeWindowSize;
		randomSizeTimer.Start();

		Timer closeAppTimer = new() { Interval = 34300 };
		closeAppTimer.Tick += (_, _) =>
		{
			_ = Cmd.Execute(new Cmd.CliWrap
			{
				App = "notepad.exe",
				WorkingDir = Program.AppPath,
				Arguments = new ArgumentsBuilder()
					.Add(filePath)
			});

			Close();
		};
		closeAppTimer.Start();

		BalloonTip.Show("slava poland kurwo", "axaxaxaxaxaxaxaaxaxaxaxaxaxaxaaxaxaxaxaxaxaxaaxaxaxaxaxaxaxa");

		Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));
	}

	private void WrongCountry_FormClosed(object sender, FormClosedEventArgs e)
	{
		Program.Logger.Info(string.Format(Resources.Main_ClosedForm_, Text));
	}

	private void ChangeWindowSize(object? sender, EventArgs e)
	{
		Width = _random.Next(200, 800);
		Height = _random.Next(200, 800);

		if (WindowState == FormWindowState.Maximized) return; // TODO

		Left = _random.Next(Screen.PrimaryScreen!.Bounds.Width - Width);
		Top = _random.Next(Screen.PrimaryScreen.Bounds.Height - Height);
	}
}
