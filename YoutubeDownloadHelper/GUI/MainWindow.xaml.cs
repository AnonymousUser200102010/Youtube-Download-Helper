﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YoutubeDownloadHelper.Code;
using UniversalHandlersLibrary;
using System.Linq;

namespace YoutubeDownloadHelper.Gui
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideosToDownload videoQueue = new VideosToDownload ();
        private ProjectAssemblies embeddedLibraries;
        public MainProgramElements MainProgramElements { get; set; }
        public VideosToDownload VideoQueue { get { return this.videoQueue; } set { this.videoQueue = value; } }

        public void RefreshQueue (Collection<Video> collectionToAssimilate, int newIndex)
        {
        	var convertedCollection = new ObservableCollection<Video>(collectionToAssimilate);
            this.MainProgramElements.Videos = convertedCollection;
            this.MainProgramElements.Videos.WriteToFile(Storage.QueueFile);
            this.MainProgramElements.CurrentlySelectedQueueIndex = newIndex;
        }

        public void ValidateAvailabilityOfSpecialItems (ObservableCollection<Video> collectionToCheck)
        {
        	if(this.modifyUrlButton != null && this.startDownloadingButton != null && this.moveQueuedItemDown != null && this.moveQueuedItemUp != null)
        	{
	            this.modifyUrlButton.IsEnabled = collectionToCheck.Any();
	            this.startDownloadingButton.IsEnabled = collectionToCheck.Any();
	            
	            var countCheck = collectionToCheck.Count > 1;
	            this.moveQueuedItemDown.IsEnabled = countCheck; 
	            this.moveQueuedItemUp.IsEnabled = countCheck;
        	}
        }

        public MainWindow (bool downloadImmediately)
        {
        	IOFunc.CreateFolderTree(Storage.Folder);
            this.MainProgramElements = new MainProgramElements (this as MainWindow);
            this.DataContext = MainProgramElements;
            InitializeComponent();
            ValidateAvailabilityOfSpecialItems(this.videoQueue.Items);
            if (downloadImmediately) this.DownloadButton_Click(this.startDownloadingButton, new RoutedEventArgs ());
        }

        void optionsMenu_Click (object sender, RoutedEventArgs e)
        {
            (new Options (this)).Show();
            this.MainProgramElements.WindowEnabled = false;
        }

        void mainWindow_Closed (object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        void UrlButton_Click (object sender, RoutedEventArgs e)
        {
            (new UrlManipulation (((Control)sender).Name.Contains("add", StringComparison.OrdinalIgnoreCase), this, this.videoQueue.Items, this.queueListView.SelectedIndex)).Show();
            this.MainProgramElements.WindowEnabled = false;
        }

        void moveQueuedItem_Click (object sender, RoutedEventArgs e)
        {
            var finalizedCollection = videoQueue.Items;
            var selectedQueueIndex = this.queueListView.SelectedIndex;
            int newlySelectedIndex = ((Control)sender).Name.Contains("down", StringComparison.OrdinalIgnoreCase) 
				?
            		selectedQueueIndex + 1 < finalizedCollection.All() ? selectedQueueIndex + 1 : finalizedCollection.All()
				: 
					selectedQueueIndex - 1 >= 0 ? selectedQueueIndex - 1 : 0;
            
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
                    this.moveQueuedItem_Click(this.moveQueuedItemUp, null);
                    break;
                case Key.S:
                case Key.K:
                    this.moveQueuedItem_Click(this.moveQueuedItemDown, null);
                    break;
                case Key.Delete:
                case Key.Back:
                    int previouslySelectedIndex = this.queueListView.SelectedIndex;
                    if (previouslySelectedIndex > 0 || (this.queueListView.Items.Count - 1) > 0)
                    {
                        var finalizedCollection = this.MainProgramElements.Videos;
						var messageBox = Xceed.Wpf.Toolkit.MessageBox.Show(string.Format(CultureInfo.InstalledUICulture, "Are you sure you want to permanently delete url '{0}'?", finalizedCollection[previouslySelectedIndex].Location), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                        if (messageBox == MessageBoxResult.Yes)
                        {
                            finalizedCollection.RemoveAt(previouslySelectedIndex);
                            for (int videoPosition = previouslySelectedIndex; videoPosition < finalizedCollection.Count; videoPosition++)
                            {
                                finalizedCollection[videoPosition].Position = videoPosition;
                            }
                            RefreshQueue(finalizedCollection, previouslySelectedIndex < this.MainProgramElements.Videos.Count - 1 ? previouslySelectedIndex : this.MainProgramElements.Videos.Count - 1);
                        }
                    }
                    else
                    {
                        Xceed.Wpf.Toolkit.MessageBox.Show("Could not delete final URL or no URL selected.", "Cannot Delete URL", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    }
                    break;
            }
        }

        void DownloadButton_Click (object sender, RoutedEventArgs e)
        {
	    	Task.Run(() => { (new ClassContainer ()).DownloadingCode.DownloadHandler(this.MainProgramElements); });
        }

        void aboutMenuItem_Click (object sender, RoutedEventArgs e)
        {
			this.embeddedLibraries = this.embeddedLibraries ?? new ProjectAssemblies(true);
			(new About (this, FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location), this.embeddedLibraries)).Show();
			if (this.embeddedLibraries.RecallIsSafe) this.embeddedLibraries = null;
            this.MainProgramElements.WindowEnabled = false;
        }
    }

    public class MainProgramElements : INotifyPropertyChanged
    {
        private readonly MainWindow main;
        private int downProgress = 0;
        private int selectedQueue = 0;
        private string downOutput = string.Empty;
        //private bool areUpDownEnabled = false;
        private bool isWindowEnabled = true;
        
        public void RefreshQueue (Collection<Video> collectionToAssimilate, int newIndex)
        {
        	this.main.RefreshQueue(collectionToAssimilate, newIndex);
        }

        /// <summary>
        /// The enabled status of this window.
        /// </summary>
        /// <remarks>
        /// The setting of this value is thread-safe. The getting is not.
        /// </remarks>
        public bool WindowEnabled
        {
            get
            {
                return this.isWindowEnabled;
            }
            set
            {
            	this.main.Dispatcher.Invoke((Action)(() =>
            	{
	                this.isWindowEnabled = value;
                	this.CurrentDownloadProgress = 0;
            	}));
                RaisePropertyChanged("WindowEnabled");
            }
        }

        /// <summary>
        /// The video queue.
        /// </summary>
        /// <remarks>
        /// This is a fully thread-safe operation.
        /// </remarks>
        public ObservableCollection<Video> Videos
        {
            get
            {
            	ObservableCollection<Video> returnValue = new ObservableCollection<Video>();
            	this.main.queueListView.Dispatcher.Invoke((Action)(() =>
            	{
	                this.main.ValidateAvailabilityOfSpecialItems(this.main.VideoQueue.Items);
	                returnValue = this.main.VideoQueue.Items;
            	}));
            	return returnValue;
            }
            set
            {
            	this.main.queueListView.Dispatcher.Invoke((Action)(() => this.main.VideoQueue.Items.Replace(value)));
                RaisePropertyChanged("Videos");
            }
        }

        /// <summary>
        /// The currently selected queue item for this window.
        /// </summary>
        /// <remarks>
        /// This is a fully thread-safe operation.
        /// </remarks>
        public int CurrentlySelectedQueueIndex
        {
            get
            {
            	int returnValue = 0;
            	this.main.queueListView.Dispatcher.Invoke((Action)(() => returnValue = this.Videos.Any() ? this.selectedQueue : -1));
                return returnValue;
            }
            set
            {
            	this.main.queueListView.Dispatcher.Invoke((Action)(() =>
            	{
	                this.selectedQueue = value;
                	this.main.queueListView.ScrollIntoView(this.main.queueListView.Items.GetItemAt(value));
            	}));
                RaisePropertyChanged("CurrentlySelectedQueueIndex");
            }
        }

        /// <summary>
        /// The download progress for the current download (if any).
        /// </summary>
        /// <remarks>
        /// This is a fully thread-safe operation.
        /// </remarks>
        public int CurrentDownloadProgress
        { 
            get
            {
            	int returnValue = 0;
            	this.main.downloadProgressBar.Dispatcher.Invoke((Action)(() => returnValue = this.downProgress));
                return returnValue;
            }
            set
            {
            	this.main.downloadProgressBar.Dispatcher.Invoke((Action)(() => this.downProgress = value));
                RaisePropertyChanged("CurrentDownloadProgress");
            } 
        }

        /// <summary>
        /// The text for the download progress bar in this window.
        /// </summary>
        /// <remarks>
        /// The setting of this value is thread-safe. The getting is not.
        /// </remarks>
        public string CurrentDownloadOutputText
        { 
            get
            {
                return this.downOutput;
            }
            set
            {
            	this.main.downloadProgressText.Dispatcher.Invoke((Action)(() => this.downOutput = value));
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

        private void RaisePropertyChanged (string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs (propertyName));
            }
        }
        #endregion
        
        /// <summary>
        /// Container for the UI values for the parent window of the entire program.
        /// </summary>
        /// <param name="mainWindow">
        /// The parent window for the entire program.
        /// </param>
        public MainProgramElements (MainWindow mainWindow)
        {
            this.main = mainWindow;
        }
    }
}