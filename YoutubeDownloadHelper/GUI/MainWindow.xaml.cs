
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YoutubeDownloadHelper.Code;
using YoutubeDownloadHelper.Gui;
using UniversalHandlersLibrary;
using System.Linq;

namespace YoutubeDownloadHelper.Gui
{

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly VideosToDownload videoQueue = new VideosToDownload ();
        private readonly ClassContainer classContainer = new ClassContainer ();
        private MainProgramElements mainProgramElements;
		
        public ObservableCollection<Video> Videos
        {
            get
            {
            	ValidateMoveButtonAvailability(videoQueue.Items);
                return videoQueue.Items;
            }
            
            set
            {
            	videoQueue.Items = value;
            }
        }
        
        public MainProgramElements MainProgramElements
        {
        	get
        	{
        		return this.mainProgramElements;
        	}
        	set
        	{
        		this.mainProgramElements = value;
        	}
        }
        
        public void RefreshQueue (ObservableCollection<Video> collectionToAssimilate, int newIndex)
        {
            MainProgramElements.Videos = collectionToAssimilate;
            MainProgramElements.CurrentlySelectedQueueIndex = newIndex;
            this.queueListView.ScrollIntoView(this.queueListView.SelectedItem);
            
        }
        
        private void ValidateMoveButtonAvailability(ObservableCollection<Video> vidQueue)
        {
        	this.moveQueuedItemDown.IsEnabled = vidQueue.Count > 1; 
            this.moveQueuedItemUp.IsEnabled = vidQueue.Count > 1;
        }

        public MainWindow (bool downloadImmediately)
        {
        	this.mainProgramElements = new MainProgramElements(this as MainWindow);
            this.DataContext = MainProgramElements;
            InitializeComponent();
            if(downloadImmediately)
            {
            	this.DownloadButton_Click(this.startDownloadingButton, new RoutedEventArgs());
            }
        }

        void optionsMenu_Click (object sender, RoutedEventArgs e)
        {
            Options optionsWindow = new Options (this);
            optionsWindow.Show();
			
            this.MainProgramElements.WindowEnabled = false;
        }

