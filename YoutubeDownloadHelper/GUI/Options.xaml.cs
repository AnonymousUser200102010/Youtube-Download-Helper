using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using YoutubeDownloadHelper.Code;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Gui
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private readonly MainProgramElements MainWindow;
        private readonly Settings savedSettings = (new ClassContainer()).IOCode.RegistryRead(new Settings ());
        private bool resetWidths;

        /// <summary>
        /// Options window.
        /// </summary>
        /// <param name="mainWindow">
        /// The parent window.
        /// </param>
        public Options (MainProgramElements mainWindow)
        {
            this.DataContext = this.savedSettings;
            InitializeComponent();
            this.MainWindow = mainWindow;
        }

        private void window1_Closed (object sender, EventArgs e)
        {
			if (!(bool)doNotSaveOnClose.IsChecked)
			{
				if(this.savedSettings.DeleteRegistryEntry)
				{
					IOFunc.DeleteRegistrySubkey(Storage.RegistryRoot, App.IsDebugging);
				}
				if(resetWidths)
	        	{
	        		this.MainWindow.QueuePositionTagWidth = this.savedSettings.QueuePositionTagWidth;
		        	this.MainWindow.QueueLocationTagWidth = this.savedSettings.QueueLocationTagWidth;
		        	this.MainWindow.QueueQualityTagWidth = this.savedSettings.QueueQualityTagWidth;
		        	this.MainWindow.QueueFormatTagWidth = this.savedSettings.QueueFormatTagWidth;
		        	this.MainWindow.QueueIsAudioTagWidth = this.savedSettings.QueueIsAudioTagWidth;
	        	}
				(new ClassContainer()).IOCode.RegistryWrite(savedSettings.AsEnumerable(SettingsReturnType.Essential));
			}
            this.MainWindow.WindowEnabled = true;
        }

        private void folderSelectButton_Click (object sender, RoutedEventArgs e)
        {
			using (var dialog = new CommonOpenFileDialog())
			{
				dialog.Multiselect = false;
				dialog.DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;
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
				else if (this.validationDirectories.IsSelected)
				{
					dialog.Title = "Select Validation Directories to Add";
					var itemsStore = this.validationDirListView.Items;
					var selectedItems = this.validationDirListView.SelectedItems;
					dialog.InitialDirectory = selectedItems.Count > 0 ? selectedItems[0].ToString() : itemsStore.Count > 0 ? itemsStore[0].ToString() : dialog.DefaultDirectory;
					dialog.Multiselect = true;
				}
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
					else if (this.validationDirectories.IsSelected)
					{
						savedSettings.ValidationLocations = new ObservableCollection<string>(savedSettings.ValidationLocations.Union(dialog.FileNames));
					}
				}
				dialog.Dispose();
			}
        }
        
        private void additionalValidationButton_Click(object sender, RoutedEventArgs e)
		{
			bool isClearButton = ((System.Windows.Controls.Control)sender).Name.Contains("clear", StringComparison.OrdinalIgnoreCase);
			System.Collections.Generic.List<string> selectedItems = new System.Collections.Generic.List<string>();
			string selectedItemsCombined = string.Empty;
			var messageBox = MessageBoxResult.None;
			var varItemsCount = this.validationDirListView.Items.Count > 0;
			switch(isClearButton)
			{
				case false:
					if (this.validationDirListView.SelectedItems.Count > 0 && varItemsCount)
					{
						for (int position = 0, maxItemsCount = this.validationDirListView.SelectedItems.Count; position < maxItemsCount; position++)
						{
							var currentItem = this.validationDirListView.SelectedItems[position];
							selectedItems.Add(currentItem.ToString());
							selectedItemsCombined += string.Format(CultureInfo.InvariantCulture, "'{0}'{1}", currentItem, position < maxItemsCount - 1 ? ", " : string.Empty);
						}
						messageBox = Xceed.Wpf.Toolkit.MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "Are you sure you want to remove the following directories: {0}?", selectedItemsCombined), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
					}
					else if (this.validationDirListView.Items.Count <= 0)
					{
						Xceed.Wpf.Toolkit.MessageBox.Show("You cannot remove items from an empty list.", "Generic Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
					}
					else
					{
						Xceed.Wpf.Toolkit.MessageBox.Show("There must be an item selected to remove it from the list.", "Generic Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
					}
					break;
				case true:
					messageBox = varItemsCount ? Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure you want to clear the list of validation directories?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) : Xceed.Wpf.Toolkit.MessageBox.Show("You cannot clear an empty list.", "Generic Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
					break;
			}
			
			if (messageBox == MessageBoxResult.Yes)
			{
				this.savedSettings.ValidationLocations = isClearButton ? new ObservableCollection<string>() : new ObservableCollection<string>(this.savedSettings.ValidationLocations.Where(dir => !selectedItems.Any(item => dir.Contains(item, StringComparison.OrdinalIgnoreCase))));
			}
		}
        
		private void resetTagsButton_Click(object sender, RoutedEventArgs e)
		{
			var messageBox = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure you want to reset the 'tag' width values? You can always change them again later.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
	        if (messageBox == MessageBoxResult.Yes)
	        {
	        	resetWidths = true;
	        	this.savedSettings.ResetTagWidthsToDefault();
	        }
		}
		
		private void saveLocationsTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			this.folderSelectButton.Visibility = this.validationDirectories.IsSelected ? Visibility.Hidden : Visibility.Visible;
			this.addButton.Visibility = this.validationDirectories.IsSelected ? Visibility.Visible : Visibility.Hidden;
			this.removeButton.Visibility = this.validationDirectories.IsSelected ? Visibility.Visible : Visibility.Hidden;
			this.clearButton.Visibility = this.validationDirectories.IsSelected ? Visibility.Visible : Visibility.Hidden;
		}
    }
}