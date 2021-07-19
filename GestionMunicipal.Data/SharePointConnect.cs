
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace GestionMunicipal.Data
{
    public class SharePointConnect
    {
        public static byte[] imagen(ClientContext clientContext, string url)
        {

            //FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, url.Replace("https://innovaitiis.sharepoint.com", ""));

            //using (var memory = new MemoryStream())
            //{
            //    byte[] buffer = new byte[1024 * 64];
            //    int nread = 0;
            //    while ((nread = fileInfo.Stream.Read(buffer, 0, buffer.Length)) > 0)
            //    {
            //        memory.Write(buffer, 0, nread);
            //    }
            return new byte[0];
            //}

        }

        public static List<Actividad> GenerarData(ClientContext clientContext)
        {

                // Get the SharePoint web  
                Web web = clientContext.Web;

                List list = web.Lists.GetByTitle("Actividades");
                CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
                ListItemCollection items = list.GetItems(query);

                clientContext.Load(items);
                clientContext.ExecuteQuery();

                var actividades = new List<Actividad>();
                foreach (ListItem listItem in items)
                {


                    var actividad = new Actividad
                    {
                        GUID = listItem["GUID"].ToString(),
                        Title = listItem["Title"].ToString(),
                        Tipo_de_actividad = listItem["Tipo_x0020_de_x0020_actividad"].ToString(),
                        Foto_Principal = Actividad.imagen(clientContext, ((FieldUrlValue)(listItem["Foto_x0020_Principal"])).Url),
                        Texto_Descriptivo = listItem["Texto_x0020_Descriptivo"].ToString(),
                        Logros1_Cantidad = decimal.Parse(listItem["Logros1_x002d_Cantidad"].ToString()),
                        Logros1_UnidadMedida = listItem["Logros1_x002d_UnidadMedida"].ToString(),
                        Logros2_Cantidad = decimal.Parse(listItem["Logros2_x002d_Cantidad"].ToString()),
                        Logros2_UnidadMedida = listItem["Logros2_x002d_UnidadMedida"].ToString(),
                        Foto1 = Actividad.imagen(clientContext, ((FieldUrlValue)(listItem["Foto1"])).Url),
                        Foto1_Pie = listItem["Foto1_x002d_Pie"].ToString(),
                        Foto2 = Actividad.imagen(clientContext, ((FieldUrlValue)(listItem["Foto2"])).Url),
                        Foto2_Pie = listItem["Foto2_x002d_Pie"].ToString(),
                        Foto3 = Actividad.imagen(clientContext, ((FieldUrlValue)(listItem["Foto3"])).Url),
                        Foto3_Pie = listItem["Foto3_x002d_Pie"].ToString()
                    };

                    actividades.Add(actividad);

                }

                return actividades;
            }
        }
    
}
