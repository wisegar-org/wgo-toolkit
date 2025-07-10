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

        private void OnCounterClicked(object sender, EventArgs e)
        {
            //count++;

            //if (count == 1)
            //    CounterBtn.Text = $"Clicked {count} time";
            //else
            //    CounterBtn.Text = $"Clicked {count} times";

            //SemanticScreenReader.Announce(CounterBtn.Text);

            //Debug.WriteLine($"Counter clicked {count} times.");

            var desktopPath =  Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (string.IsNullOrEmpty(desktopPath)) {
                Shell.Current.DisplayAlert("Info", "Impossible to retreave windows desktop path", "OK");
                return;
            }

            pageService.MergeFiles(items, Path.Combine(desktopPath, "mergedfile.pdf"));
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
