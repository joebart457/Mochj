﻿#pragma checksum "..\..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "EACE5AF6792D8A9EFDB119D85088C39CB590E697"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Mochj.IDE;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Mochj.IDE {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 64 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItem_New;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItem_Open;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItem_Save;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItem_Exit;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItem_Rescan;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItem_ToggleCodeCompletion;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItem_ToggleIntellisense;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl TabControl_WorkingDocuments;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.3.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Mochj.IDE;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.3.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.MenuItem_New = ((System.Windows.Controls.MenuItem)(target));
            
            #line 64 "..\..\..\MainWindow.xaml"
            this.MenuItem_New.Click += new System.Windows.RoutedEventHandler(this.MenuItem_New_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MenuItem_Open = ((System.Windows.Controls.MenuItem)(target));
            
            #line 65 "..\..\..\MainWindow.xaml"
            this.MenuItem_Open.Click += new System.Windows.RoutedEventHandler(this.MenuItem_Open_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.MenuItem_Save = ((System.Windows.Controls.MenuItem)(target));
            
            #line 66 "..\..\..\MainWindow.xaml"
            this.MenuItem_Save.Click += new System.Windows.RoutedEventHandler(this.MenuItem_Save_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.MenuItem_Exit = ((System.Windows.Controls.MenuItem)(target));
            
            #line 68 "..\..\..\MainWindow.xaml"
            this.MenuItem_Exit.Click += new System.Windows.RoutedEventHandler(this.MenuItem_Exit_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MenuItem_Rescan = ((System.Windows.Controls.MenuItem)(target));
            
            #line 71 "..\..\..\MainWindow.xaml"
            this.MenuItem_Rescan.Click += new System.Windows.RoutedEventHandler(this.MenuItem_Rescan_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.MenuItem_ToggleCodeCompletion = ((System.Windows.Controls.MenuItem)(target));
            
            #line 76 "..\..\..\MainWindow.xaml"
            this.MenuItem_ToggleCodeCompletion.Checked += new System.Windows.RoutedEventHandler(this.MenuItem_ToggleCodeCompletion_Checked);
            
            #line default
            #line hidden
            
            #line 77 "..\..\..\MainWindow.xaml"
            this.MenuItem_ToggleCodeCompletion.Unchecked += new System.Windows.RoutedEventHandler(this.MenuItem_ToggleCodeCompletion_Checked);
            
            #line default
            #line hidden
            return;
            case 7:
            this.MenuItem_ToggleIntellisense = ((System.Windows.Controls.MenuItem)(target));
            
            #line 82 "..\..\..\MainWindow.xaml"
            this.MenuItem_ToggleIntellisense.Checked += new System.Windows.RoutedEventHandler(this.MenuItem_ToggleIntellisense_Checked);
            
            #line default
            #line hidden
            
            #line 83 "..\..\..\MainWindow.xaml"
            this.MenuItem_ToggleIntellisense.Unchecked += new System.Windows.RoutedEventHandler(this.MenuItem_ToggleIntellisense_Checked);
            
            #line default
            #line hidden
            return;
            case 8:
            this.TabControl_WorkingDocuments = ((System.Windows.Controls.TabControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

