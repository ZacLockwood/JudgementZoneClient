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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ScnViewGestures.Plugin.Forms;
using ScnViewGestures.Plugin.Forms.iOS.Renderers;
using CoreGraphics;

[assembly: ExportRenderer(typeof(ViewGestures), typeof(ViewGesturesRenderer))]

namespace ScnViewGestures.Plugin.Forms.iOS.Renderers
{
    public class ViewGesturesRenderer : ViewRenderer
    {
        // Used for registration with dependency service
        public async static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            var viewGesture = (ViewGestures)Element;

            //var tapGestureRecognizer = new TapGestureRecognizer()
            //{
            //    OnTouchesBegan = ((x, y) => viewGesture.OnTouchBegan(x, y)),
            //    OnTouchesEnded = (() => viewGesture.OnTouchEnded()),
            //    OnTap = ((x, y) => viewGesture.OnTap(x, y)),
            //};

            //var longPressGestureRecognizer = new LongPressGestureRecognizer(() => viewGesture.OnLongTap())
            //{
            //    OnTouchesBegan = ((x, y) => viewGesture.OnTouchBegan(x, y)),
            //    OnTouchesEnded = (() => viewGesture.OnTouchEnded()),
            //};

            //#region SwipeGestureRecognizer
            //var swipeLeftGestureRecognizer = new SwipeGestureRecognizer(() => viewGesture.OnSwipeLeft())
            //{
            //    Direction = UISwipeGestureRecognizerDirection.Left,

            //    OnTouchesBegan = ((x, y) => viewGesture.OnTouchBegan(x, y)),
            //    OnTouchesEnded = (() => viewGesture.OnTouchEnded()),
            //};

            //var swipeRightGestureRecognizer = new SwipeGestureRecognizer(() => viewGesture.OnSwipeRight())
            //{
            //    Direction = UISwipeGestureRecognizerDirection.Right,

            //    OnTouchesBegan = ((x, y) => viewGesture.OnTouchBegan(x, y)),
            //    OnTouchesEnded = (() => viewGesture.OnTouchEnded()),
            //};

            //var swipeUpGestureRecognizer = new SwipeGestureRecognizer(() => viewGesture.OnSwipeUp())
            //{
            //    Direction = UISwipeGestureRecognizerDirection.Up,

            //    OnTouchesBegan = ((x, y) => viewGesture.OnTouchBegan(x, y)),
            //    OnTouchesEnded = (() => viewGesture.OnTouchEnded()),
            //};

            //var swipeDownGestureRecognizer = new SwipeGestureRecognizer(() => viewGesture.OnSwipeDown())
            //{
            //    Direction = UISwipeGestureRecognizerDirection.Down,

            //    OnTouchesBegan = ((x, y) => viewGesture.OnTouchBegan(x, y)),
            //    OnTouchesEnded = (() => viewGesture.OnTouchEnded()),
            //};
            //#endregion

            #region DragGestureRecognizer
            var dragGestureRecognizer = new DragGestureRecognizer()
            {
                OnTouchesBegan = ((x, y) => viewGesture.OnTouchBegan(x, y)),
                OnTouchesEnded = ((x, y) => viewGesture.OnTouchEnded(x, y)),
                OnDrag = ((x, y) => viewGesture.OnDrag(x, y)),
            };

            //from iOS Developer Library (Gesture Recognizers)
            //...For your view to recognize both swipes and pans, you want the swipe gesture recognizer to analyze the touch event before the pan gesture recognizer does...
            //if ((viewGesture.SupportGestures & ViewGestures.GestureType.gtSwipeLeft) != 0)
            //    dragGestureRecognizer.RequireGestureRecognizerToFail(swipeLeftGestureRecognizer);
            //if ((viewGesture.SupportGestures & ViewGestures.GestureType.gtSwipeRight) != 0)
            //    dragGestureRecognizer.RequireGestureRecognizerToFail(swipeRightGestureRecognizer);
            //if ((viewGesture.SupportGestures & ViewGestures.GestureType.gtSwipeUp) != 0)
            //    dragGestureRecognizer.RequireGestureRecognizerToFail(swipeUpGestureRecognizer);
            //if ((viewGesture.SupportGestures & ViewGestures.GestureType.gtSwipeDown) != 0)
                //dragGestureRecognizer.RequireGestureRecognizerToFail(swipeDownGestureRecognizer);
            #endregion

            if (e.NewElement == null)
            {
                //if (tapGestureRecognizer != null)
                //    this.RemoveGestureRecognizer(tapGestureRecognizer);

                //if (longPressGestureRecognizer != null)
                //    this.RemoveGestureRecognizer(longPressGestureRecognizer);

                //if (swipeLeftGestureRecognizer != null)
                //    this.RemoveGestureRecognizer(swipeLeftGestureRecognizer);

                //if (swipeRightGestureRecognizer != null)
                //    this.RemoveGestureRecognizer(swipeRightGestureRecognizer);

                //if (swipeUpGestureRecognizer != null)
                //    this.RemoveGestureRecognizer(swipeUpGestureRecognizer);

                //if (swipeDownGestureRecognizer != null)
                    //this.RemoveGestureRecognizer(swipeDownGestureRecognizer);

                if (dragGestureRecognizer != null)
                    this.RemoveGestureRecognizer(dragGestureRecognizer);
            }

            if (e.OldElement == null)
            {
                //this.AddGestureRecognizer(tapGestureRecognizer);
                //this.AddGestureRecognizer(longPressGestureRecognizer);
                //this.AddGestureRecognizer(swipeLeftGestureRecognizer);
                //this.AddGestureRecognizer(swipeRightGestureRecognizer);
                //this.AddGestureRecognizer(swipeUpGestureRecognizer);
                //this.AddGestureRecognizer(swipeDownGestureRecognizer);
                this.AddGestureRecognizer(dragGestureRecognizer);
            }
        }