        void mainWindow_Closed (object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        void UrlButton_Click (object sender, RoutedEventArgs e)
        {
			
            string senderName = ((Control)sender).Name;
            
            UrlManipulation manipulationWindow = new UrlManipulation (senderName.Contains("add", StringComparison.OrdinalIgnoreCase), this, this.videoQueue.Items, this.queueListView.SelectedIndex);
            manipulationWindow.Show();
            this.mainProgramElements.WindowEnabled = false;
			
        }

        void moveQueuedItem_Click (object sender, RoutedEventArgs e)
        {
			
            ObservableCollection<Video> finalizedCollection = videoQueue.Items;
			
			var selectedQueueIndex = this.queueListView.SelectedIndex;
            int newlySelectedIndex = ((Control)sender).Name.Contains("down", StringComparison.OrdinalIgnoreCase) 
				?
					selectedQueueIndex + 1 < finalizedCollection.Count - 1 ? selectedQueueIndex + 1 : finalizedCollection.Count - 1 
				: 
					selectedQueueIndex - 1 > 0 ? selectedQueueIndex - 1 : 0;
            
			finalizedCollection.Move(selectedQueueIndex, newlySelectedIndex);
			finalizedCollection[selectedQueueIndex].Position = selectedQueueIndex;
			finalizedCollection[newlySelectedIndex].Position = newlySelectedIndex;
            RefreshQueue(finalizedCollection, newlySelectedIndex);
			
        }

        void queueListView_KeyUp (object sender, KeyEventArgs e)
        {
			switch (e.Key)
			{
				case Key.W:
				case Key.I:
					moveQueuedItem_Click(this.moveQueuedItemUp, null);
					break;
				case Key.S:
				case Key.K:
					moveQueuedItem_Click(this.moveQueuedItemDown, null);
					break;
				case Key.Delete:
				case Key.Back:
					int previouslySelectedIndex = this.queueListView.SelectedIndex;
					if (previouslySelectedIndex > 0 || (this.queueListView.Items.Count - 1) > 0)
					{
						ObservableCollection<Video> finalizedCollection = videoQueue.Items;
						MessageBoxResult deletionAnswer = MessageBox.Show(string.Format(CultureInfo.InstalledUICulture, "Are you sure you want to delete {0}?", finalizedCollection[previouslySelectedIndex].Location), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
						if (deletionAnswer == MessageBoxResult.Yes)
						{
							finalizedCollection.RemoveAt(previouslySelectedIndex);
							for(int videoPosition = 0; videoPosition < finalizedCollection.Count; videoPosition++)
							{
								finalizedCollection[videoPosition].Position = videoPosition;
							}
							RefreshQueue(finalizedCollection, previouslySelectedIndex < Videos.Count - 1 ? previouslySelectedIndex : Videos.Count - 1);
						}
					}
					else
					{
						MessageBox.Show("Could not delete final URL or no URL selected.", "Cannot Delete URL", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
					}
					break;
			}
        }

        void DownloadButton_Click (object sender, RoutedEventArgs e)
        {
            this.MainProgramElements.WindowEnabled = false;
           	this.MainProgramElements.CurrentDownloadOutputText = "Beginning....";
            int selectedIndex = this.queueListView.SelectedIndex;
            Task.Factory.StartNew(() => this.classContainer.DownloadingCode.DownloadHandler(this, selectedIndex));
        }
        
		void aboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string disclaimer = string.Format(CultureInfo.CurrentCulture, "{0}\n\n{1}\n\n{2}\n\n{3}\n\n{4}", "I am not liable for any damage done to you, your property, or otherwise with regard to the running, compiling, and/or overall use of this program, it's associated libraries (if any), and/or any additional data contained within the source. I am likewise not financially, morally or legally obligated to pay for the cost of the aforementioned property, any lost wages, any emotional distress, and/or the repairs thereof.", "This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.", "I do not extend additional warranties and will not fulfil any warranty given to you by any third party or otherwise. By downloading this source, compiling the source from either (github, a third party website, or otherwise), and/or using (a) pre-compiled version(s) of this program from a third party website or otherwise, you agree to not only the license contained within this project, regardless of whether your version contained said license, but the information within this disclaimer and agree to all said information from the point you downloaded forward, and if using a previous version you agree to any new additions or removals of/to said information.", "This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.", "Full details available on the project page (https://github.com/AnonymousUser200102010/Youtube-Download-Helper)");
			
			About aboutWindow = new About(System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location), disclaimer);
            aboutWindow.Show();
		}
    }
    public class MainProgramElements : INotifyPropertyChanged 
    {
    	private readonly MainWindow main;
    	private int downProgress = 0;
    	private int selectedQueue = 0;
    	private string downOutput = string.Empty;
    	private bool areUpDownEnabled = false;
    	private bool isWindowEnabled = true;
    	public bool WindowEnabled
    	{
    		get
    		{
    			return isWindowEnabled;
    		}
    		set
    		{
    			isWindowEnabled = value;
    			RaisePropertyChanged("WindowEnabled");
    		}
    	}
    	public ObservableCollection<Video> Videos
    	{
    		get
    		{
    			return this.main.Videos;
    		}
    		set
    		{
    			this.main.Videos = value;
    		}
    	}
    	
    	public int CurrentlySelectedQueueIndex
    	{
    		get
    		{
    			return this.Videos.Count > 0 ? selectedQueue : -1;
    		}
    		set
    		{
    			selectedQueue = value;
    			RaisePropertyChanged("CurrentlySelectedQueueIndex");
    		}
    	}
    	public int CurrentDownloadProgress 
    	{ 
    		get
    		{
    			return downProgress;
    		}
    		set
    		{
    			downProgress = value;
    			RaisePropertyChanged("CurrentDownloadProgress");
    		} 
    	}
    	public string CurrentDownloadOutputText 
    	{ 
    		get
    		{
    			return downOutput;
    		}
    		set
    		{
    			downOutput = value;
    			RaisePropertyChanged("CurrentDownloadOutputText");
    		}
    	}

//    	public bool AreItemsLeft
//        {
//        	get
//        	{
//        		areUpDownEnabled = this.Videos.Count() > 1;
//        		return areUpDownEnabled;
//        	}
//        }
    	
    	#region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        
        public MainProgramElements(MainWindow mainWindow)
        {
        	this.main = mainWindow;
        }
    }
}