
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace AppActividades
{
    public class SharePointConnect
    {

        private string url = $"https://innovaitiis.sharepoint.com/sites/Intranet2020/PorfolioModel/PYayuntamientos/Gdom/Imgenes/";
        private ClientContext _clientContext;
        private List<string> imageneUrls = new List<string>();
        private string[] imageneUrls1 = new string[4];

        public SharePointConnect(ClientContext clientContext)
        {
            this._clientContext = clientContext;
        }

        private async Task<Stream> GetImageFileAsStreamAsync(string url)
        {

            try
            {
                var file = _clientContext.Web.GetFileByUrl(url);

                var fileInfo = file.OpenBinaryStream();
                _clientContext.Load(file);
                await _clientContext.ExecuteQueryAsync();

                return fileInfo.Value;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public string AppendToFile(Stream fileInfo)
        {
            try
            {
               

                //string path = $@"~\GestionMunicipal\AppActividades\AppActividades\ImageTemp";
                //var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), $"{Guid.NewGuid()}.jpg");
                var cacheFile = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, $"{Guid.NewGuid()}.jpg");
                using (var stream = fileInfo)
                {
                    using (FileStream fs = System.IO.File.Create(cacheFile))
                    {
                        byte[] info = GetImageStreamAsBytes(stream);
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }

                return cacheFile;

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private Stream GetImageStream(string imageUrl)
        {
            byte[] buffer = System.IO.File.ReadAllBytes(imageUrl);

            return new MemoryStream(buffer);
        }

        private byte[] GetImageStreamAsBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task<string> CargarImagen(Stream imagen)
        {
            try
            {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = GetImageStreamAsBytes(imagen);
                FileStream fs = imagen as FileStream;
                var imageName = Path.GetFileName(fs.Name);
                string name = string.Empty;
                if (fs != null)
                {
                    newFile.Url = imageName;//$"{Guid.NewGuid()}{Path.GetExtension(fs.Name)}";//Path.GetFileName(fs.Name);
                }
                else
                {
                    newFile.Url = $"{Guid.NewGuid()}.jpg";
                }                    

                List docs = _clientContext.Web.Lists.GetByTitle("Imágenes");
                CamlQuery query = CamlQuery.CreateAllItemsQuery();
                ListItemCollection items = docs.GetItems(query);

                _clientContext.Load(items);
                await _clientContext.ExecuteQueryAsync();

               

                var exiteImagen = items.FirstOrDefault(a => a["FileLeafRef"].ToString() == imageName);


                if(exiteImagen == null)
                {
                    Microsoft.SharePoint.Client.File uploadFile = docs.RootFolder.Files.Add(newFile);
                   
                    _clientContext.Load(uploadFile);
                    await _clientContext.ExecuteQueryAsync();
                    name = newFile.Url;
                }
                else
                {
                    name = exiteImagen["FileLeafRef"].ToString();
                }

                return name;

            }
            catch (Exception ex)
            {
                throw;
            }

        }


        private static async Task<Stream> GetStreamFromImageSourceAsync(StreamImageSource imageSource, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (imageSource.Stream != null)
            {
                return await imageSource.Stream(cancellationToken);
            }
            return null;
        }

        public async Task<string> CargarFotosAsync(ImageSource foto = null)
        {

             //string imageUrl = string.Empty;
             var stream = GetStreamFromImageSourceAsync((StreamImageSource)foto).Result;
            string imageUrl = await CargarImagen(stream);

            return imageUrl;
        }

        public async Task MapearCamposAsync(ListItem listItem, Actividad actividad, Stream[] streamFotos)
        {

            
            imageneUrls1 = new string[4];
            for (int i = 0; i < streamFotos.Length; i++)
            {
                var imageUrl = string.Empty;
                if (streamFotos[i] != null)
                {
                    imageUrl = await CargarImagen(streamFotos[i]);
                    imageneUrls1[i] = imageUrl;
                    //i++;
                }
            }

            if (imageneUrls1[0] != null)
            {
                var url = $"https://innovaitiis.sharepoint.com/sites/Intranet2020/PorfolioModel/PYayuntamientos/Gdom/Imgenes/{imageneUrls1[0]}";

                FieldUrlValue FotoPrincipal = new FieldUrlValue();
                FotoPrincipal.Url = url;
                FotoPrincipal.Description = url;
                listItem["Foto_x0020_Principal"] = FotoPrincipal;
            }

            if (imageneUrls1[1] != null)
            {
                var url = $"https://innovaitiis.sharepoint.com/sites/Intranet2020/PorfolioModel/PYayuntamientos/Gdom/Imgenes/{imageneUrls1[1]}";

                FieldUrlValue Foto1 = new FieldUrlValue();
                Foto1.Url = url;
                Foto1.Description = url;
                listItem["Foto1"] = Foto1;
            }

            if (imageneUrls1[2] != null)
            {

                url = $"https://innovaitiis.sharepoint.com/sites/Intranet2020/PorfolioModel/PYayuntamientos/Gdom/Imgenes/{imageneUrls1[2]}";

                FieldUrlValue Foto2 = new FieldUrlValue();
                Foto2.Url = url;
                Foto2.Description = url;
                listItem["Foto2"] = Foto2;
            }

            if (imageneUrls1[3] != null)
            {
                url = $"https://innovaitiis.sharepoint.com/sites/Intranet2020/PorfolioModel/PYayuntamientos/Gdom/Imgenes/{imageneUrls1[3]}";

                FieldUrlValue Foto3 = new FieldUrlValue();
                Foto3.Url = url;
                Foto3.Description = url;
                listItem["Foto3"] = Foto3;
            }


            listItem["Title"] = actividad.Title;
            listItem["Tipo_x0020_de_x0020_actividad"] = actividad.Tipo_de_actividad;
            listItem["Texto_x0020_Descriptivo"] = actividad.Texto_Descriptivo;
            listItem["Logros1_x002d_Cantidad"] = actividad.Logros1_Cantidad;
            listItem["Logros1_x002d_UnidadMedida"] = actividad.Logros1_UnidadMedida;
            listItem["Logros2_x002d_Cantidad"] = actividad.Logros2_Cantidad;
            listItem["Logros2_x002d_UnidadMedida"] = actividad.Logros2_UnidadMedida;
            listItem["Foto1_x002d_Pie"] = actividad.Foto1_Pie;
            listItem["Foto2_x002d_Pie"] = actividad.Foto2_Pie;
            listItem["Foto3_x002d_Pie"] = actividad.Foto3_Pie;


        }

        public async Task ActualizarActividadAsync(Actividad actividad, Stream[] streamFotos)
        {
            try
            {
                List lista = _clientContext.Web.Lists.GetByTitle("Actividades");
                ListItem listItem = lista.GetItemById(actividad.Id);

               await MapearCamposAsync(listItem, actividad, streamFotos);

                listItem.Update();
               await _clientContext.ExecuteQueryAsync();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<int> AgregarActividadAsync(Actividad actividad, Stream[] streamFotos)
        {
            try
            {
                List list = _clientContext.Web.Lists.GetByTitle("Actividades");
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem listItem = list.AddItem(itemCreateInfo);

                await MapearCamposAsync(listItem, actividad, streamFotos);

                listItem.Update();
                await _clientContext.ExecuteQueryAsync();
                int id = await GetLastItemIdAsync("Actividades");
               return id;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        private async Task<int> GetLastItemIdAsync(string lista)
        {

            var items = await GetActividadesAsync(lista);
            return items.OrderByDescending(a => a.Id).FirstOrDefault().Id;
        }

        private async Task<ListItemCollection> GetActividadesAsync(string lista)
        {
            // Get the SharePoint web  
            Web web = _clientContext.Web;

            List list = web.Lists.GetByTitle(lista);
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = list.GetItems(query);

            _clientContext.Load(items);
            await  _clientContext.ExecuteQueryAsync();

            return items;
        }

        public async Task<Actividad> GetActividadByIdAsync(int Id)
        {

            var actividades = await GetActividadesAsync("Actividades");
            
            var item = actividades.FirstOrDefault(a => a["ID"].ToString() == Id.ToString());


            var actividad = new Actividad
            {
                Id = int.Parse(item["ID"].ToString()),
                GUID = item["GUID"].ToString(),
                Title = item["Title"] != null ? item["Title"].ToString() : "",
                Tipo_de_actividad = item["Tipo_x0020_de_x0020_actividad"] != null ? item["Tipo_x0020_de_x0020_actividad"].ToString() : "",
                //FotoPrincipal = imagenPrincipal != null ? ImageSource.FromStream(() => imagenPrincipal) : null,
                Texto_Descriptivo = item["Texto_x0020_Descriptivo"] != null ? item["Texto_x0020_Descriptivo"].ToString() : "",
                Logros1_Cantidad = item["Logros1_x002d_Cantidad"] != null ? decimal.Parse(item["Logros1_x002d_Cantidad"].ToString()) : 0,
                Logros1_UnidadMedida = item["Logros1_x002d_UnidadMedida"] != null ? item["Logros1_x002d_UnidadMedida"].ToString() : "",
                Logros2_Cantidad = item["Logros2_x002d_Cantidad"] != null ? decimal.Parse(item["Logros2_x002d_Cantidad"].ToString()) : 0,
                Logros2_UnidadMedida = item["Logros2_x002d_UnidadMedida"] != null ? item["Logros2_x002d_UnidadMedida"].ToString() : "",
                //Foto1 = streamFoto1 != null ? ImageSource.FromStream(() => streamFoto1) : null,
                Foto1_Pie = item["Foto1_x002d_Pie"] != null ? item["Foto1_x002d_Pie"].ToString() : "",
                //Foto2 = streamFoto2 != null ? ImageSource.FromStream(() => streamFoto2) : null,
                Foto2_Pie = item["Foto2_x002d_Pie"] != null ? item["Foto2_x002d_Pie"].ToString() : "",
                //Foto3 = streamFoto3 != null ? ImageSource.FromStream(() => streamFoto3) : null,
                Foto3_Pie = item["Foto3_x002d_Pie"] != null ? item["Foto3_x002d_Pie"].ToString() : "" 
            };

            var fotoPrincipal = item["Foto_x0020_Principal"];
            var foto1 = item["Foto1"];
            var foto2 = item["Foto2"];
            var foto3 = item["Foto3"];

            Stream imagen = null;
            if (fotoPrincipal != null)
            {
                imagen = await GetImageFileAsStreamAsync(((FieldUrlValue)(fotoPrincipal)).Url);
                actividad.Foto_Principal = AppendToFile(imagen);
            }

            if (foto1 != null)
            {
                imagen = await GetImageFileAsStreamAsync(((FieldUrlValue)(foto1)).Url);
                actividad.Foto1Url = AppendToFile(imagen);
            }

            if (foto2 != null)
            {
                imagen = await GetImageFileAsStreamAsync(((FieldUrlValue)(foto2)).Url);
                actividad.Foto2Url = AppendToFile(imagen);
            }

            if (foto3 != null)
            {
                imagen = await GetImageFileAsStreamAsync(((FieldUrlValue)(foto3)).Url);
                actividad.Foto3Url = AppendToFile(imagen);
            }


                return actividad;
        }

        public async Task<List<Actividad>> GenerarDataAsync()
        {

            var items = await GetActividadesAsync("Actividades");

            var actividades = new List<Actividad>();
            foreach (ListItem listItem in items)
            {

                var fotoPrincipal = listItem["Foto_x0020_Principal"];
                var foto1 = listItem["Foto1"];
                var foto2 = listItem["Foto2"];
                var foto3 = listItem["Foto3"];
              

                var actividad = new Actividad
                {
                    Id = int.Parse(listItem["ID"].ToString()),
                    GUID = listItem["GUID"].ToString(),
                    Title = listItem["Title"] != null ? listItem["Title"].ToString(): "",
                    Tipo_de_actividad = listItem["Tipo_x0020_de_x0020_actividad"] != null? listItem["Tipo_x0020_de_x0020_actividad"].ToString() : "",
                    Texto_Descriptivo = listItem["Texto_x0020_Descriptivo"] != null ? listItem["Texto_x0020_Descriptivo"].ToString() : "",
                    //FotoPrincipal = imagenPrincipal != null ? ImageSource.FromStream(() => imagenPrincipal) : null,
                    Logros1_Cantidad = listItem["Logros1_x002d_Cantidad"] != null ? decimal.Parse(listItem["Logros1_x002d_Cantidad"].ToString()) : 0,
                    Logros1_UnidadMedida = listItem["Logros1_x002d_UnidadMedida"] != null ? listItem["Logros1_x002d_UnidadMedida"].ToString() : "",
                    Logros2_Cantidad = listItem["Logros2_x002d_Cantidad"] != null ? decimal.Parse(listItem["Logros2_x002d_Cantidad"].ToString()) : 0,
                    Logros2_UnidadMedida = listItem["Logros2_x002d_UnidadMedida"] != null ? listItem["Logros2_x002d_UnidadMedida"].ToString() : "",
                    Foto1_Pie = listItem["Foto1_x002d_Pie"] != null ? listItem["Foto1_x002d_Pie"].ToString() : "",
                    Foto2_Pie = listItem["Foto2_x002d_Pie"] != null ? listItem["Foto2_x002d_Pie"].ToString() : "",
                    Foto3_Pie = listItem["Foto3_x002d_Pie"] != null ? listItem["Foto3_x002d_Pie"].ToString() : ""
                };

                Stream imagen = null;
                if (fotoPrincipal != null)
                {
                    imagen = await GetImageFileAsStreamAsync(((FieldUrlValue)(fotoPrincipal)).Url);
                    actividad.Foto_Principal = AppendToFile(imagen);
                }

                if (foto1 != null)
                {
                    imagen = await GetImageFileAsStreamAsync(((FieldUrlValue)(foto1)).Url);
                    actividad.Foto1Url = AppendToFile(imagen);
                }

                if (foto2 != null)
                {
                    imagen = await GetImageFileAsStreamAsync(((FieldUrlValue)(foto2)).Url);
                    actividad.Foto2Url = AppendToFile(imagen);
                }

                if (foto3 != null)
                {
                    imagen = await GetImageFileAsStreamAsync(((FieldUrlValue)(foto3)).Url);
                    actividad.Foto3Url = AppendToFile(imagen);
                }

                actividades.Add(actividad);

            }

            return actividades;
        }
    }
}
