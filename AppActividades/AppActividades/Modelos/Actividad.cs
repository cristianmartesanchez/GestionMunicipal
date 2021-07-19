using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace AppActividades
{
    public class Actividad
    {
        public int Id { get; set; }
        public string GUID { get; set; }
        public string Title { get; set; }
        public string Tipo_de_actividad { get; set; }
        public string Foto_Principal { get; set; }
        public string Texto_Descriptivo { get; set; }
        public decimal Logros1_Cantidad { get; set; }
        public string Logros1_UnidadMedida { get; set; }
        public decimal Logros2_Cantidad { get; set; }
        public string Logros2_UnidadMedida { get; set; }
        public ImageSource Foto1 { get; set; }
        public string Foto1Url { get; set; }
        public string Foto1_Pie { get; set; }
        public ImageSource Foto2 { get; set; }
        public string Foto2Url { get; set; }
        public string Foto2_Pie { get; set; }
        public ImageSource Foto3 { get; set; }
        public string Foto3Url { get; set; }
        public string Foto3_Pie { get; set; }
        public ImageSource FotoPrincipal { get;  set; }
        public ImageSource[] Fotos { get; set; }

    }
}
