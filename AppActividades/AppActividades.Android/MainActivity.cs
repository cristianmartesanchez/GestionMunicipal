using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using Android.Content;
using System.Collections.Generic;
using Xamarin.Forms;
using Android.Database;
using Android.Provider;
using CarouselView.FormsPlugin.Droid;
using System.IO;
using Acr.UserDialogs;

namespace AppActividades.Droid
{
    [Activity(Label = "AppActividades", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //NuGet Initializations


            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            UserDialogs.Init(this);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}