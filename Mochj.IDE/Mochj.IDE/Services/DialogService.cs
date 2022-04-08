using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Mochj.IDE.Services
{
    static class DialogService
    {
        public static string FolderBrowser()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }

        public static string LibraryFileBrowser()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.Multiselect = false;
            dialog.Filters.Add(new CommonFileDialogFilter("Dynamically linked library (*.dll)", "*.dll"));
            dialog.Filters.Add(new CommonFileDialogFilter("Mochj script file (*.mochj)", "*.mochj"));
            dialog.DefaultExtension = ".dll";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }

        public static string ScriptFileBrowser()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.Multiselect = false;
            dialog.Filters.Add(new CommonFileDialogFilter("Mochj script file (*.mochj)", "*.mochj"));
            dialog.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));
            dialog.DefaultExtension = ".mochj";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }
    }
}
