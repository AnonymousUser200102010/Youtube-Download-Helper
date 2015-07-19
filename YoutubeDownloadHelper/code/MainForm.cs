using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using UniversalHandlersLibrary;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form, IMainForm
    {
		
    	private MainForm mainForm;
    	
        private IMainForm iMainForm;
        
        private IValidation Validation;
        
        private IDownload Download;
        
        private IStorage Storage;

        private const string exampleLink = "Example: https://www.youtube.com/watch?v=ft6rWcJIlpU";

        private readonly string[] videoFormats = {
            "Mp4", 
            "Flash", 
            "Mobile", 
            "WebM"
        };

        private readonly string[] audioFormats = {
            "Mp3", 
            "Aac", 
            "Vorbis"
        };
        
        private readonly Icon formIcon = (new System.ComponentModel.ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")) as Icon;
        
        private readonly Image[] imageList = {
        	(new System.ComponentModel.ComponentResourceManager(typeof(MainForm)).GetObject("FolderImage") as Image)
        };

        private const decimal defaultResolution = 360;

        private bool currentlyDownloadingVar;

        public MainForm (bool downloadImmediately)
        {
        	
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            this.Icon = formIcon;
            
            this.changeSaveLocation.BackgroundImage = imageList[0];
            
            this.changeTemporaryLocation.BackgroundImage = imageList[0];
			
            this.iMainForm = (this as IMainForm);
            
            this.mainForm = this;
            
            this.Validation = (new Validation() as IValidation);
            
            this.Storage = (new Storage(this.iMainForm, this.Validation) as IStorage);
            
            this.Download = (new Download(this.iMainForm, this.Storage) as IDownload);
            
            Storage.ReadFromRegistry();
            
            GlobalVariables.urlList = Storage.ReadUrlList();
            
            this.RefreshQueue(0);
            
            writeFileVersionToStatBar();
  			
            if (this.queuedBox.Items.Count >= 1)
            {
            	
                this.urlToModify.Text = GlobalVariables.urlList [0].Item1;
            	
                this.resolutionToModify.Value = GlobalVariables.urlList [0].Item2;
                
                this.formatToModify.SelectedItem = GetVideoFormat(GlobalVariables.urlList [0].Item3);
                
                if (downloadImmediately)
                {
                	
                    StartDownloadButtonClick(null, null);
                	
                }
          	  
            }
            
            SchedulingEnabledCheckedChanged(null, null);
			
        }

        #region Private Properties and Independant Methods
        
        private readonly string mainProgramFileVers = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            
        private readonly string youTEFileVers = FileVersionInfo.GetVersionInfo("YoutubeExtractor.dll").FileVersion;

        /// <summary>
        /// Writes to the status bar. In this case, it writes all "version" information deemed necessary for the user to see.
        /// </summary>
        private void writeFileVersionToStatBar ()
        {
            
            this.statusLabel.ForeColor = Color.Black;
            
            this.statusLabel.Text = string.Format(CultureInfo.InstalledUICulture, "Youtube Download Helper v {0} | Youtube Extractor v {1}", mainProgramFileVers, youTEFileVers);
        	
        }

        /// <summary>
        /// Transforms the selected format parameter into it's logical equivalent.
        /// </summary>
        /// <param name="formatType">
        /// The parameter to convert.
        /// </param>
        /// <returns>
        /// The name of the VideoType provided as a string.
        /// </returns>
        private static string GetVideoFormat (VideoType formatType)
        {
        	
            if (formatType == VideoType.Mp4)
            {
        		
                return "Mp4";
        		
            }
        	
            if (formatType == VideoType.Flash)
            {
        		
                return "Flash";
        		
            }
        	
            if (formatType == VideoType.Mobile)
            {
        		
                return "Mobile";
        		
            }
        	
            if (formatType == VideoType.WebM)
            {
        		
                return "WebM";
        		
            }
        	
            throw new ArgumentException ("getVideoFormat: VideoType format not recognized.");
        	
        }

        /// <summary>
        /// Transforms a string variable into it's AudioType equivalent.
        /// </summary>
        /// <param name="formatString">
        /// The name of the AudioType you wish returned.
        /// </param>
        /// <returns>
        /// The AudioType most closely related to the string provided.
        /// </returns>
        private static AudioType getAudioFormatFromString (string formatString)
        {
        	
            if (formatString.Contains("mp3", StringComparison.OrdinalIgnoreCase))
            {
        		
                return AudioType.Mp3;
        		
            }
        	
            if (formatString.Contains("aac", StringComparison.OrdinalIgnoreCase))
            {
        		
                return AudioType.Aac;
        		
            }
        	
            if (formatString.Contains("vorbis", StringComparison.OrdinalIgnoreCase))
            {
        		
                return AudioType.Vorbis;
        		
            }
        	
            throw new ArgumentException ("getAudioFormatFromString: format not recognized.");
        	
        }

        /// <summary>
        /// Reset the values in the "New" tab (for the queued urls).
        /// </summary>
        private void resetNewUrlValues ()
        {
        	
            NewQueuedText = exampleLink;
        	
            newUrlRes.Value = defaultResolution;
            
            newUrlFormat.SelectedIndex = 0;
        	
        }
        
        private string NewQueuedText
        {
        	
            get { return this.newURL.Text; }
        	
            set
            { 
        		
                this.newURL.Text = value;
        			
                this.newURL.TextAlign = HorizontalAlignment.Center;
        		
                this.newURL.Font = new Font ("Microsoft Sans Serif", 7.8f, FontStyle.Italic);
        			
            }
        	
        }

        #endregion

        #region Public Properties and Independant Methods

        public int UrlListNumberItems
        {
			
            get { return mainForm.queuedBox.Items.Count; }
			
        }

        public bool CurrentlyDownloading
        {
        	
            get { return mainForm.currentlyDownloadingVar; }
        	
            set { mainForm.currentlyDownloadingVar = value; }
        	
        }

        public void RefreshQueue (int previouslySelectedIndex)
        {
        	
            mainForm.queuedBox.Items.Clear();
        	
            if (GlobalVariables.urlList.Count > 0)
            {
        		
                for (int count = 0, GlobalVariablesurlListCount = GlobalVariables.urlList.Count; count < GlobalVariablesurlListCount; count++)
                {
					
                    Tuple<string, int, VideoType> url = GlobalVariables.urlList [count];
					
                    AddToQueue(url);
					
                }
	        	
                mainForm.queuedBox.SelectedIndex = previouslySelectedIndex;
	        	
            }
        	
        }

        public void AddToQueue (Tuple<string, int, VideoType> queueTuple)
        {
        	
            mainForm.queuedBox.Items.Add(string.Format(CultureInfo.InstalledUICulture, "{0}: {1} [Resolution: {2}p][Format: {3}]", mainForm.queuedBox.Items.Count + 1, queueTuple.Item1, queueTuple.Item2, queueTuple.Item3));
        	
        }

        internal static VideoType GetVideoFormat (string value)
        {
        	
            if (value.Contains("mp4", StringComparison.OrdinalIgnoreCase))
            {
        		
                return VideoType.Mp4;
        		
            }
        	
            if (value.Contains("flash", StringComparison.OrdinalIgnoreCase))
            {
        		
                return VideoType.Flash;
        		
            }
        	
            if (value.Contains("mobile", StringComparison.OrdinalIgnoreCase))
            {
        		
                return VideoType.Mobile;
        		
            }
        	
            if (value.Contains("webm", StringComparison.OrdinalIgnoreCase))
            {
        		
                return VideoType.WebM;
        		
            }
        	
            throw new ArgumentException ("getVideoFormat: String format not recognized.");
        	
        }

        public int SelectedQueueIndex
        {
        	
            get { return mainForm.queuedBox.SelectedIndex; }
        	
            set { mainForm.queuedBox.SelectedIndex = value; }
        	
        }

        public string StatusBar
        {
        	
            set
            { 
        		
                mainForm.statusLabel.Text = value;
        		
                mainForm.statusLabel.ForeColor = Color.Red;
        	
            }
        	
        }

        public string DownloadLocation
        {
			
            get { return mainForm.saveLocation.Text; }
            
            set { mainForm.saveLocation.Text = value; }
			
        }

        public string TempDownloadLocation
        {
			
            get { return mainForm.temporaryLocation.Text; }
            
            set { mainForm.temporaryLocation.Text = value; }
			
        }

        public bool Scheduling
        {
        	
            get { return mainForm.schedulingEnabled.Checked; }
        	
            set { mainForm.schedulingEnabled.Checked = value; }
        	
        }

        public string SchedulingStart
        {
        	
            get { return mainForm.schedulerTimePicker1.Value.ToString("hh:mm:ss", CultureInfo.CurrentCulture); }
        	
            set { mainForm.schedulerTimePicker1.Value = DateTime.Parse(value, CultureInfo.InvariantCulture); }
        	
        }

        public string SchedulingEnd
        {
        	
            get { return mainForm.schedulerTimePicker2.Value.ToString("hh:mm:ss", CultureInfo.CurrentCulture); }
        	
            set { mainForm.schedulerTimePicker2.Value = DateTime.Parse(value, CultureInfo.InvariantCulture); }
        	
        }

        public void StartDownloadingSession (bool initiate)
        {
        	
            foreach (Control controlItem in mainForm.Controls)
            {
        		
                if (!controlItem.Name.Contains("res", StringComparison.OrdinalIgnoreCase) && !controlItem.Name.Equals("statusLabel", StringComparison.OrdinalIgnoreCase) && !controlItem.Name.Equals("statusStrip", StringComparison.OrdinalIgnoreCase) && !controlItem.Name.Equals("downloadButton", StringComparison.OrdinalIgnoreCase))
                {
                		
                    controlItem.Enabled = !initiate;
                        
                }
        			
            }
                
            mainForm.downloadButton.Text = !CurrentlyDownloading ? "Start Downloading" : "Stop Downloading";
                
            mainForm.DownloadLabel = null;
        		
            mainForm.DownloadFinishedPercent = 0;
        	
        }

        public bool StartDownButtonEnabled
        {
        	
            get { return mainForm.downloadButton.Enabled; }
            
            set { mainForm.downloadButton.Enabled = value; }
        	
        }
        
        public int DownloadFinishedPercent
        {
        	
            set { mainForm.videoDownloadProgressBar.Value = value; }
        	
        }

        public string DownloadLabel
        {
        	
            set { mainForm.downloadingLabel.Text = value; }
        	
        }

        #endregion

        #region Form Methods

        private void SchedulingEnabledCheckedChanged (object sender, EventArgs e)
        {
			
            schedulerTimePicker1.Visible = schedulingEnabled.Checked;
			
            schedulerToLabel.Visible = schedulingEnabled.Checked;
			
            schedulerTimePicker2.Visible = schedulingEnabled.Checked;
            
            OptionChanged(sender, e);
			
        }

        private void AddURLButtonClick (object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(newURL.Text) && !newURL.Text.Contains(exampleLink) && newUrlRes.Value >= 144 && newUrlRes.Value < 720)
            {
            	
            	int resolutionReal = (int)this.newUrlRes.Value;
            	
                if (this.queuedBox.Items.Count > 0)
                {
            		
                    resolutionReal = (int)(GetVideoFormat(GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item3).Contains("Mp4", StringComparison.OrdinalIgnoreCase) ? 360 : this.newUrlRes.Value);
            		
                }
				
                var newUrlTuple = new Tuple<string, int, VideoType> (this.newURL.Text, resolutionReal, GetVideoFormat(this.newUrlFormat.Text));
				
                this.AddToQueue(newUrlTuple);
			
                GlobalVariables.urlList.Add(newUrlTuple);
				
                Storage.WriteUrlsToFile(GlobalVariables.urlList, false);
                
                this.SelectedQueueIndex = this.queuedBox.Items.Count - 1;
				
            }
            else if (newUrlRes.Value >= 720)
            {
				
                MessageBox.Show("HD video resolution is currently unsupported", "Resolution Unsupported", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
            }
            else
            {
				
                MessageBox.Show("Some or all values of the new URL are invalid.", "URL Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
            }
			
            this.resetNewUrlValues();
            
            
			
        }

        private void NewURLEnter (object sender, EventArgs e)
        {
			
            this.newURL.Clear();
        			
            this.newURL.TextAlign = HorizontalAlignment.Left;
        		
            this.newURL.Font = new Font ("Microsoft Sans Serif", 7.8f, FontStyle.Regular);
			
        }

        private void NewURLLeave (object sender, EventArgs e)
        {
			
            if (string.IsNullOrWhiteSpace(newURL.Text))
            {
				
                this.resetNewUrlValues();
				
            }
			
        }

        private void DeleteButtonClick (object sender, EventArgs e)
        {
			
            int previouslySelectedIndex = this.queuedBox.SelectedIndex;
			
            if (this.queuedBox.SelectedIndex > 0 || (this.queuedBox.Items.Count - 1) > 0)
            {
        		
                DialogResult deletionAnswer = MessageBox.Show(string.Format(CultureInfo.InstalledUICulture, "Are you sure you want to delete {0}?", GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item1), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        		
                if (deletionAnswer == DialogResult.Yes)
                {
        			
                    GlobalVariables.urlList.RemoveAt(this.queuedBox.SelectedIndex);
					
                    this.queuedBox.Items.RemoveAt(this.queuedBox.SelectedIndex);
					
                    Storage.WriteUrlsToFile(GlobalVariables.urlList, false);
	                
                    RefreshQueue(previouslySelectedIndex > (this.queuedBox.Items.Count - 1) ? (this.queuedBox.Items.Count - 1) : previouslySelectedIndex);
	                
                }
				
            }
            else
            {
				
                MessageBox.Show("Could not delete final URL or no URL selected.", "Cannot Delete URL", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
            }
			
        }

        private void QueuedBoxSelectedIndexChanged (object sender, EventArgs e)
        {
			
            if (this.queuedBox.SelectedIndex > -1)
            {
            	
                this.urlToModify.Text = GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item1;
            	
                this.resolutionToModify.Value = GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item2;
                
                this.formatToModify.SelectedItem = GetVideoFormat(GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item3);
                
                this.resolutionToModify.Enabled = !GetVideoFormat(GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item3).Contains("Mp4", StringComparison.OrdinalIgnoreCase);
          	  
            }
			
        }

        private void SubmitModificationButtonClick (object sender, EventArgs e)
        {
			
            int previouslySelected = this.queuedBox.SelectedIndex;
        	
            if (!string.IsNullOrWhiteSpace(urlToModify.Text) && resolutionToModify.Value > 143 && resolutionToModify.Value < 720)
            {
				
                GlobalVariables.urlList.RemoveAt(this.queuedBox.SelectedIndex);
				
                GlobalVariables.urlList.Insert(this.queuedBox.SelectedIndex, new Tuple<string, int, VideoType> (urlToModify.Text, (int)resolutionToModify.Value, GetVideoFormat(this.formatToModify.Text)));
				
                Storage.WriteUrlsToFile(GlobalVariables.urlList, false);
				
            }
            else if (resolutionToModify.Value >= 720)
            {
				
                MessageBox.Show("HD video resolution is currently unsupported", "Resolution Unsupported", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
                QueuedBoxSelectedIndexChanged(sender, e);
				
            }
            else
            {
				
                MessageBox.Show("Some or all values of the new URL are invalid.", "URL Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
                QueuedBoxSelectedIndexChanged(sender, e);
				
            }
            
            RefreshQueue(previouslySelected);
			
        }

        protected internal delegate void DDown (int retryCount, Collection<Tuple<string, int, VideoType>> urlList);

        private void StartDownloadButtonClick (object sender, EventArgs e)
        {
            
            if (this.queuedBox.Items.Count > 0 && !CurrentlyDownloading)
            {
            	
                writeFileVersionToStatBar();
            	
                this.Validation.CheckOrCreateFolder(this.DownloadLocation);
    		
                this.Validation.CheckOrCreateFolder(this.TempDownloadLocation);
            	
                var tempDelegate = new DDown (Download.SetupDownloadingProcess);
    			
                tempDelegate.BeginInvoke(0, GlobalVariables.urlList, null, null);
                
            }
            else if (this.queuedBox.Items.Count > 0 && CurrentlyDownloading)
            {
            	
                this.downloadButton.Text = "Stopping....";
            	
                this.downloadButton.Enabled = false;
            	
            }
            else
            {
            	
                CurrentlyDownloading = false;
            	
            }
            
            CurrentlyDownloading = !CurrentlyDownloading;
			
        }

        private void OptionChanged (object sender, EventArgs e)
        {
			
            Storage.WriteToRegistry();
			
        }

        private void ChangeSaveLocationClick (object sender, EventArgs e)
        {
			
            DialogResult result = saveLocationDialog.ShowDialog();
			
            if (result == DialogResult.OK)
            {
					
                saveLocation.Text = saveLocationDialog.SelectedPath;
					
            }
			
        }

        private void ChangeTempSaveLocationClick (object sender, EventArgs e)
        {
			
            DialogResult result = tempLocationDialog.ShowDialog();
			
            if (result == DialogResult.OK)
            {
					
                temporaryLocation.Text = tempLocationDialog.SelectedPath;
					
            }
			
        }

        private void FormatToModifySelectedIndexChanged (object sender, EventArgs e)
        {
			
            this.resolutionToModify.Enabled = !formatToModify.SelectedItem.ToString().Contains("Mp4", StringComparison.OrdinalIgnoreCase);
			
            if (formatToModify.SelectedItem.ToString().Contains("Mp4", StringComparison.OrdinalIgnoreCase))
            {
				
                this.resolutionToModify.Value = defaultResolution;
				
            }
			
        }

        #endregion
		
    }

}
