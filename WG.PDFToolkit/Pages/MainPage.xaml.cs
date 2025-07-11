using System.Diagnostics;
using WG.PdfTools.Pages;

namespace WG.PdfTools
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        List<string> items = [];
        private readonly MainPageService pageService;
        public MainPage()
        {
            InitializeComponent();
            pageService = new MainPageService();
        }

        private void OnMergeClicked(object sender, EventArgs e)
        {
            var desktopPath =  Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (string.IsNullOrEmpty(desktopPath)) {
                Shell.Current.DisplayAlert("WG PDFToolkit", "Impossible to retreave windows desktop path", "OK");
                return;
            }
            if (items.Count == 0) {
                Shell.Current.DisplayAlert("WG PDFToolkit", "Non sono stati caricare dei file", "OK");
                return;
            }
            var today = DateTime.Now;
            var outputFile = Path.Combine(desktopPath, $"wg-pdftoolkit-merged-file-{today.Year}{today.Month}{today.Day}{today.Hour}{today.Minute}.pdf");
            var result = pageService.MergeFiles(items, outputFile);

            if (result == true) {
                Shell.Current.DisplayAlert("WG PDFToolkit", $"File output correttamente generato\n{outputFile}", "OK");
                return;
            }

            Shell.Current.DisplayAlert("INFO WG PDFToolkit", "Non è stato possibile elaborare i file", "OK", FlowDirection.RightToLeft);
        }

        private void ConvertBtn_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("ConvertBtn_Clicked");
        }

        private async void DropGestureRecognizer_DragOver(object sender, DragEventArgs e)
        {
            pageService.OnDragOver(sender, e);
        }

        private void DropGestureRecognizer_DragLeave(object sender, DragEventArgs e)
        {
            Debug.WriteLine("DropGestureRecognizer_DragLeave");
        }

        private async void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            var newDroppedItems = await pageService.OnDrop(sender, e);
            foreach (var item in newDroppedItems)
            {
                if (items.Contains(item)) continue;
                items.Add(item);    
            }
            FilesToMergeListView.ItemsSource = new List<string>(items);
        }
    }

}
