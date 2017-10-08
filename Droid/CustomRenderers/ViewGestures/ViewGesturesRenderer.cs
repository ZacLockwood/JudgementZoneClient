using System;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using ScnViewGestures.Plugin.Forms;
using ScnViewGestures.Plugin.Forms.Droid.Renderers;
using Android.Util;
using Android.Graphics;

[assembly: ExportRenderer(typeof(ViewGestures), typeof(ViewGesturesRenderer))]

namespace ScnViewGestures.Plugin.Forms.Droid.Renderers
{
    public class ViewGesturesRenderer : ViewRenderer
    {

        private ViewGestures _viewGesturesElement;
        private int rendererWidth = -1;
        private int rendererHeight = -1;
        private int rendererStatusBarHeight = 0;

        // Used for registration with dependency service
        public async static void Init()
        {
            var temp = DateTime.Now;
        }

        public ViewGesturesRenderer()
        {
            DisplayMetrics displaymetrics = Resources.DisplayMetrics;
            rendererWidth = displaymetrics.WidthPixels;
			rendererHeight = displaymetrics.HeightPixels;

            int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                rendererStatusBarHeight = Resources.GetDimensionPixelSize(resourceId);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            _viewGesturesElement = (ViewGestures)Element;

            if (e.NewElement == null)
            {
                this.Touch -= HandleTouch;
            }

            if (e.OldElement == null)
            {

                this.Touch += HandleTouch;
            }
        }

        void HandleTouch(object sender, TouchEventArgs e)
        {
            double x = e.Event.RawX / rendererWidth * _viewGesturesElement.Width;
            double y = (e.Event.RawY - rendererStatusBarHeight) / rendererHeight * _viewGesturesElement.Height;

            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    _viewGesturesElement.OnTouchBegan(x, y);
                    break;
                case MotionEventActions.Move:
                    _viewGesturesElement.OnDrag(x, y);
                    break;
                case MotionEventActions.Up:
                    _viewGesturesElement.OnTouchEnded(x, y);
                    break;
            }
        }
     }
}