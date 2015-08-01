using System;
using System.Windows;
using System.Windows.Forms;
using YoutubeDownloadHelper.Code;

namespace YoutubeDownloadHelper.Gui
{
	/// <summary>
	/// Interaction logic for Options.xaml
	/// </summary>
	public partial class Options : Window
	{
		
		private readonly MainWindow MainWindow;
		private readonly Settings savedSettings = new ClassContainer().IOHandlingCode.ReadFromRegistry();
		
		public Options(MainWindow mainWindow)
		{
			this.DataContext = this.savedSettings;
			InitializeComponent();
			this.MainWindow = mainWindow;
		}
		
		private void window1_Closed(object sender, EventArgs e)
		{
			
			if(!(bool)doNotSaveOnClose.IsChecked)
			{
				new ClassContainer().IOHandlingCode.WriteToRegistry(this.savedSettings);
			}
			this.MainWindow.MainProgramElements.WindowEnabled = true;
			
		}
		
		private void folderSelectButton_Click(object sender, RoutedEventArgs e)
		{
			
			using(var dialog = new FolderBrowserDialog())
			{
	            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
	            {
	            	if(this.mainTab.IsSelected)
	            	{
	            		savedSettings.MainSaveLocation = dialog.SelectedPath;
	            	}
	            	else
	            	{
	            		savedSettings.TemporarySaveLocation = dialog.SelectedPath;
	            	}	
	            }
			}
		}
	}
}