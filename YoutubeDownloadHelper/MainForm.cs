using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using UniversalHandlersLibrary;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
		
        public static MainForm mainForm;

        private const string exampleLink = "Example: https://www.youtube.com/watch?v=ft6rWcJIlpU";
        
        private string[] defaultFormats = {
        	string.Format("{0}\n{1}\n{2}\n{3}", "Mp4", "Flash", "Mobile", "WebM"),
        	string.Format("{0}\n{1}\n{2}", "Mp3", "Aac", "Vorbis")
        	
        };

        private const decimal defaultResolution = 360;
        
        public static bool currentlyDownloading;

        public MainForm ()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
			
            mainForm = this as MainForm;
            
            Storage.ReadFromRegistry();
            
            Storage.readUrlList();
            
            MainForm.refreshQueue(0);
            
            FileVersionInfo fviMainProgram = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            
            FileVersionInfo fviYTE = FileVersionInfo.GetVersionInfo("YoutubeExtractor.dll");
            
            statusLabel.Text = string.Format("Youtube Download Helper v {0} | Youtube Extractor v {1}", fviMainProgram.FileVersion, fviYTE.FileVersion);
  			
            if (this.queuedBox.Items.Count >= 1)
            {
            	
                this.urlToModify.Text = GlobalVariables.urlList [0].Item1;
            	
                this.resolutionToModify.Value = GlobalVariables.urlList [0].Item2;
                
                this.formatToModify.SelectedItem = getVideoFormat(GlobalVariables.urlList[0].Item3);
                
                if (GlobalVariables.DownloadImmediately)
                {
                	
                    StartDownloadButtonClick(null, null);
                	
                }
          	  
            }
            
            SchedulingEnabledCheckedChanged(null, null);
			
        }

        #region properties

        public static ListBox urlList
        {
			
            get { return mainForm.queuedBox; }
			
        }

        public static void refreshQueue (int previouslySelectedIndex)
        {
        	
            mainForm.queuedBox.Items.Clear();
        	
            if (GlobalVariables.accessUrlList.Count > 0)
            {
        		
				for (int count = 0, GlobalVariablesurlListCount = GlobalVariables.accessUrlList.Count; count < GlobalVariablesurlListCount; count++)
				{
					
					Tuple<string, int, VideoType> url = GlobalVariables.accessUrlList[count];
					
					addToQueue(url);
					
				}
	        	
                mainForm.queuedBox.SelectedIndex = previouslySelectedIndex;
	        	
            }
        	
        }
        
        public static void addToQueue(Tuple<string, int, VideoType> queueTuple)
        {
        	
        	MainForm.mainForm.queuedBox.Items.Add(string.Format("{0}: {1} [Resolution: {2}p][Format: {3}]", mainForm.queuedBox.Items.Count + 1, queueTuple.Item1, queueTuple.Item2, queueTuple.Item3));
        	
        }
        
        public static VideoType getVideoFormat(string formatString)
        {
        	
        	if(formatString.Contains("mp4", StringComparison.OrdinalIgnoreCase))
        	{
        		
        		return VideoType.Mp4;
        		
        	}
        	
        	if(formatString.Contains("flash", StringComparison.OrdinalIgnoreCase))
        	{
        		
        		return VideoType.Flash;
        		
        	}
        	
        	if(formatString.Contains("mobile", StringComparison.OrdinalIgnoreCase))
        	{
        		
        		return VideoType.Mobile;
        		
        	}
        	
        	if(formatString.Contains("webm", StringComparison.OrdinalIgnoreCase))
        	{
        		
        		return VideoType.WebM;
        		
        	}
        	
        	throw new ArgumentException("getVideoFormat: String format not recognized.");
        	
        }
        
        private string getVideoFormat(VideoType formatType)
        {
        	
        	if(formatType == VideoType.Mp4)
        	{
        		
        		return "Mp4";
        		
        	}
        	
        	if(formatType == VideoType.Flash)
        	{
        		
        		return "Flash";
        		
        	}
        	
        	if(formatType == VideoType.Mobile)
        	{
        		
        		return "Mobile";
        		
        	}
        	
        	if(formatType == VideoType.WebM)
        	{
        		
        		return "WebM";
        		
        	}
        	
        	throw new ArgumentException("getVideoFormat: VideoType format not recognized.");
        	
        }
        
        private AudioType getAudioFormatFromString(string formatString)
        {
        	
        	if(formatString.Contains("mp3", StringComparison.OrdinalIgnoreCase))
        	{
        		
        		return AudioType.Mp3;
        		
        	}
        	
        	if(formatString.Contains("aac", StringComparison.OrdinalIgnoreCase))
        	{
        		
        		return AudioType.Aac;
        		
        	}
        	
        	if(formatString.Contains("vorbis", StringComparison.OrdinalIgnoreCase))
        	{
        		
        		return AudioType.Vorbis;
        		
        	}
        	
        	throw new ArgumentException("getAudioFormatFromString: format not recognized.");
        	
        }

        public static int selectedQueueIndex
        {
        	
            get { return mainForm.queuedBox.SelectedIndex; }
        	
            set { mainForm.queuedBox.SelectedIndex = value; }
        	
        }

        public static string statusBar
        {
        	
            set
            { 
        		
                mainForm.statusLabel.Text = value;
        		
                mainForm.statusLabel.ForeColor = Color.Red;
        	
            }
        	
        }

        public static string downloadLocation
        {
			
            get { return mainForm.saveLocation.Text; }
            
            set { mainForm.saveLocation.Text = value; }
			
        }
        
        public static string tempDownloadLocation
        {
			
            get { return mainForm.temporaryLocation.Text; }
            
            set { mainForm.temporaryLocation.Text = value; }
			
        }

        public static bool scheduling
        {
        	
            get { return mainForm.schedulingEnabled.Checked; }
        	
            set { mainForm.schedulingEnabled.Checked = value; }
        	
        }

        public static string schedulingStart
        {
        	
            get { return mainForm.schedulerTimePicker1.Value.ToString("hh:mm:ss"); }
        	
            set { mainForm.schedulerTimePicker1.Value = DateTime.Parse(value); }
        	
        }

        public static string schedulingEnd
        {
        	
            get { return mainForm.schedulerTimePicker2.Value.ToString("hh:mm:ss"); }
        	
            set { mainForm.schedulerTimePicker2.Value = DateTime.Parse(value); }
        	
        }

        public static bool startDownloadingSession
        {
        	
            set
            { 
        		
                foreach (Control controlItem in mainForm.Controls)
                {
        		
                	if (!controlItem.Name.Contains("res", StringComparison.OrdinalIgnoreCase) && !controlItem.Name.Equals("statusLabel", StringComparison.OrdinalIgnoreCase) && !controlItem.Name.Equals("statusStrip", StringComparison.OrdinalIgnoreCase) && !controlItem.Name.Equals("downloadButton", StringComparison.OrdinalIgnoreCase))
                    {
                		
                        controlItem.Enabled = !value;
                        
                    }
        			
                }
                
                mainForm.downloadButton.Text = !currentlyDownloading ? "Start Downloading" : "Stop Downloading";
                
                MainForm.downloadLabel = null;
        		
                MainForm.downloadFinishedPercent = 0;
        	
            }
        	
        }

        public static Button startDownButton
        {
        	
            get { return mainForm.downloadButton; }
        	
        }

        public static string newUrlText
        {
        	
            get { return mainForm.newURL.Text; }
        	
            set
            { 
        		
                mainForm.newURL.Text = value;
        			
                mainForm.newURL.TextAlign = HorizontalAlignment.Center;
        		
                mainForm.newURL.Font = new Font ("Microsoft Sans Serif", 7.8f, FontStyle.Italic);
        			
            }
        	
        }

        public static int downloadFinishedPercent
        {
        	
            set { mainForm.videoDownloadProgressBar.Value = value; }
        	
        }

        public static string downloadLabel
        {
        	
            set { mainForm.downloadingLabel.Text = value; }
        	
        }

        private void resetNewUrlValues ()
        {
        	
            newUrlText = exampleLink;
        	
            newUrlRes.Value = defaultResolution;
            
            newUrlFormat.SelectedIndex = 0;
        	
        }

        #endregion

        void SchedulingEnabledCheckedChanged (object sender, EventArgs e)
        {
			
            schedulerTimePicker1.Visible = schedulingEnabled.Checked;
			
            schedulerToLabel.Visible = schedulingEnabled.Checked;
			
            schedulerTimePicker2.Visible = schedulingEnabled.Checked;
            
            OptionChanged(sender, e);
			
        }

        public static string RemoveIllegalPathCharacters (string path)
        {
            string regexSearch = new string (Path.GetInvalidFileNameChars()) + new string (Path.GetInvalidPathChars());
            var r = new Regex (string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        void AddURLButtonClick (object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(newURL.Text) && !newURL.Text.Contains(exampleLink) && newUrlRes.Value >= 144 && newUrlRes.Value < 720)
            {
				
            	var newUrlTuple = new Tuple<string, int, VideoType>(this.newURL.Text, (int)this.newUrlRes.Value, getVideoFormat(this.newUrlFormat.Text));
				
				addToQueue(newUrlTuple);
			
                GlobalVariables.urlList.Add(newUrlTuple);
				
                Storage.WriteUrlsToFile();
				
            }
            else if (newUrlRes.Value >= 720)
            {
				
                MessageBox.Show("HD video resolution is currently unsupported", "Resolution Unsupported", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
            }
            else
            {
				
                MessageBox.Show("Some or all values of the new URL are invalid.", "URL Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
            }
			
            resetNewUrlValues();
			
        }

        void NewURLEnter (object sender, EventArgs e)
        {
			
            mainForm.newURL.Clear();
        			
            mainForm.newURL.TextAlign = HorizontalAlignment.Left;
        		
            mainForm.newURL.Font = new Font ("Microsoft Sans Serif", 7.8f, FontStyle.Regular);
			
        }

        void NewURLLeave (object sender, EventArgs e)
        {
			
			
            if (string.IsNullOrWhiteSpace(newURL.Text))
            {
				
                resetNewUrlValues();
				
            }
			
        }

        void Button2Click (object sender, EventArgs e)
        {
			
        	int previouslySelectedIndex = this.queuedBox.SelectedIndex;
			
        	if (this.queuedBox.SelectedIndex > 0 || (this.queuedBox.Items.Count - 1) > 0)
            {
        		
        		DialogResult deletionAnswer = MessageBox.Show(string.Format("Are you sure you want to delete {0}?", GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item1), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        		
        		if (deletionAnswer == DialogResult.Yes)
        		{
        			
	                GlobalVariables.urlList.RemoveAt(this.queuedBox.SelectedIndex);
					
	                this.queuedBox.Items.RemoveAt(this.queuedBox.SelectedIndex);
					
	                Storage.WriteUrlsToFile();
	                
	                refreshQueue(previouslySelectedIndex > (this.queuedBox.Items.Count - 1) ? (this.queuedBox.Items.Count - 1) : previouslySelectedIndex);
	                
        		}
				
            }
            else
            {
				
                MessageBox.Show("Could not delete final URL or no URL selected.", "Cannot Delete URL", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				
            }
			
        }

        void QueuedBoxSelectedIndexChanged (object sender, EventArgs e)
        {
			
            if (this.queuedBox.SelectedIndex > -1)
            {
            	
                this.urlToModify.Text = GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item1;
            	
                this.resolutionToModify.Value = GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item2;
                
                this.formatToModify.SelectedItem = getVideoFormat(GlobalVariables.urlList [this.queuedBox.SelectedIndex].Item3);
          	  
            }
			
        }

        void SubmitModificationButtonClick (object sender, EventArgs e)
        {
			
            int previouslySelected = this.queuedBox.SelectedIndex;
        	
            if (!string.IsNullOrWhiteSpace(urlToModify.Text) && resolutionToModify.Value >= 144 && resolutionToModify.Value < 720)
            {
				
                GlobalVariables.urlList.RemoveAt(this.queuedBox.SelectedIndex);
				
                GlobalVariables.urlList.Insert(this.queuedBox.SelectedIndex, new Tuple<string, int, VideoType> (urlToModify.Text, (int)resolutionToModify.Value, getVideoFormat(this.formatToModify.Text)));
				
                Storage.WriteUrlsToFile();
				
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
            
            refreshQueue(previouslySelected);
			
        }

        private delegate void DownVidsDelegate (int retryCount);

        void StartDownloadButtonClick (object sender, EventArgs e)
        {
            
        	currentlyDownloading = !currentlyDownloading;
        	
            if (MainForm.urlList.Items.Count > 0 && currentlyDownloading)
            {
            	
            	Validation.CheckOrCreateFolder(MainForm.downloadLocation);
    		
    			Validation.CheckOrCreateFolder(MainForm.tempDownloadLocation);
            	
    			var tempDelegate = new DownVidsDelegate (Download.delegate_DownloadVideos);
    			
    			tempDelegate.BeginInvoke(0, null, null);
                
            }
            else if (MainForm.urlList.Items.Count > 0 && !currentlyDownloading)
            {
            	
            	this.downloadButton.Text = "Stopping....";
            	
            	this.downloadButton.Enabled = false;
            	
            }
            else
            {
            	
            	currentlyDownloading = false;
            	
            }
			
        }

        void OptionChanged (object sender, EventArgs e)
        {
			
            Storage.WriteToRegistry();
			
        }
        
		void ChangeSaveLocationClick(object sender, EventArgs e)
		{
			
			DialogResult result = saveLocationDialog.ShowDialog();
			
			if(result == DialogResult.OK)
			{
					
				saveLocation.Text = saveLocationDialog.SelectedPath;
					
			}
			
		}
		
		void ChangeTempSaveLocationClick(object sender, EventArgs e)
		{
			
			DialogResult result = tempLocationDialog.ShowDialog();
			
			if(result == DialogResult.OK)
			{
					
				temporaryLocation.Text = tempLocationDialog.SelectedPath;
					
			}
			
		}
		
    }
    
    
	
}
