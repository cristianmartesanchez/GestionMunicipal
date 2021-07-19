using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Widget;
using AppActividades.Droid;
using AppActividades.Droid.Helpers;
using AppActividades.Services;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(MediaService))]
namespace AppActividades.Droid
{

    public class MediaService : Java.Lang.Object, IMediaService
    {
  //      public static int OPENGALLERYCODE = 3;
  //      public async Task OpenGallery()
  //      {
  //          try
  //          {
  //              var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
  //              if (status != PermissionStatus.Granted)
  //              {
  //                  if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
  //                  {
  //                      Toast.MakeText(Forms.Context, "Necesita permiso para acceder a las fotos.", ToastLength.Long).Show();
  //                  }

  //                  var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
  //                  status = results[Permission.Storage];
  //              }


  //              if (status == PermissionStatus.Granted)
  //              {
  //                  var imageIntent = new Intent(Intent.ActionPick);
  //                  imageIntent.SetType("image/*");
  //                  imageIntent.PutExtra(Intent.ExtraAllowMultiple, true);
  //                  imageIntent.SetAction(Intent.ActionGetContent);

  //                  ((Activity)CrossCurrentActivity.Current.Activity)
  //                      .StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), MainActivity.OPENGALLERYCODE);

  //                  Toast.MakeText(CrossCurrentActivity.Current.Activity, "Mantenga pulsado para seleccionar varias fotos.", ToastLength.Short).Show();
  //              }        
  //              else if (status != PermissionStatus.Unknown)
  //              {
  //                  Toast.MakeText(Forms.Context, "Permiso denegado", ToastLength.Long).Show();
  //              }
  //          }
  //          catch (Exception ex)
  //          {
  //              Console.WriteLine(ex.ToString());
  //              Toast.MakeText(CrossCurrentActivity.Current.Activity, "Error. Can not continue, try again.", ToastLength.Long).Show();
  //          }
  //      }

		//void IMediaService.ClearFileDirectory()
  //      {
  //          string directory;
  //          if ((int)Android.OS.Build.VERSION.SdkInt >= 29)
  //          {
  //              directory = new Java.IO.File(Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures), ImageHelpers.collectionName).Path.ToString();
  //          }
  //          else
  //          {
  //              directory = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), ImageHelpers.collectionName).Path.ToString();
  //          }

  //          if (Directory.Exists(directory))
  //          {
  //              var list = Directory.GetFiles(directory, "*");
  //              if (list.Length > 0)
  //              {
  //                  for (int i = 0; i < list.Length; i++)
  //                  {
  //                      File.Delete(list[i]);
  //                  }
  //              }
  //          }
  //      }

        /*
        Example of how to call ClearFileDirectory():
            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IMediaService>().ClearFileDirectory();
            }
            if (Device.RuntimePlatform == Device.iOS)
            {
                GMMultiImagePicker.Current.ClearFileDirectory();
            }
        */
    }
}