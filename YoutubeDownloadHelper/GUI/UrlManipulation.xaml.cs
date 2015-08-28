using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Linq;
using YoutubeExtractor;
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
        private readonly UrlShaping urlShapingVars = new UrlShaping ();
        private Collection<Video> videoQueue;
        private readonly bool add;
        private int persistantUrlIndex;

        /// <summary>
        /// Url Manipulation window.
        /// </summary>
        /// <param name="add">
        /// This is the Url Adding window.
        /// </param>
        /// <param name="mainWindow">
        /// The parent window.
        /// </param>
        /// <param name="itemsToUse">
        /// Video collection to manipulate.
        /// </param>
        /// <param name="selectedUrl">
        /// Selected item in the video collection.
        /// </param>
        public UrlManipulation (bool add, MainWindow mainWindow, ObservableCollection<Video> itemsToUse, int selectedUrl)
        {
            this.DataContext = this.urlShapingVars;
            InitializeComponent();
            this.videoQueue = itemsToUse;
            this.MainWindow = mainWindow;
            this.persistantUrlIndex = selectedUrl;
            this.add = add;
            this.Title = string.Format(CultureInfo.CurrentCulture, "{0} Url(s) {1} the Queue", add ? "Add" : "Modify", add ? "to" : "in");
            this.basicManipulateUrlButton.Content = add ? "Add Url to the Queue" : "Modify Current Url";

			if (!add && videoQueue.Any())
			{
				this.avTextBox.Text = string.Join(string.Empty, videoQueue);
				this.urlShapingVars.BasicText = videoQueue[this.persistantUrlIndex].Location;
				this.urlShapingVars.AudioOnlyEnabled = videoQueue[this.persistantUrlIndex].IsAudioFile;
				this.urlShapingVars.SelectedFormat = this.formatComboBox.Items.IndexOf(videoQueue[this.persistantUrlIndex].Format);
				this.urlShapingVars.SelectedResolution = videoQueue[this.persistantUrlIndex].Quality;
			}
			else this.avTextBox.Text = string.Format(CultureInfo.CurrentCulture, "{0} {1} Mp4 False (REMOVE!)", UrlShaping.ExampleText, UrlShaping.DefaultResolution);
        }

        private void window1_Closed (object sender, EventArgs e)
        {
            this.MainWindow.RefreshQueue(videoQueue, persistantUrlIndex);
            this.MainWindow.MainProgramElements.WindowEnabled = true;
        }

        private void window1_Closing (object sender, EventArgs e)
        {
            if (advancedView.IsSelected)
            {
                try
                {
                    var urlList = new Collection<string> (this.avTextBox.Text.Split(new [] {
                        "\n",
                        Environment.NewLine
                    }, StringSplitOptions.RemoveEmptyEntries));
                    var newItems = urlList.ConvertToVideoCollection(add ? videoQueue.Count : 0);
                    videoQueue.Replace(add ? new ObservableCollection<Video> (videoQueue.Union(newItems)) : newItems);
                }
                catch (Exception ex) 
                { 
                	this.basicUserInfoText.Text = ex.Message.Truncate(20);
            		(new ClassContainer()).BakedExceptionCode.Alert(ex);
                	throw new FatalException("A fatal exception has occured.", ex);
                }
            }
        }

        private void basicManipulateUrlButton_Click (object sender, RoutedEventArgs e)
        {
            var basicQualityValue = (int)this.urlShapingVars.SelectedResolution;
			var formatType = this.urlShapingVars.AudioOnlyEnabled ? typeof(AudioType).ToString() : typeof(VideoType).ToString();
			var formatAsString = formatType.Contains("audio", StringComparison.OrdinalIgnoreCase) ? "audio track" : "video";
			
            if (!string.IsNullOrWhiteSpace(this.urlShapingVars.BasicText) && !this.urlShapingVars.BasicText.Contains("example", StringComparison.OrdinalIgnoreCase) && basicQualityValue >= UrlShaping.MinimumQuality[formatType] && basicQualityValue <= UrlShaping.MaximumQuality)
            {
                try
                {
                    Collection<Video> finalizedCollection = videoQueue;
                    VideoType format = this.urlShapingVars.AudioOnlyEnabled ? VideoType.Mp4 : (VideoType)Enum.Parse(typeof(VideoType), Enum.GetNames(typeof(VideoType)).First(name => name.Contains(this.formatComboBox.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase)));
                    AudioType aFormat = this.urlShapingVars.AudioOnlyEnabled ? (AudioType)Enum.Parse(typeof(AudioType), Enum.GetNames(typeof(AudioType)).First(name => name.Contains(this.formatComboBox.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase))) : AudioType.Mp3;
                    var resultantVideo = new Video (add ? finalizedCollection.Count : persistantUrlIndex, this.urlShapingVars.BasicText, basicQualityValue, format, aFormat, this.urlShapingVars.AudioOnlyEnabled);
					
                    if (add)
                    {
                        finalizedCollection.Add(resultantVideo);
                        this.persistantUrlIndex = finalizedCollection.IndexOf(resultantVideo);
                        clearBasicUrlElements(true);
                    }
                    else
                    {
                        finalizedCollection.RemoveAt(persistantUrlIndex);
                        finalizedCollection.Insert(persistantUrlIndex, resultantVideo);
                    }
                    this.videoQueue = finalizedCollection;
                    this.basicUserInfoText.Text = "Success!";
                }
                catch (Exception ex) 
                { 
                	this.basicUserInfoText.Text = ex.Message.Truncate(20);
            		(new ClassContainer()).BakedExceptionCode.Alert(ex); 
                	throw new FatalException("A fatal exception has occured.", ex);
                }
            }
            else if (basicQualityValue >= UrlShaping.MaximumQuality)
            {
                this.basicUserInfoText.Text = "Generic Error";
                Xceed.Wpf.Toolkit.MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "The quality you set for this {0} is too high.", formatAsString), "Quality Unsupported", MessageBoxButton.OK, MessageBoxImage.Stop);
                this.urlShapingVars.SelectedResolution = UrlShaping.MaximumQuality;
            }
            else if (basicQualityValue < UrlShaping.MinimumQuality[formatType])
            {
            	this.basicUserInfoText.Text = "Generic Error";
                Xceed.Wpf.Toolkit.MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "The quality you set for this {0} is too low.", formatAsString), "Quality Unsupported", MessageBoxButton.OK, MessageBoxImage.Stop);
                this.urlShapingVars.SelectedResolution = UrlShaping.MinimumQuality[formatType];
            }
            else
            {
                this.basicUserInfoText.Text = "Generic Error";
                Xceed.Wpf.Toolkit.MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "Some or all values of the{0} URL are invalid.", add ? " new" : string.Empty), "URL Invalid", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }
        
        private void clearBasicUrlElements (bool selectTextBox)
        {
            if (selectTextBox) this.basicUrlText.Focus();
            this.urlShapingVars.BasicText = string.Empty;
        }

        private void basicUrlText_TextManipulation (object sender, RoutedEventArgs e)
        {
            this.urlShapingVars.BasicText = this.basicUrlText.Text;
            if (this.basicUrlText.IsFocused && this.urlShapingVars.BasicText.Contains("example", StringComparison.OrdinalIgnoreCase))
            {
                this.clearBasicUrlElements(false);
            }
            else if (string.IsNullOrWhiteSpace(this.urlShapingVars.BasicText) || this.urlShapingVars.BasicText.Contains("example", StringComparison.OrdinalIgnoreCase))
            {
                this.urlShapingVars.BasicText = UrlShaping.ExampleText;
            }
        }
    }

    public class UrlShaping : INotifyPropertyChanged
    {
        private readonly IEnumerable<string> videoFormats = new Collection<string> {
            "Mp4",
            "Flash",
            "Mobile",
            "WebM"
        };
        private readonly IEnumerable<string> audioFormats = new Collection<string> {
            "Mp3",
            "Aac",
            "Vorbis"
    	};
        
        private bool audioOnlyCheckBoxChecked = false;
        private TextAlignment basicTextAlign = TextAlignment.Center;
        private FontStyle basicFontStyle = FontStyles.Italic;
        private FontWeight basicFontWeight = FontWeights.Light;

        private string basicUrlTextBoxText { get; set; }
        private int basicSelectedFormat { get; set; }
        private int basicSelectedResolution { get; set; }
        private ObservableCollection<string> formatList { get; set; }
        
        /// <summary>
        /// Returns the lowest quality the user is allowed to set a video to in the basic tab.
        /// </summary>
        public static IDictionary<string, int> MinimumQuality { get { return new Dictionary<string, int> { { typeof(VideoType).ToString(), 144 }, { typeof(AudioType).ToString(), 24 } }; } }
        
        /// <summary>
        /// Returns the highest quality the user is allowed to set a video to in the basic tab.
        /// </summary>
        public static int MaximumQuality { get { return 600; } }

        /// <summary>
        /// The default quality for the program.
        /// </summary>
        public static int DefaultResolution { get { return 360; } }

        /// <summary>
        /// The default url text for the program.
        /// </summary>
        public static string ExampleText { get { return "Example: https://www.youtube.com/watch?v=ft6rWcJIlpU"; } }

        /// <summary>
        /// The format selected in the basic tab of the window.
        /// </summary>
        public int SelectedFormat
        { 
            get { return this.basicSelectedFormat; }
            set
            {
                this.basicSelectedFormat = value;
                RaisePropertyChanged("SelectedFormat");
            } 
        }

        /// <summary>
        /// The selected resolution in the basic tab of the window.
        /// </summary>
        public int SelectedResolution
        { 
            get { return this.basicSelectedResolution; } 
            set
            {
                this.basicSelectedResolution = value;
                RaisePropertyChanged("SelectedResolution");
            } 
        }

        /// <summary>
        /// The url text in the basic tab of the window.
        /// </summary>
        public string BasicText
        {
            get { return this.basicUrlTextBoxText; }
            set
            {
                this.basicUrlTextBoxText = value;
                var containsDefaultText = value.Contains("example", StringComparison.OrdinalIgnoreCase);
                this.BasicUrlTextAlignment = containsDefaultText ? TextAlignment.Center : TextAlignment.Left;
                this.BasicUrlFontStyle = containsDefaultText ? FontStyles.Italic : FontStyles.Normal;
                this.BasicUrlBoldness = containsDefaultText ? FontWeights.Light : FontWeights.Normal;
                if (containsDefaultText)
                {
                    this.AudioOnlyEnabled = false;
                    this.SelectedResolution = UrlShaping.DefaultResolution;
                }
                RaisePropertyChanged("BasicText");
            }
        }

        /// <summary>
        /// The text alignment for the url text in the basic tab of the window.
        /// </summary>
        public TextAlignment BasicUrlTextAlignment
        {
            get { return this.basicTextAlign; }
            set
            {
                this.basicTextAlign = value;
                RaisePropertyChanged("BasicUrlTextAlignment");
            }
        }

        /// <summary>
        /// The font for the url text in the basic tab of the window.
        /// </summary>
        public FontStyle BasicUrlFontStyle
        {
            get { return this.basicFontStyle; }
            set
            {
                this.basicFontStyle = value;
                RaisePropertyChanged("BasicUrlFontStyle");
            }
        }

        /// <summary>
        /// The style for the url text in the basic tab of the window.
        /// </summary>
        public FontWeight BasicUrlBoldness
        {
            get { return this.basicFontWeight; }
            set
            {
                this.basicFontWeight = value;
                RaisePropertyChanged("BasicUrlBoldness");
            }
        }

        /// <summary>
        /// The checked condition of the "Audio only" check box in the basic url tab of this window.
        /// </summary>
        public bool AudioOnlyEnabled
        {
            get { return this.audioOnlyCheckBoxChecked; }
            set
            {
                this.audioOnlyCheckBoxChecked = value;
                this.FormatList = new ObservableCollection<string>(!value ? this.videoFormats : this.audioFormats);
                this.SelectedFormat = 0;
                RaisePropertyChanged("AudioOnlyEnabled");
            }
        }

        /// <summary>
        /// The list of formats being used in the basic tab of this window.
        /// </summary>
        public ObservableCollection<string> FormatList
        {
            get { return this.formatList; }
            set
            {
                this.formatList = value;
                RaisePropertyChanged("FormatList");
            }
        }
        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private void RaisePropertyChanged (string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs (propertyName));
            }
        }
        #endregion
        
        /// <summary>
        /// Container for the UI values for the Url Manipulation window of this program.
        /// </summary>
        public UrlShaping ()
        {
            this.BasicText = UrlShaping.ExampleText;
            this.SelectedFormat = 0;
            this.SelectedResolution = UrlShaping.DefaultResolution;
            this.FormatList = new ObservableCollection<string>(!this.AudioOnlyEnabled ? this.videoFormats : this.audioFormats);
        }
    }
}