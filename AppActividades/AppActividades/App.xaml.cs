using AppActividades.Vistas;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Security;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppActividades
{
    public partial class App : Application
    {
        public static ClientContext _context { get; set; }
        public static SharePointConnect _sharePointConnect { get; set; }
        public static List<Actividad> _actividades { get; set; }
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDY4ODc2QDMxMzkyZTMyMmUzMFEwUHE3N1ROWS96Skh0dWRVYmlXRDZid1RHMG5DMFJYcWdXdVA0K2Q0cDA9");
            InitializeComponent();
            MainPage = new Login();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
