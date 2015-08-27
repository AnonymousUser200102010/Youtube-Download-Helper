using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            this.MainProgramElements.Videos.Replace(convertedCollection);
            this.MainProgramElements.Videos.WriteToFile(Storage.File);
            this.MainProgramElements.CurrentlySelectedQueueIndex = newIndex;
            this.queueListView.ScrollIntoView(this.queueListView.SelectedItem);
        }

        public void ValidateMoveButtonAvailability (ObservableCollection<Video> collectionToCheck)
        {
            var countCheck = collectionToCheck.Count > 1;
            this.moveQueuedItemDown.IsEnabled = countCheck; 
            this.moveQueuedItemUp.IsEnabled = countCheck;
        }

        public MainWindow (bool downloadImmediately)
        {
        	IOFunc.CreateFolderTree(Storage.Folder);
            this.MainProgramElements = new MainProgramElements (this as MainWindow);
            this.DataContext = MainProgramElements;
            InitializeComponent();
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
		    if (this.MainProgramElements.Videos.Any())
		    {
	    		this.MainProgramElements.WindowEnabled = false;
	    		int selectedIndex = this.queueListView.SelectedIndex;
	    		Task.Factory.StartNew(() => { (new ClassContainer ()).DownloadingCode.DownloadHandler(this, selectedIndex); });
	    	}
        }

        void aboutMenuItem_Click (object sender, RoutedEventArgs e)
        {
			this.embeddedLibraries = this.embeddedLibraries ?? new ProjectAssemblies(true);
            (new About (System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location), this.embeddedLibraries)).Show();
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

        /// <summary>
        /// The enabled status of this window.
        /// </summary>
        public bool WindowEnabled
        {
            get
            {
                return this.isWindowEnabled;
            }
            set
            {
                this.isWindowEnabled = value;
                if (!value)
                {
                	this.CurrentDownloadOutputText = "Starting Downloading Process....";
                }
                this.CurrentDownloadProgress = 0;
                RaisePropertyChanged("WindowEnabled");
            }
        }

        /// <summary>
        /// The video queue.
        /// </summary>
        public ObservableCollection<Video> Videos
        {
            get
            {
                this.main.ValidateMoveButtonAvailability(this.main.VideoQueue.Items);
                return this.main.VideoQueue.Items;
            }
            set
            {
            	this.main.VideoQueue.Items.Replace(value);
                RaisePropertyChanged("Videos");
            }
        }

        /// <summary>
        /// The currently selected queue item for this window.
        /// </summary>
        public int CurrentlySelectedQueueIndex
        {
            get
            {
                return this.Videos.Any() ? selectedQueue : -1;
            }
            set
            {
                selectedQueue = value;
                RaisePropertyChanged("CurrentlySelectedQueueIndex");
            }
        }

        /// <summary>
        /// The download progress for the current download (if any).
        /// </summary>
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

        /// <summary>
        /// The text for the download progress bar in this window.
        /// </summary>
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