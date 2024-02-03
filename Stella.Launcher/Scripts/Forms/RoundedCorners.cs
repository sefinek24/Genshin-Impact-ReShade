using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace StellaLauncher.Scripts.Forms
{
	internal static class RoundedCorners
	{
		public static void Apply(Form form, int radius = 16)
		{
			if (form == null) throw new ArgumentNullException(nameof(form));

			GraphicsPath path = new GraphicsPath();
			path.AddArc(0, 0, radius, radius, 180, 90);
			path.AddArc(form.Width - radius - 1, 0, radius, radius, 270, 90);
			path.AddArc(form.Width - radius - 1, form.Height - radius - 1, radius, radius, 0, 90);
			path.AddArc(0, form.Height - radius - 1, radius, radius, 90, 90);
			path.CloseFigure();

			form.Region = new Region(path);
		}
	}
}
