using AppActividades.Modelos;
using AppActividades.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppActividades.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        Uri site = new Uri("https://innovaitiis.sharepoint.com/sites/Intranet2020/PorfolioModel/PYayuntamientos/Gdom");
        public Login()
        {


            var vm = new LoginViewModel();
            this.BindingContext = vm;
            vm.DisplayInvalidLoginPrompt += () => DisplayAlert("Error", "Login invalido, intentar otra vez", "OK");
            InitializeComponent();

            Email.Completed += (object sender, EventArgs e) =>
            {
                Password.Focus();
            };

            Password.Completed += (object sender, EventArgs e) =>
            {
                vm.SubmitCommand.Execute(null);
            };

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();


        }

        private void Login_Clicked(object sender, EventArgs e)
        {


        }
    }
}