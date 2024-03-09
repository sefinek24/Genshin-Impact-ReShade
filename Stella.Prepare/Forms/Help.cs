namespace PrepareStella.Forms;

public sealed partial class Help : Form
{
	public Help()
	{
		InitializeComponent();

		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
		UpdateStyles();
	}
}
