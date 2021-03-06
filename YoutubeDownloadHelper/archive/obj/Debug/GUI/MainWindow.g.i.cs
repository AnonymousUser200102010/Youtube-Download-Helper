﻿#pragma checksum "..\..\..\GUI\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "14AF040E0A06FFACDA6A8E78E91447B3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace YoutubeDownloadHelper.Gui {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal YoutubeDownloadHelper.Gui.MainWindow mainWindow;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl mainTabControl;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem queueTab;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid queueGrid;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button addUrlButton;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button modifyUrlButton;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button moveQueuedItemUp;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid upArrowGrid;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image upArrowImage;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button moveQueuedItemDown;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid downArrowGrid;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image downArrowImage;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView queueListView;
        
        #line default
        #line hidden
        
        
        #line 199 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Menu mainMenu;
        
        #line default
        #line hidden
        
        
        #line 202 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem optionsMenu;
        
        #line default
        #line hidden
        
        
        #line 207 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem helpMenu;
        
        #line default
        #line hidden
        
        
        #line 210 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem aboutMenuItem;
        
        #line default
        #line hidden
        
        
        #line 225 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar downloadProgressBar;
        
        #line default
        #line hidden
        
        
        #line 268 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock downloadProgressText;
        
        #line default
        #line hidden
        
        
        #line 278 "..\..\..\GUI\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button startDownloadingButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/YoutubeDownloadHelper;component/gui/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\GUI\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mainWindow = ((YoutubeDownloadHelper.Gui.MainWindow)(target));
            
            #line 9 "..\..\..\GUI\MainWindow.xaml"
            this.mainWindow.Closed += new System.EventHandler(this.mainWindow_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.mainTabControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 3:
            this.queueTab = ((System.Windows.Controls.TabItem)(target));
            return;
            case 4:
            this.queueGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.addUrlButton = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\..\GUI\MainWindow.xaml"
            this.addUrlButton.Click += new System.Windows.RoutedEventHandler(this.UrlButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.modifyUrlButton = ((System.Windows.Controls.Button)(target));
            
            #line 52 "..\..\..\GUI\MainWindow.xaml"
            this.modifyUrlButton.Click += new System.Windows.RoutedEventHandler(this.UrlButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.moveQueuedItemUp = ((System.Windows.Controls.Button)(target));
            
            #line 65 "..\..\..\GUI\MainWindow.xaml"
            this.moveQueuedItemUp.Click += new System.Windows.RoutedEventHandler(this.moveQueuedItem_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.upArrowGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 9:
            this.upArrowImage = ((System.Windows.Controls.Image)(target));
            return;
            case 10:
            this.moveQueuedItemDown = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\..\GUI\MainWindow.xaml"
            this.moveQueuedItemDown.Click += new System.Windows.RoutedEventHandler(this.moveQueuedItem_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.downArrowGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 12:
            this.downArrowImage = ((System.Windows.Controls.Image)(target));
            return;
            case 13:
            this.queueListView = ((System.Windows.Controls.ListView)(target));
            
            #line 127 "..\..\..\GUI\MainWindow.xaml"
            this.queueListView.KeyUp += new System.Windows.Input.KeyEventHandler(this.queueListView_KeyUp);
            
            #line default
            #line hidden
            return;
            case 14:
            this.mainMenu = ((System.Windows.Controls.Menu)(target));
            return;
            case 15:
            this.optionsMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 205 "..\..\..\GUI\MainWindow.xaml"
            this.optionsMenu.Click += new System.Windows.RoutedEventHandler(this.optionsMenu_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            this.helpMenu = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 17:
            this.aboutMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 212 "..\..\..\GUI\MainWindow.xaml"
            this.aboutMenuItem.Click += new System.Windows.RoutedEventHandler(this.aboutMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 18:
            this.downloadProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 19:
            this.downloadProgressText = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 20:
            this.startDownloadingButton = ((System.Windows.Controls.Button)(target));
            
            #line 279 "..\..\..\GUI\MainWindow.xaml"
            this.startDownloadingButton.Click += new System.Windows.RoutedEventHandler(this.DownloadButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

