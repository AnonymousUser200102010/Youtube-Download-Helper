
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Linq;
using YoutubeDownloadHelper.Code;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Gui
{
	/// <summary>
	/// Interaction logic for UrlManipulation.xaml
	/// </summary>
	public partial class UrlManipulation : Window
	{
		private readonly MainWindow MainWindow;
		private readonly ClassContainer classContainer = new ClassContainer();
		private ObservableCollection<Video> videoQueue;
		private readonly bool add;
		private int persistantUrlIndex;
		private readonly string exampleText = "Example: https://www.youtube.com/watch?v=ft6rWcJIlpU";
		
		private readonly string[] videoFormats = {
            "Mp4", 
            "Flash", 
            "Mobile", 
            "WebM"
        };
		
		public ReadOnlyCollection<string> FormatList
		{
			get
			{
				return new ReadOnlyCollection<string>(videoFormats);
			}
		}
		
		public UrlManipulation(bool add, MainWindow mainWindow, ObservableCollection<Video> itemQueue, int selectedUrl)
		{
			this.DataContext = this;
			InitializeComponent();
			this.videoQueue = itemQueue;
			this.MainWindow = mainWindow;
			this.persistantUrlIndex = selectedUrl;
			this.add = add;
			this.Title = string.Format(CultureInfo.CurrentCulture, "{0} Url(s) {1} the Queue", add ? "Add" : "Modify", add ? "to" : "in");
			this.avTextBox.Text = !add && videoQueue.Count > 0 ? string.Join(string.Empty, videoQueue) : string.Format(CultureInfo.CurrentCulture, "{0} 360 Mp4 (REMOVE!)", exampleText);
			this.basicManipulateUrlButton.Content = add ? "Add Url to the Queue" : "Modify Current Url";
			
			if(!add && (this.MainWindow as MainWindow).queueListView.Items.Count > 0)
			{
				this.basicUrlText.Text = videoQueue[this.persistantUrlIndex].Location;
				this.basicUrlText.TextAlignment = TextAlignment.Left;
				this.basicUrlText.FontStyle = FontStyles.Normal;
				this.basicUrlText.FontWeight = FontWeights.Normal;
				this.formatComboBox.SelectedIndex = this.formatComboBox.Items.IndexOf(videoQueue[this.persistantUrlIndex].Format.ToString());
				this.basicResolution.Value = videoQueue[this.persistantUrlIndex].Resolution;
			}
		}
		
		private void window1_Closed(object sender, EventArgs e)
		{
			this.MainWindow.RefreshQueue(videoQueue, persistantUrlIndex);
			this.MainWindow.MainProgramElements.WindowEnabled = true;
		}
		
		private void window1_Closing(object sender, EventArgs e)
		{
			if(advancedView.IsSelected)
			{
				try
				{
					ObservableCollection<Video> newItems = classContainer.ConversionCode.ConvertUrl(add ? videoQueue.Count : 0, this.avTextBox.Text.Split(new [] {"\n", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries));
					videoQueue.Replace(add ? new ObservableCollection<Video>(videoQueue.Union(newItems)) : newItems);
				}
				catch(Exception ex)
				{
					
					MessageBox.Show(ex.Message, "Fatal System Error", MessageBoxButton.OK, MessageBoxImage.Stop);
					this.basicUserInfoText.Text = classContainer.ConversionCode.Truncate(ex.Message, 20);
					Environment.Exit(0);
					throw;
				}
			}
		}
		
		void basicManipulateUrlButton_Click(object sender, RoutedEventArgs e)
		{
			
			this.basicUserInfoText.Text = string.Empty;
			var basicResolutionValue = (int)this.basicResolution.Value;
			
			if (!string.IsNullOrWhiteSpace(this.basicUrlText.Text) && !this.basicUrlText.Text.Contains("example", StringComparison.OrdinalIgnoreCase) && basicResolutionValue > 143 && basicResolutionValue < 720)
            {
				try
				{
					ObservableCollection<Video> finalizedCollection = videoQueue;
					
					var resultantVideo = new Video(add ? finalizedCollection.Count : persistantUrlIndex, this.basicUrlText.Text, basicResolutionValue, classContainer.ConversionCode.GetVideoFormat(this.formatComboBox.SelectedItem.ToString()));
					
					if(add)
					{
						finalizedCollection.Add(resultantVideo);
						this.persistantUrlIndex = finalizedCollection.IndexOf(resultantVideo);
					}
					else
					{
						finalizedCollection.RemoveAt(persistantUrlIndex);
		                finalizedCollection.Insert(persistantUrlIndex, resultantVideo);
					}
					
					videoQueue = finalizedCollection;
					this.basicUserInfoText.Text = "Success!";
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message, "Fatal System Error", MessageBoxButton.OK, MessageBoxImage.Stop);
					this.basicUserInfoText.Text = classContainer.ConversionCode.Truncate(ex.Message, 20);
					Environment.Exit(0);
					throw;
				}
			}
			else if (basicResolutionValue >= 720)
            {
				this.basicUserInfoText.Text = "Generic Error";
				MessageBox.Show("HD video resolution is currently unsupported", "Resolution Unsupported", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
            	this.basicUserInfoText.Text = "Generic Error";
                MessageBox.Show("Some or all values of the new URL are invalid.", "URL Invalid", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
		}
		
		void basicUrlText_TextManipulation(object sender, RoutedEventArgs e)
		{
			
			if(this.basicUrlText.IsFocused && this.basicUrlText.Text.Equals(exampleText, StringComparison.OrdinalIgnoreCase))
			{
				this.basicUrlText.Text = string.Empty;
				this.basicUrlText.FontStyle = FontStyles.Normal;
				this.basicUrlText.FontWeight = FontWeights.Normal;
				this.basicUrlText.TextAlignment = TextAlignment.Left;
			}
			else if(string.IsNullOrEmpty(this.basicUrlText.Text) || this.basicUrlText.Text.Contains("example:", StringComparison.OrdinalIgnoreCase))
			{
				this.basicUrlText.Text = "Example: https://www.youtube.com/watch?v=ft6rWcJIlpU";
				this.basicUrlText.FontStyle = FontStyles.Italic;
				this.basicUrlText.FontWeight = FontWeights.Light;
				this.basicUrlText.TextAlignment = TextAlignment.Center;
			}
		}
	}
}