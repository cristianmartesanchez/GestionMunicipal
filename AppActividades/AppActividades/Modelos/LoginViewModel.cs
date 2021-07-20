
using AppActividades.Services;
using AppActividades.Vistas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AppActividades.Modelos
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public Action DisplayInvalidLoginPrompt;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        Uri site = new Uri("https://innovaitiis.sharepoint.com/sites/Intranet2020/PorfolioModel/PYayuntamientos/Gdom");
        
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Email"));
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }
        public ICommand SubmitCommand { protected set; get; }
        public LoginViewModel()
        {
            SubmitCommand = new Command(async() => await OnSubmit());
        }
        public async Task OnSubmit()
        {

            DependencyService.Get<ILodingPageService>().InitLoadingPage(new MainPage());
            DependencyService.Get<ILodingPageService>().ShowLoadingPage();

            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                await App.Current.MainPage.DisplayAlert("", "Red interrumpida, verifique el estado de su red", "OK");
                DependencyService.Get<ILodingPageService>().HideLoadingPage();
            }
            else if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await App.Current.MainPage.DisplayAlert("Campos Vacios", "Ingrese correo electrónico y contraseña.", "OK");
                DependencyService.Get<ILodingPageService>().HideLoadingPage();
            }              
            else
            {
                try
                {



                    ///string user = "cmarte@innovaitiis.com.co";
                    SecureString passwordSecure = new SecureString();

                    foreach (char c in password.ToCharArray()) passwordSecure.AppendChar(c);

                    var context = new AuthenticationManager().GetContext(site, email, passwordSecure);
                    
                    context.Load(context.Web, p => p.Title);
                    await context.ExecuteQueryAsync();
                    App._context = context;
                    App._sharePointConnect = new SharePointConnect(context);
                    App._actividades = await App._sharePointConnect.GenerarDataAsync();

                    Application.Current.MainPage = new NavigationPage(new Actividades());

                }
                catch (Exception)
                {
                    await App.Current.MainPage.DisplayAlert("Login Fail", "Ingrese el correo electrónico y la contraseña correctos.", "OK");
                    DependencyService.Get<ILodingPageService>().HideLoadingPage();
                }

            }

        }
    }
}
