using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ScnViewGestures.Plugin.Forms.iOS.Renderers;
using UIKit;

namespace JudgementZone.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            ViewGesturesRenderer.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
