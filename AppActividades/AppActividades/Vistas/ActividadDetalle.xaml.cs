using AppActividades.Services;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppActividades.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActividadDetalle : ContentPage
    {
        private Actividad _actividad = new Actividad();

        public ActividadDetalle(int actividadId)
        {
            InitializeComponent();
            this._actividad = App._actividades.FirstOrDefault(a => a.Id == actividadId);
            BindingContext = _actividad;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
            if (_actividad.Logros1_Cantidad <= 0 && _actividad.Logros2_Cantidad <= 0)
            {
                contentLogros.IsVisible = false;
            }
            BindingContext = _actividad;
        }


        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<ILodingPageService>().InitLoadingPage(new MainPage());
            DependencyService.Get<ILodingPageService>().ShowLoadingPage();
            await Navigation.PushAsync(new ActividadEditar(_actividad.Id));
            

        }
    }
}