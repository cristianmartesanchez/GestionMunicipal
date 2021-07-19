using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionMunicipal.Data
{
    public class Actividad
    {
        public string GUID { get; set; }
        public string Title { get; set; }
        public string Tipo_de_actividad { get; set; }
        public string Foto_Principal { get; set; }
        public string Texto_Descriptivo { get; set; }
        public decimal Logros1_Cantidad { get; set; }
        public string Logros1_UnidadMedida { get; set; }
        public decimal Logros2_Cantidad { get; set; }
        public string Logros2_UnidadMedida { get; set; }
        public string Foto1 { get; set; }
        public string Foto1_Pie { get; set; }
        public string Foto2 { get; set; }
        public string Foto2_Pie { get; set; }
        public string Foto3 { get; set; }
        public string Foto3_Pie { get; set; }

        internal static string imagen(ClientContext clientContext, string url)
        {
            throw new NotImplementedException();
        }
    }
}
