using Acr.UserDialogs;
using AppActividades.Services;
using Microsoft.SharePoint.Client;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppActividades.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActividadEditar : ContentPage
    {
        private int contador = 0;
        private MediaFile _mediaFile;
        private MediaFile[] _mediaFileFotos = new MediaFile[3];
        private int actividadId = 0;
        private SharePointConnect sharePoint { get; set; }
        private ClientContext _context { get; set; }
        private Stream[] streamsFotos = new Stream[4];
        private Actividad _actividad;
        public ActividadEditar(int id)
        {
            InitializeComponent();
            actividadId = id;
            _context = App._context;
            //sharePoint = new SharePointConnect(_context);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
            _actividad = App._actividades.FirstOrDefault(a => a.Id == actividadId);

            _actividad.Foto1Url = string.IsNullOrEmpty(_actividad.Foto1Url) ? "plus.png" : _actividad.Foto1Url;
            _actividad.Foto2Url = string.IsNullOrEmpty(_actividad.Foto2Url) ? "plus.png" : _actividad.Foto2Url;
            _actividad.Foto3Url = string.IsNullOrEmpty(_actividad.Foto3Url) ? "plus.png" : _actividad.Foto3Url;

            BindingContext = _actividad;
            
        }


        public async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.None)
                {
                    await App.Current.MainPage.DisplayAlert("", "Red interrumpida, verifique el estado de su red", "OK");
                    return;
                }

                DependencyService.Get<ILodingPageService>().InitLoadingPage(new MainPage());
                DependencyService.Get<ILodingPageService>().ShowLoadingPage();
                var actividad = (Actividad)BindingContext;


                int i = 0;
                streamsFotos = new Stream[4];
                streamsFotos[i] = _mediaFile != null ? _mediaFile.GetStream() : null;
                foreach (var item in _mediaFileFotos)
                {
                    i++;
                    streamsFotos[i] = item != null ? item.GetStream() : null;                    
                }

                await App._sharePointConnect.ActualizarActividadAsync(actividad, streamsFotos);

                var newActividad = await App._sharePointConnect.GetActividadByIdAsync(actividad.Id);
                var oldActividad = App._actividades.FirstOrDefault(a => a.Id == actividad.Id);

                App._actividades.Remove(oldActividad);
                App._actividades.Add(newActividad);

                                
                await Navigation.PushAsync(new Actividades());
                DependencyService.Get<ILodingPageService>().HideLoadingPage();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error editando la actividad, inténtelo de nuevo.", "OK");
                DependencyService.Get<ILodingPageService>().HideLoadingPage();
            }
        }

        private async void btnSelectPic_Clicked(object sender, EventArgs e)
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
            else if (answer.Equals("Camara"))
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
                imageView.Source = ImageSource.FromStream(() => _mediaFile.GetStream());
        }

        private void btnAgregarLogros_Clicked(object sender, EventArgs e)
        {

        }

        private static void OnEditorTextChanged(object sender, TextChangedEventArgs args)
        {

            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x));

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }
        }

        private void btnAgregarFoto_Clicked(object sender, EventArgs e)
        {

            var hijos = ContentFoto.Children.Count;
            if (hijos < 6)
            {

                var foto = new Image() { ClassId = $"{contador}", HeightRequest = 150, WidthRequest = 150, Source = "plus.png" };
                var pie = new Editor { ClassId = "pieFoto", Placeholder = "Descripción de la foto", AutoSize = EditorAutoSizeOption.TextChanges };
                contador++;
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.NumberOfTapsRequired = 2;
                tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
                foto.GestureRecognizers.Add(tapGestureRecognizer);

                List<Xamarin.Forms.View> Children = new List<Xamarin.Forms.View>();
                Children.Add(foto);
                Children.Add(pie);

                Children.ForEach(a => ContentFoto.Children.Add(a, 0, hijos++));
            }

        }

        public async void CargarImagen(Image image, int index)
        {
            try
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
                else if (answer.Equals("Camara"))
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
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al cargar la imagen. "+ex.Message, "OK");
            }

        }

        private void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            var imageSender = (Image)sender;
            var index = int.Parse(imageSender.ClassId);
            CargarImagen(imageSender, index);
        }

    }
}