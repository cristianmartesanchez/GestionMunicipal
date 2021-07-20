using System;
using System.IO;
using Android.Content;
using Java.IO;
using Xamarin.Forms;
using System.Threading.Tasks;
using AppActividades;
using Plugin.CurrentActivity;
using Android.App;
using AndroidX.Core.Content;
using AppActividades.Services;

[assembly: Dependency(typeof(SaveAndroid))]

class SaveAndroid: ISave
{
    //Method to save document as a file in Android and view the saved document
    public async Task SaveAndView(string fileName, String contentType, MemoryStream stream)
    {

        try
        {

            string docPath = null;
            /*Android.OS.Environment.ExternalStorageDirectory.Path*/

            docPath = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments).Path;// + "/" + Android.OS.Environment.DirectoryDocuments;


            //Create directory and file 
            Java.IO.File myDir = new Java.IO.File(docPath, fileName);
            myDir.Mkdir();

            Java.IO.File file = new Java.IO.File(docPath, fileName);

            var p = file.Exists();
            //Remove if the file exists
            if (file.Exists()) file.Delete();


            //Write the stream into the file
            FileOutputStream outs = new FileOutputStream(file);
            outs.Write(stream.ToArray());

            outs.Flush();
            outs.Close();

            //Invoke the created file for viewing
            if (file.Exists())
            {
                //Android.Net.Uri path = Android.Net.Uri.FromFile(file);
                Android.Net.Uri path = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
                string mimeType = string.Empty;
                switch (extension.ToLower())
                {

                    case "doc":
                    case "docx":
                        mimeType = "application/msword";
                        break;
                    case "pdf":
                        mimeType = "application/pdf";
                        break;

                    default:
                        mimeType = "*/*";
                        break;
                }

                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(path, mimeType);
                intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                ((Activity)CrossCurrentActivity.Current.Activity).StartActivity(intent);
                DependencyService.Get<ILodingPageService>().HideLoadingPage();

            }
        }
        catch (Exception ex)
        {
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
            await App.Current.MainPage.DisplayAlert("", "No se encontro el archivo.", "OK");

        }


    }

}
