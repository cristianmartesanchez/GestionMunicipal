using AppActividades.Vistas;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Security;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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
            AppCenter.Start("android=a8f152b9-8792-491d-bc5f-bc03e6152518;",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
