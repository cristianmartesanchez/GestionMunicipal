
using AppActividades.Services;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;

namespace AppActividades.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Actividades : ContentPage
    {
        public ClientContext _context { get; set; }
        private SharePointConnect sharePoint;
        private List<Actividad> _actividades;

        public Actividades()
        {
            InitializeComponent();
            _actividades = App._actividades;
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            _actividades = App._actividades;
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                App.Current.MainPage.DisplayAlert("", "Red interrumpida, verifique el estado de su red", "OK");
                return;
            }            
            collectionView.ScrollTo(_actividades, animate: false, position: ScrollToPosition.Center);
            collectionView.ItemsSource = _actividades.OrderByDescending(a => a.Id);
            DependencyService.Get<ILodingPageService>().HideLoadingPage();

        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            DependencyService.Get<ILodingPageService>().InitLoadingPage(new MainPage());
            DependencyService.Get<ILodingPageService>().ShowLoadingPage();

            try
            {
               
                var textSearchBar = searchBar.Text.ToUpper();
                var result = _actividades.Where(a => a.Title.ToUpper().Contains(textSearchBar) ||
                a.Tipo_de_actividad.ToUpper().Contains(textSearchBar) || 
                a.Texto_Descriptivo.ToUpper().Contains(textSearchBar)).OrderByDescending(a => a.Id).ToList();

                if (result != null)
                {
                    collectionView.ScrollTo(result, animate: false, position: ScrollToPosition.Center);
                    collectionView.ItemsSource = result;
                }
            }
            catch (Exception ex)
            {

                
            }
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            DependencyService.Get<ILodingPageService>().InitLoadingPage(new MainPage());
            DependencyService.Get<ILodingPageService>().ShowLoadingPage();
            await Navigation.PushAsync(new AgregarActividad(),true);
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
        }


        private async void OnPrintClicked(object sender, EventArgs e)
        {
            try
            {
                DependencyService.Get<ILodingPageService>().InitLoadingPage(new MainPage());
                DependencyService.Get<ILodingPageService>().ShowLoadingPage();

                var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

                if (status == PermissionStatus.Granted)
                {
                    await PrintDoc();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", "No tiene permiso para realizar esta acción.", "OK");
                }
             

            }
            catch (Exception)
            {

                await App.Current.MainPage.DisplayAlert("", "Red interrumpida, verifique el estado de su red", "OK");
            }
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
        }


        private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                await App.Current.MainPage.DisplayAlert("", "Red interrumpida, verifique el estado de su red", "OK");
                return;
            }

            DependencyService.Get<ILodingPageService>().InitLoadingPage(new MainPage());
            DependencyService.Get<ILodingPageService>().ShowLoadingPage();
            int id = (e.CurrentSelection.FirstOrDefault() as Actividad).Id;

            await Navigation.PushAsync(new ActividadDetalle(id), true);
            DependencyService.Get<ILodingPageService>().HideLoadingPage();
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
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

        public async Task PrintDoc()
        {
            //"App" is the class of Portable project
            Assembly assembly = typeof(App).GetTypeInfo().Assembly;

            var p = assembly.GetManifestResourceNames();

            var documento = assembly.GetManifestResourceStream("AppActividades.MEMORIAANUAL.docx");
            //Creating a new document
            WordDocument document = new WordDocument(documento, FormatType.Automatic);

            if (_actividades.Count > 0)
            {


                foreach (var item in _actividades)
                {

                    document.Background.Color = Syncfusion.Drawing.Color.White;
                    WSection section = document.AddSection() as WSection;
                    section.PageSetup.InsertPageNumbers(false, PageNumberAlignment.Center);
                    section.PageSetup.Margins = new MarginsF(59f, 29f, 59f, 29f);
                    section.PageSetup.HeaderDistance = 0;
                    section.PageSetup.FooterDistance = 0;
                    IWParagraph paragraph = section.HeadersFooters.Header.AddParagraph();

                    //Titulo
                    paragraph = section.AddParagraph();
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    WTextRange textRange = paragraph.AppendText(item.Title) as WTextRange;
                    textRange.CharacterFormat.FontSize = 24f;
                    textRange.CharacterFormat.FontName = "Century Gothic (Headings)";
                    textRange.CharacterFormat.Bold = true;
                    textRange.CharacterFormat.TextColor = Syncfusion.Drawing.Color.DarkBlue;
                    
                    //Tipo de actividad
                    paragraph = section.AddParagraph();

                    Shape shape = paragraph.AppendShape(AutoShapeType.SnipSingleCornerRectangle,250,30);
                    shape.HorizontalAlignment = ShapeHorizontalAlignment.Left;
                    shape.FillFormat.Color = Syncfusion.Drawing.Color.DarkBlue;
                    shape.LineFormat.Line = false;
                    shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.TopAndBottom;
                    
                    paragraph = shape.TextBody.AddParagraph() as WParagraph;
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    textRange = paragraph.AppendText(item.Tipo_de_actividad) as WTextRange;
                    textRange.CharacterFormat.FontSize = 14f;
                    textRange.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                    textRange.CharacterFormat.Bold = false;
                    textRange.CharacterFormat.TextColor = Syncfusion.Drawing.Color.White;

                    //Imagen principal
                    byte[] buffer = System.IO.File.ReadAllBytes(item.Foto_Principal);
                    Stream imageStream1 = new MemoryStream(buffer);
                    paragraph = section.AddParagraph();
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    IWPicture picture = paragraph.AppendPicture(imageStream1);
                    picture.TextWrappingStyle = TextWrappingStyle.Inline;
                    picture.Width = 470;
                    picture.Height = 300;


                    //Texto descriptivo
                    paragraph = section.AddParagraph();
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    textRange = paragraph.AppendText(item.Texto_Descriptivo) as WTextRange;
                    textRange.CharacterFormat.FontSize = 12;
                    textRange.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                    textRange.CharacterFormat.Bold = false;
                    textRange.CharacterFormat.TextColor = Syncfusion.Drawing.Color.Black;


                    //Logros

                    //Rectangulo
                    paragraph = section.AddParagraph();
                    shape =  paragraph.AppendShape(AutoShapeType.Rectangle,450,150);
                    shape.VerticalAlignment = ShapeVerticalAlignment.Center;
                    shape.HorizontalAlignment = ShapeHorizontalAlignment.Center;
                    shape.FillFormat.Color = Syncfusion.Drawing.Color.DarkBlue;
                    shape.LineFormat.Line = false;
                    shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.InFrontOfText;

                    //Circulos                   
                    paragraph = document.LastParagraph;
                    Shape shapeCirculo = paragraph.AppendShape(AutoShapeType.FlowChartConnector,80,80);
                    paragraph = shapeCirculo.TextBody.AddParagraph() as WParagraph;
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                    IWTextRange text = paragraph.AppendText(item.Logros1_Cantidad.ToString());
                    text.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                    text.CharacterFormat.FontSize = 12;
                    text.CharacterFormat.Bold = true;
                    text.CharacterFormat.TextColor = Syncfusion.Drawing.Color.White;
                    shapeCirculo.FillFormat.Fill = false;
                    shapeCirculo.LineFormat.Color = Syncfusion.Drawing.Color.White;
                    shapeCirculo.LineFormat.Weight = 2;
                    shapeCirculo.WrapFormat.TextWrappingStyle = TextWrappingStyle.Tight;
                    shapeCirculo.WrapFormat.AllowOverlap = true;
                    shapeCirculo.HorizontalPosition = 72.5f;
                    shapeCirculo.VerticalPosition = 10;

                    paragraph = document.LastParagraph;
                    Shape unidadMedida1 = paragraph.AppendShape(AutoShapeType.Rectangle,200, 50);
                    unidadMedida1.FillFormat.Fill = false;
                    unidadMedida1.LineFormat.Line = false;
                    unidadMedida1.WrapFormat.TextWrappingStyle = TextWrappingStyle.Tight;
                    unidadMedida1.VerticalPosition = 100;
                    unidadMedida1.HorizontalPosition = 12.5f;
                    unidadMedida1.WrapFormat.AllowOverlap = true;

                    paragraph = unidadMedida1.TextBody.AddParagraph() as WParagraph;
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                    text = paragraph.AppendText(item.Logros1_UnidadMedida);
                    text.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                    text.CharacterFormat.FontSize = 11;
                    text.CharacterFormat.Bold = false;
                    text.CharacterFormat.TextColor = Syncfusion.Drawing.Color.White;

                    //Circulos 2                  
                    paragraph = document.LastParagraph;
                    Shape shapeCirculo2 = paragraph.AppendShape(AutoShapeType.FlowChartConnector,80,80);
                    paragraph = shapeCirculo2.TextBody.AddParagraph() as WParagraph;
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                    
                    text = paragraph.AppendText(item.Logros2_Cantidad.ToString());
                    text.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                    text.CharacterFormat.FontSize = 12;
                    text.CharacterFormat.Bold = true;
                    text.CharacterFormat.TextColor = Syncfusion.Drawing.Color.White;
                    shapeCirculo2.FillFormat.Fill = false;
                    shapeCirculo2.LineFormat.Color = Syncfusion.Drawing.Color.White;
                    shapeCirculo2.LineFormat.Weight = 2;
                    shapeCirculo2.WrapFormat.TextWrappingStyle = TextWrappingStyle.Tight;
                    shapeCirculo2.WrapFormat.AllowOverlap = true;
                    shapeCirculo2.HorizontalPosition = 297.5f;
                    shapeCirculo2.VerticalPosition = 10;


                    paragraph = document.LastParagraph;
                    Shape unidadMedida2 = paragraph.AppendShape(AutoShapeType.Rectangle, 200, 50);
                    unidadMedida2.FillFormat.Fill = false;
                    unidadMedida2.LineFormat.Line = false;
                    unidadMedida2.WrapFormat.TextWrappingStyle = TextWrappingStyle.Tight;
                    unidadMedida2.VerticalPosition = 100;
                    unidadMedida2.HorizontalPosition = 237.5f;
                    unidadMedida2.WrapFormat.AllowOverlap = true;


                    paragraph = unidadMedida2.TextBody.AddParagraph() as WParagraph;
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                    text = paragraph.AppendText(item.Logros2_UnidadMedida);
                    text.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                    text.CharacterFormat.FontSize = 11;
                    text.CharacterFormat.Bold = false;
                    text.CharacterFormat.TextColor = Syncfusion.Drawing.Color.White;


                    //Nueva Pagina
                    section = document.AddSection() as WSection;
                    section.PageSetup.InsertPageNumbers(false, PageNumberAlignment.Center);
                    section.PageSetup.Margins = new MarginsF(59f, 29f, 59f, 29f);
                    section.PageSetup.HeaderDistance = 75;
                    section.PageSetup.FooterDistance = 0;
                    

                    //Tabla
                    IWTable table = section.AddTable();
                    table.TableFormat.HorizontalAlignment = RowAlignment.Center;
                    table.TableFormat.Borders.BorderType = BorderStyle.None;
                    table.TableFormat.CellSpacing = 1;
                    //table.ResetCells(3, 2);


                    WTableRow row = null;
                    WTableCell cell = null;
                    if (!string.IsNullOrEmpty(item.Foto1Url) || !string.IsNullOrEmpty(item.Foto1_Pie))
                    {
                        //Primera fila
                        row = table.AddRow();
                        row.RowFormat.BackColor = Syncfusion.Drawing.Color.LightGray;

                        //Imagen                    
                        buffer = System.IO.File.ReadAllBytes(item.Foto1Url);
                        imageStream1 = new MemoryStream(buffer);
                        cell = row.AddCell();
                        paragraph = cell.AddParagraph();
                        paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        picture = paragraph.AppendPicture(imageStream1);
                        picture.TextWrappingStyle = TextWrappingStyle.Inline;
                        picture.Width = 200;
                        picture.Height = 150;


                        //Texto
                        cell = row.AddCell();
                        paragraph = cell.AddParagraph();
                        paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                        text = paragraph.AppendText(item.Foto1_Pie);
                        text.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                        text.CharacterFormat.FontSize = 12;
                        text.CharacterFormat.Bold = false;
                        text.CharacterFormat.TextColor = Syncfusion.Drawing.Color.DarkBlue;
                    }


                    if (!string.IsNullOrEmpty(item.Foto2Url) || !string.IsNullOrEmpty(item.Foto2_Pie))
                    {
                        ////Segunda fila
                        row = table.AddRow();
                        row.RowFormat.BackColor = Syncfusion.Drawing.Color.DimGray;
                        row.RowFormat.Paddings.Top = 5;
                        //Imagen
                        cell = row.Cells[0];
                        buffer = System.IO.File.ReadAllBytes(item.Foto2Url);
                        imageStream1 = new MemoryStream(buffer);
                        paragraph = cell.AddParagraph();
                        paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        picture = paragraph.AppendPicture(imageStream1);
                        picture.CharacterFormat.TextBackgroundColor = Syncfusion.Drawing.Color.LightYellow;
                        picture.TextWrappingStyle = TextWrappingStyle.Inline;
                        picture.Width = 200;
                        picture.Height = 150;

                        //Texto
                        cell = row.Cells[1];
                        paragraph = cell.AddParagraph();
                        paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                        text = paragraph.AppendText(item.Foto2_Pie);
                        text.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                        text.CharacterFormat.FontSize = 12;
                        text.CharacterFormat.Bold = false;
                        text.CharacterFormat.TextColor = Syncfusion.Drawing.Color.DarkBlue;
                    }


                    if (!string.IsNullOrEmpty(item.Foto3Url) || !string.IsNullOrEmpty(item.Foto3_Pie))
                    {
                        //Tercera fila
                        row = table.AddRow();
                        row.RowFormat.BackColor = Syncfusion.Drawing.Color.LightGray;

                        //Imagen
                        cell = row.Cells[0];
                        buffer = System.IO.File.ReadAllBytes(item.Foto3Url);
                        imageStream1 = new MemoryStream(buffer);
                        paragraph = cell.AddParagraph();
                        paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        picture = paragraph.AppendPicture(imageStream1);
                        picture.TextWrappingStyle = TextWrappingStyle.Inline;
                        picture.CharacterFormat.TextColor = Syncfusion.Drawing.Color.LightYellow;
                        picture.Width = 200;
                        picture.Height = 150;

                        //Texto
                        cell = row.Cells[1];
                        paragraph = cell.AddParagraph();
                        paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                        text = paragraph.AppendText(item.Foto3_Pie);
                        text.CharacterFormat.FontName = "Microsoft Sans Serif (Body)";
                        text.CharacterFormat.FontSize = 12;
                        text.CharacterFormat.Bold = false;
                        text.CharacterFormat.TextColor = Syncfusion.Drawing.Color.DarkBlue;
                    }



                }
            }


            //Saves the Word document to MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);

            //Save the stream as a file in the device and invoke it for viewing
           await DependencyService.Get<ISave>().SaveAndView("Actividades.docx", "application/msword", stream);

        }


    }
}