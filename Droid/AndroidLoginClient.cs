using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using JudgementZone.Droid;
using JudgementZone.Interfaces;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;


//[assembly: Xamarin.Forms.Dependency(typeof(AndroidLoginClient))]
namespace JudgementZone.Droid
{
    public class AndroidLoginClient //: I_MobileClient
    {
        public AndroidLoginClient() { }

        //public async Task<MobileServiceUser> Authorize(MobileServiceAuthenticationProvider provider)
        //{
        //    return await App.Client.LoginAsync(Xamarin.Forms.Forms.Context, provider);
        //}
    }
}