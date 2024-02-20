using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace StellaLauncher.Scripts.Forms
{
	internal static class RoundedCorners
	{
		public static void Form(Form form, int radius = 12)
		{
			if (form == null) throw new ArgumentNullException(nameof(form));

			using (GraphicsPath path = RoundedRectPath(new Rectangle(0, 0, form.Width, form.Height), radius))
			{
				form.Region = new Region(path);
			}
		}

		public static Image Picture(Image startImage, int cornerRadius, Color backgroundColor)
		{
			cornerRadius *= 2;
			Bitmap roundedImage = new Bitmap(startImage.Width, startImage.Height);
			using (Graphics g = Graphics.FromImage(roundedImage))
			{
				g.Clear(backgroundColor);
				g.SmoothingMode = SmoothingMode.AntiAlias;

				using (Brush brush = new TextureBrush(startImage))
				using (GraphicsPath gp = RoundedRectPath(new Rectangle(0, 0, roundedImage.Width, roundedImage.Height), cornerRadius))
				{
					g.FillPath(brush, gp);
				}
			}

			return roundedImage;
		}

		private static GraphicsPath RoundedRectPath(Rectangle bounds, int radius)
		{
			GraphicsPath path = new GraphicsPath();
			int diameter = radius * 2;
			Rectangle arc = new Rectangle(bounds.Left, bounds.Top, diameter, diameter);

			path.AddArc(arc, 180, 90);

			arc.X = bounds.Right - diameter;
			path.AddArc(arc, 270, 90);

			arc.Y = bounds.Bottom - diameter;
			path.AddArc(arc, 0, 90);

			arc.X = bounds.Left;
			path.AddArc(arc, 90, 90);

			path.CloseFigure();
			return path;
		}
	}
}
