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
        	(new System.ComponentModel.ComponentResourceManager(typeof(MainForm)).GetObject("FolderImage") as Image),
        	(new System.ComponentModel.ComponentResourceManager(typeof(MainForm)).GetObject("UpArrowImage") as Image),
        	(new System.ComponentModel.ComponentResourceManager(typeof(MainForm)).GetObject("DownArrowImage") as Image)
        };

        private const decimal defaultResolution = 360;

        private bool currentlyDownloadingVar;

        public MainForm (bool downloadImmediately)
        {
        	
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            InitiateStartParams(downloadImmediately);
			
        }

        #region Private Properties and Independant Methods
        
        private readonly string mainProgramFileVers = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            
        private readonly string youTEFileVers = FileVersionInfo.GetVersionInfo("YoutubeExtractor.dll").FileVersion;
        
        private void InitiateStartParams(bool downloadImmediately)
        {
        	
        	this.Icon = formIcon;
            
            this.changeSaveLocation.BackgroundImage = imageList[0];
            
            this.changeTemporaryLocation.BackgroundImage = imageList[0];
            
            this.moveQueuedItemUp.BackgroundImage = imageList[1];
            
            this.moveQueuedItemDown.BackgroundImage = imageList[2];
			
            this.iMainForm = (this as IMainForm);
            
            this.mainForm = this;
            
            this.Validation = (new Validation() as IValidation);
            
            this.Storage = (new Storage(this.iMainForm, this.Validation) as IStorage);
            
            this.Download = (new Download(this.iMainForm, this.Storage) as IDownload);
            
            Storage.ReadFromRegistry();
            
            VideoQueue.Items = Storage.ReadUrls();
            
            this.RefreshQueue(0, false);
            
            writeFileVersionToStatBar();
  			
            if (this.queuedBox.Items.Count >= 1)
            {
            	
                this.urlToModify.Text = VideoQueue.Items [0].UrlName;
            	
                this.resolutionToModify.Value = VideoQueue.Items [0].Resolution;
                
                this.formatToModify.SelectedItem = VideoQueue.Items [0].Format.ToString();
                
                if (downloadImmediately)
                {
                	
                    StartDownloadButtonClick(null, null);
                	
                }
          	  
            }
            
            SchedulingEnabledCheckedChanged(null, null);
        	
        }

        /// <summary>
        /// Writes to the status bar. In this case, it writes all "version" information deemed necessary for the user to see.
        /// </summary>
        private void writeFileVersionToStatBar ()
        {
            
            this.statusLabel.ForeColor = Color.Black;
            
            this.statusLabel.Text = string.Format(CultureInfo.InstalledUICulture, "Youtube Download Helper v {0} | Youtube Extractor v {1}", mainProgramFileVers, youTEFileVers);
        	
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

        public int UrlsNumberItems
        {
			
            get { return mainForm.queuedBox.Items.Count; }
			
        }

        public bool CurrentlyDownloading
        {
        	
            get { return mainForm.currentlyDownloadingVar; }
        	
            set { mainForm.currentlyDownloadingVar = value; }
        	
        }

        public void RefreshQueue (int previouslySelectedIndex, bool forceFocusOnQueue)
        {
        	
            mainForm.queuedBox.Items.Clear();
        	
            if (VideoQueue.Items.Count > 0)
            {
        		
                for (int count = 0, numberOfVideos = VideoQueue.Items.Count; count < numberOfVideos; count++)
                {
					
                    Video url = VideoQueue.Items [count];
					
                    AddToQueue(url);
					
                }
	        	
                mainForm.SelectedQueueIndex = previouslySelectedIndex;
	        	
            }
            
            if(forceFocusOnQueue)
            {
            	
            	mainForm.queuedBox.Focus();
            	
            }
        	
        }

        public void AddToQueue (Video queueTuple)
        {
            
            ListViewItem lvi = mainForm.queuedBox.Items.Add((mainForm.queuedBox.Items.Count + 1).ToString(CultureInfo.CurrentCulture));
            
            lvi.SubItems.Add(queueTuple.UrlName);
            
            lvi.SubItems.Add(queueTuple.Resolution.ToString(CultureInfo.CurrentCulture));
            
            lvi.SubItems.Add(queueTuple.Format.ToString());
        	
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
        	
        	get 
        	{
        		
				return mainForm.queuedBox.SelectedItems.Count > 0 ? mainForm.queuedBox.Items.IndexOf(mainForm.queuedBox.SelectedItems[0]) : 0;
        		
        	
        	}
        	
        	set 
        	{ 
        		
        		mainForm.queuedBox.Items[value].Selected = true;
        		
        		mainForm.queuedBox.Items[value].Focused = true;
        		
        		mainForm.queuedBox.Items[value].EnsureVisible();
        	
        	}
        	
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
        	
			for (int count = 0, controls = mainForm.Controls.Count; count < controls; count++)
			{
				Control controlItem = mainForm.Controls[count];
				
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
            	
                var newUrlTuple = new Video (this.newURL.Text, (int)this.newUrlRes.Value, GetVideoFormat(this.newUrlFormat.Text));
				
                this.AddToQueue(newUrlTuple);
			
                VideoQueue.Items.Add(newUrlTuple);
				
                Storage.WriteUrlsToFile(VideoQueue.Items, false);
                
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
			
            int previouslySelectedIndex = this.SelectedQueueIndex;
			
            if (this.SelectedQueueIndex > 0 || (this.queuedBox.Items.Count - 1) > 0)
            {
        		
                DialogResult deletionAnswer = MessageBox.Show(string.Format(CultureInfo.InstalledUICulture, "Are you sure you want to delete {0}?", VideoQueue.Items [this.SelectedQueueIndex].UrlName), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        		
                if (deletionAnswer == DialogResult.Yes)
                {
        			
                    VideoQueue.Items.RemoveAt(this.SelectedQueueIndex);
					
                    this.queuedBox.Items.RemoveAt(this.SelectedQueueIndex);
					
                    Storage.WriteUrlsToFile(VideoQueue.Items, false);
	                
                    RefreshQueue(previouslySelectedIndex > (this.queuedBox.Items.Count - 1) ? (this.queuedBox.Items.Count - 1) : previouslySelectedIndex, true);
	                
                }
				
            }
            else
            {
				
                MessageBox.Show("Could not delete final URL or no URL selected.", "Cannot Delete URL", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
            }
			
        }

        private void QueuedBoxSelectedIndexChanged (object sender, EventArgs e)
        {
			
            if (this.SelectedQueueIndex > -1)
            {
            	
                this.urlToModify.Text = VideoQueue.Items [this.SelectedQueueIndex].UrlName;
            	
                this.resolutionToModify.Value = VideoQueue.Items [this.SelectedQueueIndex].Resolution;
                
                this.formatToModify.SelectedItem = VideoQueue.Items [this.SelectedQueueIndex].Format.ToString();
                
                this.resolutionToModify.Enabled = !VideoQueue.Items [this.SelectedQueueIndex].Format.ToString().Contains("Mp4", StringComparison.OrdinalIgnoreCase);
          	  
            }
            
            this.moveQueuedItemDown.Enabled = this.SelectedQueueIndex > -1 && VideoQueue.Items.Count > 1;
	                
	        this.moveQueuedItemUp.Enabled = this.SelectedQueueIndex > -1 && VideoQueue.Items.Count > 1;
			
        }

        private void SubmitModificationButtonClick (object sender, EventArgs e)
        {
			
            int previouslySelected = this.SelectedQueueIndex;
        	
            if (!string.IsNullOrWhiteSpace(urlToModify.Text) && resolutionToModify.Value > 143 && resolutionToModify.Value < 720)
            {
				
                VideoQueue.Items.RemoveAt(this.SelectedQueueIndex);
				
                VideoQueue.Items.Insert(this.SelectedQueueIndex, new Video (urlToModify.Text, (int)resolutionToModify.Value, GetVideoFormat(this.formatToModify.Text)));
				
                Storage.WriteUrlsToFile(VideoQueue.Items, false);
				
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
            
            RefreshQueue(previouslySelected, false);
			
        }

        protected internal delegate void DDown (int retryCount, ObservableCollection<Video> urls);

        private void StartDownloadButtonClick (object sender, EventArgs e)
        {
            
            if (this.queuedBox.Items.Count > 0 && !CurrentlyDownloading)
            {
            	
                writeFileVersionToStatBar();
            	
                this.Validation.CheckOrCreateFolder(this.DownloadLocation);
    		
                this.Validation.CheckOrCreateFolder(this.TempDownloadLocation);
            	
                var downloadDelegate = new DDown (Download.HandleDownloadingProcesses);
    			
                downloadDelegate.BeginInvoke(0, VideoQueue.Items, null, null);
                
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
			
        	string senderName = ((Control)sender).Name;
        	
        	DialogResult result = senderName.Contains("temp", StringComparison.OrdinalIgnoreCase) ? tempLocationDialog.ShowDialog() : saveLocationDialog.ShowDialog();
			
            if (result == DialogResult.OK)
            {
				
            	if(senderName.Contains("temp", StringComparison.OrdinalIgnoreCase))
            	{
            		
            		temporaryLocation.Text = tempLocationDialog.SelectedPath;
            		
            	}
            	else
            	{
            		
            		saveLocation.Text = saveLocationDialog.SelectedPath;
            		
            	}
					
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
        
		void NewUrlFormatSelectedIndexChanged(object sender, EventArgs e)
		{
			
			this.newUrlRes.Enabled = this.newUrlFormat.SelectedIndex != 0;
			
		}
		
		void MoveQueuedItem(object sender, EventArgs e)
		{
			
			int newlySelectedIndex = ((Control)sender).Name.Contains("down", StringComparison.OrdinalIgnoreCase) 
				?
					this.SelectedQueueIndex + 1 < VideoQueue.Items.Count - 1 ? this.SelectedQueueIndex + 1 : VideoQueue.Items.Count - 1 
				: 
					this.SelectedQueueIndex - 1 > 0 ? this.SelectedQueueIndex - 1 : 0;
			
			VideoQueue.Items.Move(this.SelectedQueueIndex, newlySelectedIndex);
			
			Storage.WriteUrlsToFile(VideoQueue.Items, false);
			
			this.RefreshQueue(newlySelectedIndex, true);
			
		}

        #endregion
		
    }

}
