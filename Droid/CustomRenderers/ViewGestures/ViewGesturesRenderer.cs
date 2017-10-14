//The MIT License(MIT)

//Copyright(c) 2015 ScienceSoft Inc.

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using ScnViewGestures.Plugin.Forms;
using ScnViewGestures.Plugin.Forms.Droid.Renderers;
using Android.Util;

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