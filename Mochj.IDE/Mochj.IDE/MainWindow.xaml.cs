using Mochj._Parser.Models;
using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.IDE._Tokenizer;
using Mochj.IDE._Tokenizer.Models;
using Mochj.IDE.Bright;
using Mochj.IDE.Builders;
using Mochj.IDE.Components;
using Mochj.IDE.Components.MochjEditor;
using Mochj.IDE.Helpers;
using Mochj.IDE.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mochj.IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeDefaults();
        }

        #region Initialize

        private void InitializeDefaults()
        {
            MenuItem_ToggleCodeCompletion.IsChecked = MochjEditorConfigService.EnableCodeCompletion;
            MenuItem_ToggleIntellisense.IsChecked = MochjEditorConfigService.EnableIntellisenseOnKey;
        }

        #endregion


        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            var openFile = DialogService.ScriptFileBrowser();
            if (!string.IsNullOrEmpty(openFile))
            {
                if (File.Exists(openFile))
                {
                    TabControl_WorkingDocuments.Items.Add(
                        new TabItem { 
                            Header = System.IO.Path.GetFileName(openFile),
                            Content = new MochjScriptEditor(File.ReadAllText(openFile)) ,
                            ToolTip = openFile,
                        });
                } else
                {
                    MessageBox.Show("File does not exist");
                }
            }
        }

        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            TabControl_WorkingDocuments.Items.Add(new TabItem { Header = "unsaved", Content = new MochjScriptEditor() });
        }

        private void MenuItem_Rescan_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl_WorkingDocuments.SelectedContent is MochjScriptEditor editor)
            {
                editor.DoFullHighlight();
            }
        }

        private void MenuItem_ToggleCodeCompletion_Checked(object sender, RoutedEventArgs e)
        {
            MochjEditorConfigService.EnableCodeCompletion = MenuItem_ToggleCodeCompletion.IsChecked;
        }

        private void MenuItem_ToggleIntellisense_Checked(object sender, RoutedEventArgs e)
        {
            MochjEditorConfigService.EnableIntellisenseOnKey = MenuItem_ToggleIntellisense.IsChecked;
        }
    }
}
