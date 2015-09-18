using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using YoutubeDownloadHelper.Code;

namespace YoutubeDownloadHelper.Gui
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private readonly MainWindow MainWindow;
        private readonly Settings savedSettings = (new ClassContainer()).IOCode.RegistryRead(new Settings ());

        /// <summary>
        /// Options window.
        /// </summary>
        /// <param name="mainWindow">
        /// The parent window.
        /// </param>
        public Options (MainWindow mainWindow)
        {
            this.DataContext = this.savedSettings;
            InitializeComponent();
            this.MainWindow = mainWindow;
        }

        private void window1_Closed (object sender, EventArgs e)
        {
        	if (!(bool)doNotSaveOnClose.IsChecked) (new ClassContainer()).IOCode.RegistryWrite(savedSettings);
            this.MainWindow.MainProgramElements.WindowEnabled = true;
        }

        private void folderSelectButton_Click (object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
            	dialog.Multiselect = false;
            	if (this.mainTab.IsSelected)
            	{
            		dialog.Title = "Select Your Primary Storage Folder";
            		dialog.InitialDirectory = savedSettings.MainSaveLocation;
            	}
            	else if (this.tempSaveLocation.IsSelected)
            	{
            		dialog.Title = "Select Your Temporary Storage Folder";
            		dialog.InitialDirectory = savedSettings.TemporarySaveLocation;
            	}
            	else
            	{
            		dialog.Title = "Select All Folders To Check For Existing Downloads";
            		dialog.InitialDirectory = this.validationDirListView.SelectedIndex >= 0 ? this.validationDirListView.SelectedItem.ToString() : savedSettings.ValidationLocations.FirstOrDefault();
            		dialog.Multiselect = true;
            	}
            	dialog.DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;
            	dialog.IsFolderPicker = true;
            	dialog.AddToMostRecentlyUsedList = false;
				dialog.AllowNonFileSystemItems = false;
				dialog.EnsureFileExists = true;
				dialog.EnsurePathExists = true;
				dialog.EnsureReadOnly = false;
				dialog.EnsureValidNames = true;
				dialog.ShowPlacesList = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (this.mainTab.IsSelected) savedSettings.MainSaveLocation = dialog.FileName;
                    else if (this.tempSaveLocation.IsSelected) savedSettings.TemporarySaveLocation = dialog.FileName;
					else savedSettings.ValidationLocations = new ObservableCollection<string>(dialog.FileNames.ToList());
                }
                this.Focus();
                dialog.Dispose();
            }
        }
    }
}