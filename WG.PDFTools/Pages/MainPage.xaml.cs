using System.Diagnostics;

namespace WG.PdfTools
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        List<string> items = [];

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void ConvertBtn_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("ConvertBtn_Clicked");
        }

        private async void DropGestureRecognizer_DragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("DropGestureRecognizer_DragOver");
#if WINDOWS
            // Check if the platform arguments contain StorageItems 
            var windowsDragEventArgs = e.PlatformArgs?.DragEventArgs;

            if (windowsDragEventArgs is null)
            {
                Debug.WriteLine("DragOver without PlatformArgs.");
                e.AcceptedOperation = DataPackageOperation.None;
                return;
            }

            var windowsDragUI = windowsDragEventArgs.DragUIOverride;

            if (windowsDragUI is not null)
            {
                Debug.WriteLine("Windows DragOver detected with DragUIOverride.");
                windowsDragUI.Caption = "Drop files herrrrrwwww";
                windowsDragUI.IsCaptionVisible = true;
            }
            else
            {
                Debug.WriteLine("Windows DragOver without DragUIOverride.");
                 return;
            }

            var draggedOverItems = await windowsDragEventArgs.DataView.GetStorageItemsAsync();
            if (draggedOverItems is null) {
                Debug.WriteLine("Windows DragOver without DragUIOverride.");
                return;
            }
            if (draggedOverItems.Any())
            {
                Debug.WriteLine($"DragOver with {draggedOverItems.Count()} items.");
                e.AcceptedOperation = DataPackageOperation.Copy;
                windowsDragUI.Caption = "Drop to copydddd";
                windowsDragUI.IsContentVisible = true;
                windowsDragUI.IsGlyphVisible = true;

                foreach (var item in draggedOverItems) {
                    if (item is Windows.Storage.StorageFile file) {
                        var fileExt = file.FileType.ToLower();
                        if (fileExt == ".pdf") {
                            windowsDragUI.Caption = "Drop pdf file here";
                            windowsDragUI.IsCaptionVisible = true;
                            windowsDragUI.IsGlyphVisible = true;
                            items.Add(file.Path);
                            Debug.WriteLine($"File ${file.Path} drag over.");
                        } else {
                            windowsDragUI.Caption = "Only pdf file allowed";
                            windowsDragUI.IsCaptionVisible = true;
                            windowsDragUI.IsGlyphVisible = false;
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("DragOver with no items.");
                e.AcceptedOperation = DataPackageOperation.None;
                windowsDragUI.IsContentVisible = false;
                windowsDragUI.IsGlyphVisible = false;
            }
#endif
        }

        private void DropGestureRecognizer_DragLeave(object sender, DragEventArgs e)
        {
            Debug.WriteLine("DropGestureRecognizer_DragLeave");
        }

        private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            FilesToMergeListView.ItemsSource = items;
        }
    }

}
