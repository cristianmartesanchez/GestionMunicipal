using Acr.UserDialogs;
using AppActividades.Services;
using Java.IO;
using Java.Nio.Charset;
using Microsoft.SharePoint.Client;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppActividades.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AgregarActividad : ContentPage
    {
        public ClientContext _context { get; set; }
        private MediaFile _mediaFile;
        private MediaFile[] _mediaFileFotos = new MediaFile[3];
        Stream[] streamsFotos = new Stream[4];
        private SharePointConnect sharePoint;
        int contador = 0;

        public AgregarActividad()
        {
            InitializeComponent();
            
            _context = App._context;
            sharePoint = App._sharePointConnect;
            BindingContext = new Actividad();

            


        }


        public async void OnSaveButtonClicked(object sender, EventArgs e)
        {


            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                await App.Current.MainPage.DisplayAlert("", "Red interrumpida, verifique el estado de su red", "OK");
                return;
            }

            DependencyService.Get<ILodingPageService>().InitLoadingPage(new MainPage());
            DependencyService.Get<ILodingPageService>().ShowLoadingPage();
            var actividad = (Actividad)BindingContext;

            var logros = ContentLogros.Children.Where(a => a.ClassId == "Logros_Cantidad").Select(a => (Entry)a).ToArray();
            var medida = ContentLogros.Children.Where(a => a.ClassId == "Logros_UnidadMedida").Select(a => (Editor)a).ToArray();
            var descripcionImagen = ContentFoto.Children.Where(a => a.ClassId == "pieFoto").Select(a => (Editor)a).ToArray();

            actividad.Logros1_Cantidad = logros.Count() > 0 ? int.Parse(logros[0].Text) : 0;
            actividad.Logros2_Cantidad = logros.Count() > 1 ? int.Parse(logros[1].Text): 0;

            actividad.Logros1_UnidadMedida = logros.Count() > 0 ? medida[0].Text : "";
            actividad.Logros2_UnidadMedida = logros.Count() > 1 ? medida[1].Text : "";

            actividad.Foto1_Pie = descripcionImagen.Count() > 0 ? descripcionImagen[0].Text : "";
            actividad.Foto2_Pie = descripcionImagen.Count() > 1 ? descripcionImagen[1].Text : "";
            actividad.Foto3_Pie = descripcionImagen.Count() > 2 ? descripcionImagen[2].Text : "";

            if (string.IsNullOrWhiteSpace(actividad.Title) || string.IsNullOrWhiteSpace(actividad.Tipo_de_actividad))
            {
                
                await DisplayAlert("Alert", "Debe completar el Titulo y Tipo de actividad", "OK");
                DependencyService.Get<ILodingPageService>().HideLoadingPage();
            }
            else if(_mediaFile == null)
            {
                
                await DisplayAlert("Alert", "Debe seleccionar la imagen principal", "OK");
                DependencyService.Get<ILodingPageService>().HideLoadingPage();
            }
            else
            {
                try
                {
                    int i = 0;
                    streamsFotos = new Stream[4];
                    streamsFotos[i] = _mediaFile != null ? _mediaFile.GetStream() : null;
                    foreach (var item in _mediaFileFotos)
                    {
                        i++;
                        streamsFotos[i] = item != null ? item.GetStream() : null;
                    }

                    int result = await sharePoint.AgregarActividadAsync(actividad, streamsFotos);

                    if (result > 0)
                    {
                        await DisplayAlert("Alert", "Acción realizada correctamente.", "OK");
                        App._actividades.Add(await App._sharePointConnect.GetActividadByIdAsync(result));
                        DependencyService.Get<ILodingPageService>().HideLoadingPage();
                        await Navigation.PopAsync(true);
                        await Navigation.PushAsync(new Actividades());
                    }
                    else
                    {
                        DependencyService.Get<ILodingPageService>().HideLoadingPage();
                        await DisplayAlert("Error", "Ocurrió un error guardado la actividad, inténtelo de nuevo.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    DependencyService.Get<ILodingPageService>().HideLoadingPage();
                    await DisplayAlert("Error", "Ocurrió un error guardado la actividad, inténtelo de nuevo.", "OK");
                }



            }
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
        }

        private static void OnEditorTextChanged(object sender, TextChangedEventArgs args)
        {

            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x));

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }
        }
       
        private void btnAgregarLogros_Clicked(object sender, EventArgs e)
        {

            var hijos = ContentLogros.Children.Count;
            if(hijos < 4)
            {

                var logro = new Entry() { ClassId= "Logros_Cantidad", Placeholder = "Logros", HeightRequest = 50 };
                var medida = new Editor { ClassId = "Logros_UnidadMedida", Placeholder = "Logros Unidad de medida", AutoSize = EditorAutoSizeOption.TextChanges };

                logro.TextChanged += OnEditorTextChanged;

                List<Xamarin.Forms.View> Children = new List<Xamarin.Forms.View>();
                Children.Add(logro);
                Children.Add(medida);

                Children.ForEach(a => ContentLogros.Children.Add(a, 0, hijos++));
            }

        }

        private void btnAgregarFoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var hijos = ContentFoto.Children.Count;
                if (hijos < 6)
                {

                    var foto = new Image() { ClassId = $"{contador}", HeightRequest = 150, WidthRequest = 150, Source = "plus.png" };
                    var pie = new Editor { ClassId = "pieFoto", Placeholder = "Descripción de la foto", AutoSize = EditorAutoSizeOption.TextChanges };
                    contador++;
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.NumberOfTapsRequired = 2;
                    tapGestureRecognizer.Tapped += OnTapGestureRecognizerTappedAsync;
                    foto.GestureRecognizers.Add(tapGestureRecognizer);

                    List<Xamarin.Forms.View> Children = new List<Xamarin.Forms.View>();
                    Children.Add(foto);
                    Children.Add(pie);

                    Children.ForEach(a => ContentFoto.Children.Add(a, 0, hijos++));
                }
            }
            catch (Exception)
            {

                
            }


        }

        public async void CargarImagen(Image image, int index)
        {

            var answer = await UserDialogs.Instance.ActionSheetAsync("Elija cómo obtener la imagen", "Cancelar", null, null, "Camara", "Galeria");


            if (string.IsNullOrWhiteSpace(answer))
                return;
            await CrossMedia.Current.Initialize();
            if (answer.Equals("Galeria"))
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Error", "Archivo no compatible.", "OK");
                    return;
                }
                else
                {
                    var mediaOption = new PickMediaOptions()
                    {
                        PhotoSize = PhotoSize.Medium
                    };

                    _mediaFileFotos[index] = await CrossMedia.Current.PickPhotoAsync();
                    if (_mediaFileFotos[index] == null) return;
                    
                }
            }
            else if(answer.Equals("Camara"))
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("Cámara", "No hay cámara disponible.", "OK");
                    return;
                }

                var mediaOption = new StoreCameraMediaOptions()
                {
                    PhotoSize = PhotoSize.Small,
                    Directory = "GestionMunicipal",
                    Name = "Foto.jpg",
                    SaveToAlbum = true,


                };

                _mediaFileFotos[index] = await CrossMedia.Current.TakePhotoAsync(mediaOption);

                if (_mediaFileFotos[index] == null)
                    return;
            }

            if (_mediaFileFotos[index] != null)
                image.Source = ImageSource.FromStream(() => _mediaFileFotos[index].GetStream());

        }

        private  void OnTapGestureRecognizerTappedAsync(object sender, EventArgs args)
        {
            try
            {

                 var imageSender = (Image)sender;
                var index = int.Parse(imageSender.ClassId);
                CargarImagen(imageSender, index);
            }
            catch (Exception)
            {
                DisplayAlert("Error", "Error al seleccionar la imagen.","OK");
                DependencyService.Get<ILodingPageService>().HideLoadingPage();
            }

        }

        private async void btnSelectPic_Clicked(object sender, EventArgs e)
        {

            try
            {

                var answer = await UserDialogs.Instance.ActionSheetAsync("Elija cómo obtener la imagen", "Cancelar", null, null, "Camara", "Galeria");


                if (string.IsNullOrWhiteSpace(answer))
                    return;

                await CrossMedia.Current.Initialize();

                _mediaFile = null;
                if (answer.Equals("Galeria"))
                {

                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        DependencyService.Get<ILodingPageService>().HideLoadingPage();
                        await DisplayAlert("Error", "Esto no es compatible con su dispositivo.", "OK");
                        return;
                    }

                    var mediaOption = new PickMediaOptions()
                    {
                        PhotoSize = PhotoSize.Medium
                    };

                    _mediaFile = await CrossMedia.Current.PickPhotoAsync();

                    if (_mediaFile == null) 
                        return;

                    
                }
                else if(answer.Equals("Camara"))
                {

                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("Cámara", "No hay cámara disponible.", "OK");
                        return;
                    }

                    var mediaOption = new StoreCameraMediaOptions()
                    {
                        PhotoSize = PhotoSize.Small,
                        Directory = "GestionMunicipal",
                        Name = "ImagenPrincipal.jpg",
                        SaveToAlbum = true,
                       
                       
                    };

                    _mediaFile = await CrossMedia.Current.TakePhotoAsync(mediaOption);

                    if (_mediaFile == null)
                        return;

                }

                if (_mediaFile != null)
                    imageView.Source = ImageSource.FromStream(() =>  _mediaFile.GetStream() );
              

            }
            catch (Exception ex)
            {
               
                await DisplayAlert("Error", "Error al seleccionar la imagen.", "OK");
                DependencyService.Get<ILodingPageService>().HideLoadingPage();
            }


        }



        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
        }

    }
}