        //class TapGestureRecognizer : UITapGestureRecognizer
        //{
        //    public override void TouchesBegan(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesBegan(touches, evt);
        //        UITouch touch = touches.AnyObject as UITouch;

        //        double positionX = -1;
        //        double positionY = -1;

        //        if (OnTap != null && touch != null)
        //        {
        //            positionX = (double)touch.PreviousLocationInView(View).X;
        //            positionY = (double)touch.PreviousLocationInView(View).Y;
        //            OnTap(positionX, positionY);
        //        }

        //        if (OnTouchesBegan != null)
        //            OnTouchesBegan(positionX, positionY);
        //    }

        //    public override void TouchesEnded(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesEnded(touches, evt);

        //        if (OnTouchesEnded != null)
        //            OnTouchesEnded();
        //    }

        //    /*public override void TouchesMoved(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesMoved(touches, evt);

        //        if (OnTouchesEnded != null)
        //            OnTouchesEnded();
        //    }*/

        //    public override void TouchesCancelled(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesCancelled(touches, evt);

        //        if (OnTouchesEnded != null)
        //            OnTouchesEnded();
        //    }

        //    public Action<double, double> OnTap;
        //    public Action<double, double> OnTouchesBegan;
        //    public Action OnTouchesEnded;
        //}

        //class LongPressGestureRecognizer : UILongPressGestureRecognizer
        //{
        //    public LongPressGestureRecognizer(Action action)
        //        : base(action)
        //    { }

        //    public override void TouchesBegan(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesBegan(touches, evt);

        //        UITouch touch = touches.AnyObject as UITouch;

        //        double positionX = -1;
        //        double positionY = -1;

        //        if (touch != null)
        //        {
        //            positionX = (double)touch.PreviousLocationInView(View).X;
        //            positionY = (double)touch.PreviousLocationInView(View).Y;
        //        }

        //        if (OnTouchesBegan != null)
        //            OnTouchesBegan(positionX, positionY);
        //    }

        //    public override void TouchesEnded(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesEnded(touches, evt);

        //        if (OnTouchesEnded != null)
        //            OnTouchesEnded();
        //    }

        //    /*public override void TouchesMoved(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesMoved(touches, evt);

        //        if (OnTouchesEnded != null)
        //            OnTouchesEnded();
        //    }*/

        //    public override void TouchesCancelled(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesCancelled(touches, evt);

        //        if (OnTouchesEnded != null)
        //            OnTouchesEnded();
        //    }

        //    public Action<double, double> OnTouchesBegan;
        //    public Action OnTouchesEnded;
        //}

        //class SwipeGestureRecognizer : UISwipeGestureRecognizer
        //{
        //    public SwipeGestureRecognizer(Action action)
        //        : base(action)
        //    { }

        //    public override void TouchesBegan(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesBegan(touches, evt);

        //        UITouch touch = touches.AnyObject as UITouch;

        //        double positionX = -1;
        //        double positionY = -1;

        //        if (touch != null)
        //        {
        //            positionX = (double)touch.PreviousLocationInView(View).X;
        //            positionY = (double)touch.PreviousLocationInView(View).Y;
        //        }

        //        if (OnTouchesBegan != null)
        //            OnTouchesBegan(positionX, positionY);
        //    }

        //    public override void TouchesEnded(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesEnded(touches, evt);

        //        if (OnTouchesEnded != null)
        //            OnTouchesEnded();
        //    }

        //    public override void TouchesCancelled(NSSet touches, UIEvent evt)
        //    {
        //        base.TouchesCancelled(touches, evt);

        //        if (OnTouchesEnded != null)
        //            OnTouchesEnded();
        //    }

        //    public Action<double, double> OnTouchesBegan;
        //    public Action OnTouchesEnded;
        //}

        class DragGestureRecognizer : UIPanGestureRecognizer
        {
            public override void TouchesBegan(NSSet touches, UIEvent evt)
            {
                base.TouchesBegan(touches, evt);

                UITouch touch = touches.AnyObject as UITouch;

                double positionX = -1;
                double positionY = -1;

                if (touch != null)
                {
                    positionX = (double)touch.PreviousLocationInView(View).X;
                    positionY = (double)touch.PreviousLocationInView(View).Y;
                }

                if (OnTouchesBegan != null)
                    OnTouchesBegan(positionX, positionY);
            }

            public override void TouchesEnded(NSSet touches, UIEvent evt)
            {
                base.TouchesEnded(touches, evt);

                UITouch touch = touches.AnyObject as UITouch;

				double positionX = -1;
				double positionY = -1;

				if (touch != null)
				{
					positionX = (double)touch.PreviousLocationInView(View).X;
					positionY = (double)touch.PreviousLocationInView(View).Y;
				}

                if (OnTouchesEnded != null)
                    OnTouchesEnded(touch.LocationInView(View).X, touch.LocationInView(View).Y);
            }

            public override void TouchesMoved(NSSet touches, UIEvent evt)
            {
                base.TouchesMoved(touches, evt);
                UITouch touch = touches.AnyObject as UITouch;

                if (OnDrag != null && touch != null)
                {
                    OnDrag(touch.LocationInView(View).X, touch.LocationInView(View).Y);
                }
            }

            public override void TouchesCancelled(NSSet touches, UIEvent evt)
            {
                base.TouchesCancelled(touches, evt);

                if (OnTouchesEnded != null)
                    OnTouchesEnded(-1,-1);
            }

            public Action<double, double> OnTouchesBegan;
            public Action<double, double> OnTouchesEnded;
            public Action<double, double> OnDrag;
        }
    }
}