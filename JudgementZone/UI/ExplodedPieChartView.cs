using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace JudgementZone.UI
{
	public class ExplodedPieChartView : ContentView
	{
        // Helper class
		class ChartData
		{
			public ChartData(int value, SKColor color)
			{
				Value = value;
				Color = color;
			}

			public int Value { private set; get; }

			public SKColor Color { private set; get; }
		}

		// 
		ChartData[] myAnswersChartData =
		{
			new ChartData(45, SKColors.Red),
			new ChartData(13, SKColors.Green),
			new ChartData(27, SKColors.Blue),
			new ChartData(19, SKColors.Magenta),
			new ChartData(40, SKColors.Cyan),
			new ChartData(22, SKColors.Brown),
			new ChartData(29, SKColors.Gray)
		};

		ChartData[] theirAnswersChartData =
		{
			new ChartData(45, SKColors.Red),
			new ChartData(13, SKColors.Green),
			new ChartData(27, SKColors.Blue),
			new ChartData(19, SKColors.Magenta),
			new ChartData(40, SKColors.Cyan),
			new ChartData(22, SKColors.Brown),
			new ChartData(29, SKColors.Gray)
		};

		public ExplodedPieChartView()
		{
			//Title = "Exploded Pie Chart";

			SKCanvasView canvasView = new SKCanvasView();
			canvasView.PaintSurface += OnCanvasViewPaintSurface;
			Content = canvasView;
		}

		void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
		{
			SKImageInfo info = args.Info;
			SKSurface surface = args.Surface;
			SKCanvas canvas = surface.Canvas;

			canvas.Clear();

			int totalValues = 0;

			foreach (ChartData item in myAnswersChartData)
			{
				totalValues += item.Value;
			}

			SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);
			float radius = Math.Min(info.Width / 2, info.Height / 2);
			SKRect rect = new SKRect(center.X - radius, center.Y - radius,
									 center.X + radius, center.Y + radius);

			float startAngle = 0;

			foreach (ChartData item in myAnswersChartData)
			{
				float sweepAngle = 180f * item.Value / totalValues;

				using (SKPath path = new SKPath())
				using (SKPaint paint = new SKPaint())
				{
					// Sweep forward, then backward, to complete connection
					path.ArcTo(rect, startAngle, sweepAngle, true);
					path.ArcTo(rect, startAngle + sweepAngle, -sweepAngle, false);
					path.Close();

					// Stroke settings
					paint.Style = SKPaintStyle.Stroke;
					paint.StrokeWidth = 20;
					paint.Color = item.Color;

					canvas.Save();

					// Fill and stroke the path
					canvas.DrawPath(path, paint);
					canvas.Restore();
				}

				startAngle += sweepAngle;
			}

            startAngle = 180;

			foreach (ChartData item in theirAnswersChartData)
			{
				float sweepAngle = 180f * item.Value / totalValues;

				using (SKPath path = new SKPath())
				using (SKPaint paint = new SKPaint())
				{
					// Sweep forward, then backward, to complete connection
					path.ArcTo(rect, startAngle, sweepAngle, true);
					path.ArcTo(rect, startAngle + sweepAngle, -sweepAngle, false);
					path.Close();

					// Stroke settings
					paint.Style = SKPaintStyle.Stroke;
					paint.StrokeWidth = 20;
					paint.Color = item.Color;

					canvas.Save();

					// Fill and stroke the path
					canvas.DrawPath(path, paint);
					canvas.Restore();
				}

				startAngle += sweepAngle;
			}
		}
	}
}