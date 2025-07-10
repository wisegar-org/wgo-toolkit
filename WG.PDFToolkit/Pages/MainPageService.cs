using QuestPDF.Fluent;
using System.Diagnostics;

namespace WG.PdfTools.Pages
{
    public class MainPageService
    {
        public async Task<List<string>> OnDrop(object sender, DropEventArgs e)
        {
            List<string> items = [];
#if WINDOWS
            var windowsDragEventArgs = e.PlatformArgs?.DragEventArgs;
            if (windowsDragEventArgs is null)
            {
                Debug.WriteLine("Drop without PlatformArgs.");            
                return items;
            }
            var windowsDragUI = windowsDragEventArgs.DragUIOverride;

            if (windowsDragUI is null)
            {
                Debug.WriteLine("Windows Drop without DragUIOverride.");
                return items;
            }

            var draggedOverItems = await windowsDragEventArgs.DataView.GetStorageItemsAsync();
            if (draggedOverItems is null) {
                Debug.WriteLine("Windows DragOver without DragUIOverride.");
                return items;
            }

            if (draggedOverItems.Any())
            {
                Debug.WriteLine($"DragOver with {draggedOverItems.Count()} items.");
                windowsDragUI.Caption = "Drop to copy";
                windowsDragUI.IsContentVisible = true;
                windowsDragUI.IsGlyphVisible = true;

                foreach (var item in draggedOverItems) {
                    if (item is Windows.Storage.StorageFile file) {
                        var fileExt = file.FileType.ToLower();
                        if (fileExt == ".pdf") {
                            items.Add(file.Path);
                            Debug.WriteLine($"File ${file.Path} Drop.");
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("DragOver with no items.");
            }

#endif

            return items;
        }

        public async void OnDragOver(object sender, DragEventArgs e) {
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
                            Debug.WriteLine($"File ${file.Path} drag over.");
                            //if (!items.Contains(file.Path)) {
                            //    items.Add(file.Path);
                            //}
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
       
        public bool MergeFiles(List<string> files, string output)
        {
            if (files is null) return false;
            if (files.Count < 2) return false;
            var documentOperator = DocumentOperation.LoadFile(files[0]);
            for (int i = 1; i < files.Count; i++)
            {
                documentOperator =  documentOperator.MergeFile(files[i]);
            }
            documentOperator.Save(output);
            return true;
        }
    }
}
