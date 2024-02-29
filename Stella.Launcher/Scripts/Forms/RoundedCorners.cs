using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SkiaSharp;

namespace StellaModLauncher.Scripts.Forms;

internal static class RoundedCorners
{
	public static void Form(Form form, int radius = 12)
	{
		ArgumentNullException.ThrowIfNull(form);

		GraphicsPath path = RoundedRectPath(new Rectangle(0, 0, form.Width, form.Height), radius);
		form.Region = new Region(path);
	}

	public static Bitmap Picture(Image? inputImage, int cornerRadius, int shadowOffsetX = 0, int shadowOffsetY = 0, int shadowRadius = 9)
	{
		SKBitmap skBitmap = ConvertToSkBitmap(inputImage);

		// Rozmiar cienia może wpłynąć na końcowy rozmiar obrazu, dlatego dodajemy przesunięcie/margines dla cienia
		SKBitmap outputBitmap = new(skBitmap.Width + shadowOffsetX + shadowRadius, skBitmap.Height + shadowOffsetY + shadowRadius);
		using (SKCanvas canvas = new(outputBitmap))
		{
			canvas.Clear(SKColors.Transparent);

			SKRect rect = new(0, 0, skBitmap.Width, skBitmap.Height);
			SKRoundRect roundRect = new(rect, cornerRadius, cornerRadius);

			SKPaint shadowPaint = new()
			{
				IsAntialias = true,
				Color = SKColors.Black.WithAlpha(100),
				ImageFilter = SKImageFilter.CreateDropShadow(shadowOffsetX, shadowOffsetY, shadowRadius, shadowRadius, SKColors.Black),
				FilterQuality = SKFilterQuality.High
			};

			canvas.DrawRoundRect(roundRect, shadowPaint);

			rect.Offset(shadowOffsetX, shadowOffsetY);

			SKPaint imagePaint = new()
			{
				IsAntialias = true,
				FilterQuality = SKFilterQuality.High
			};

			SKPath path = new();
			path.AddRoundRect(rect, cornerRadius, cornerRadius);
			canvas.ClipPath(path);

			canvas.DrawBitmap(skBitmap, shadowOffsetX, shadowOffsetY, imagePaint);
		}

		return ConvertToBitmap(outputBitmap);
	}

	private static SKBitmap ConvertToSkBitmap(Image? image)
	{
		using MemoryStream stream = new();
		image.Save(stream, ImageFormat.Png);
		stream.Seek(0, SeekOrigin.Begin);
		return SKBitmap.Decode(stream);
	}

	private static Bitmap ConvertToBitmap(SKBitmap skBitmap)
	{
		using SKImage skImage = SKImage.FromBitmap(skBitmap);
		using SKData data = skImage.Encode();
		using MemoryStream stream = new(data.ToArray());
		return new Bitmap(stream);
	}

	private static GraphicsPath RoundedRectPath(Rectangle bounds, int radius)
	{
		GraphicsPath path = new();
		int diameter = radius * 2;
		Rectangle arc = new(bounds.Left, bounds.Top, diameter, diameter);

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
