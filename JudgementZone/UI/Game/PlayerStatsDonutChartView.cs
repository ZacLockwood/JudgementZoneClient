using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using JudgementZone.Models;

namespace JudgementZone.UI
{
	public class PlayerStatsDonutChartView : ContentView
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

		ChartData[] myAnswersChartData =
		{
            new ChartData(0, SKColor.Parse("#FF0000")),
            new ChartData(45, SKColor.Parse("#F6E202")),
            new ChartData(45, SKColor.Parse("#00D23E")),
            new ChartData(45, SKColor.Parse("#137AF9"))
		};

		ChartData[] theirAnswersChartData =
		{
			new ChartData(45, SKColor.Parse("#33FF0000")),
			new ChartData(45, SKColor.Parse("#33F6E202")),
			new ChartData(45, SKColor.Parse("#3300D23E")),
			new ChartData(45, SKColor.Parse("#33137AF9"))
		};

		public PlayerStatsDonutChartView(M_Client_PlayerGameStats donutStats)
        {
			myAnswersChartData = new ChartData[]
			{
				new ChartData(donutStats.PlayerSelectionsRed, SKColor.Parse("#FF0000")),
				new ChartData(donutStats.PlayerSelectionsYellow, SKColor.Parse("#F6E202")),
				new ChartData(donutStats.PlayerSelectionsGreen, SKColor.Parse("#00D23E")),
				new ChartData(donutStats.PlayerSelectionsBlue, SKColor.Parse("#137AF9"))
			};

			theirAnswersChartData = new ChartData[]
			{
				new ChartData(donutStats.OtherSelectionsRed, SKColor.Parse("#33FF0000")),
				new ChartData(donutStats.OtherSelectionsYellow, SKColor.Parse("#33F6E202")),
				new ChartData(donutStats.OtherSelectionsGreen, SKColor.Parse("#3300D23E")),
				new ChartData(donutStats.OtherSelectionsBlue, SKColor.Parse("#33137AF9"))
			};

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

			SKPoint center = new SKPoint(info.Width / 2.0f, info.Height / 2.0f);
			float radius = Math.Min(info.Width / 2.3f, info.Height / 2.3f);
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
					paint.StrokeWidth = 35;
					paint.Color = item.Color;

					canvas.Save();

					// Fill and stroke the path
					canvas.DrawPath(path, paint);
					canvas.Restore();
				}

				startAngle += sweepAngle;
			}

            startAngle = 180;

			totalValues = 0;

            foreach (ChartData item in theirAnswersChartData)
			{
				totalValues += item.Value;
			}

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
					paint.StrokeWidth = 35;
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