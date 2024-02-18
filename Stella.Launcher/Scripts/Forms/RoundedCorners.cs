using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace StellaLauncher.Scripts.Forms
{
	internal static class RoundedCorners
	{
		public static void Form(Form form, int radius = 16)
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

		public static Image Picture(Image startImage, int cornerRadius, Color backgroundColor)
		{
			cornerRadius *= 2;
			Bitmap roundedImage = new Bitmap(startImage.Width, startImage.Height);
			Graphics g = Graphics.FromImage(roundedImage);
			g.Clear(backgroundColor);
			g.SmoothingMode = SmoothingMode.AntiAlias;

			Brush brush = new TextureBrush(startImage);
			GraphicsPath gp = new GraphicsPath();

			gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
			gp.AddArc(0 + roundedImage.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
			gp.AddArc(0 + roundedImage.Width - cornerRadius, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
			gp.AddArc(0, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
			gp.CloseFigure();

			g.FillPath(brush, gp);
			g.Dispose();
			brush.Dispose();
			return roundedImage;
		}
	}
}
