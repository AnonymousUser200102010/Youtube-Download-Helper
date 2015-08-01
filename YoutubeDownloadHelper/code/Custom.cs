using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using YoutubeExtractor;

namespace YoutubeDownloadHelper.Code
{
	public class VideosToDownload
	{
		private ObservableCollection<Video> _items = new ObservableCollection<Video>();
		
		/// <summary>
		/// The items within this collection.
		/// </summary>
		public ObservableCollection<Video> Items 
		{ 
			get {return _items;}
			set 
			{
				_items = value;
				new ClassContainer().IOHandlingCode.WriteUrlsToFile(value, false);
			}
		}
		
		/// <summary>
		/// This is the video queue.
		/// </summary>
		public VideosToDownload()
		{
			this._items = new ClassContainer().IOHandlingCode.ReadUrls();
		}
	}
	
    public class Video : INotifyPropertyChanged
    {
    	private int _position;
    	
    	/// <summary>
    	/// The position of the Video within whatever list it's in.
    	/// </summary>
    	public int Position 
    	{
    		get{return _position+1;}
    		set{
    			_position = value;
    			RaisePropertyChanged("Position");
    		}
    	}
    	
    	/// <summary>
    	/// The string value of the url of this video.
    	/// </summary>
    	public string Location { get; private set; }
    	
    	/// <summary>
    	/// The resolution of this video.
    	/// </summary>
    	public int Resolution { get; private set; }
    	
    	/// <summary>
    	/// The format (or extension) of the video.
    	/// </summary>
    	public VideoType Format { get; private set; }
    	
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} {1} {2}\n", Location, Resolution, Format);
		}
		
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
    	
    	/// <summary>
    	/// Represents a video through a set of locally held attributes.
    	/// </summary>
    	/// <param name="pos">
    	/// The position of the Video within the current list.
    	/// </param>
    	/// <param name="location">
    	/// The "location" of the video on the internet. In other words, the string representation of the url for the video.
    	/// </param>
    	/// <param name="res">
    	/// The resolution of the video.
    	/// </param>
    	/// <param name="format">
    	/// The format (or extension) of the video.
    	/// </param>
    	public Video(int pos, string location, int res, VideoType format)
    	{
    		this.Position = pos;
    		this.Location = location;
    		this.Resolution = res;
    		this.Format = format;
    	}
    	
    	
    	public Video()
    	{
    		this.Position = -1;
    		this.Location = "Invalid Url!";
    		this.Resolution = 144;
    		this.Format = VideoType.Unknown;
    	}
    	
    }
    
    public class Settings : INotifyPropertyChanged
	{
    	private bool schedulingEnabled = false;
    	private string[] saveLocations = new string[2];
    	private Collection<string> schedulingTimes = new Collection<string>();
    	
    	/// <summary>
    	/// Is the program currently using the scheduling function?
    	/// </summary>
    	public bool Scheduling 
    	{ 
    		get
    		{
    			return schedulingEnabled;
    		} 
    		set
    		{
    			schedulingEnabled = value;
    			RaisePropertyChanged("Scheduling");
    		} 
    	}
		
		/// <summary>
		/// The save directory where finished files will be moved to.
		/// </summary>
		public string MainSaveLocation
    	{ 
    		get
    		{
    			return saveLocations[0];
    		} 
    		set
    		{
    			saveLocations[0] = value;
    			RaisePropertyChanged("MainSaveLocation");
    		} 
    	}
		
		/// <summary>
		/// The temporary save directory where files will be downloaded to.
		/// </summary>
		public string TemporarySaveLocation
    	{ 
    		get
    		{
    			return saveLocations[1];
    		} 
    		set
    		{
    			saveLocations[1] = value;
    			RaisePropertyChanged("TemporarySaveLocation");
    		} 
    	}
		
		/// <summary>
		/// An array containing the start of scheduling and the end of scheduling.
		/// </summary>
		public Collection<string> Schedule
    	{ 
    		get
    		{
    			return schedulingTimes;
    		} 
    		set
    		{
    			schedulingTimes = value;
    		} 
    	}
		
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
		
		/// <summary>
		/// The container for user-defined program settings.
		/// </summary>
		/// <param name="scheduling">
		/// The user defined value for whether scheduling is enabled.
		/// </param>
		/// <param name="mainSaveLoc">
		/// The user defined value for the primary save location.
		/// </param>
		/// <param name="tempSaveLoc">
		/// The user defined value for the secondary save location.
		/// </param>
		/// <param name="schedule">
		/// The user defined start/end time of scheduling.
		/// </param>
		public Settings(bool scheduling, string mainSaveLoc, string tempSaveLoc, Collection<string> schedule)
		{
			
			this.Scheduling = scheduling;
			this.MainSaveLocation = mainSaveLoc;
			this.TemporarySaveLocation = tempSaveLoc;
			this.Schedule = schedule;
			
		}
	}
    
    public class ClassContainer
    {
    	
    	/// <summary>
    	/// Conversion class.
    	/// </summary>
    	internal IConversion ConversionCode { get; private set; }
    	
    	/// <summary>
    	/// Storage class.
    	/// </summary>
    	internal IStorage IOHandlingCode { get; private set; }
    	
    	/// <summary>
    	/// Validation class.
    	/// </summary>
    	internal IValidation ValidationCode { get; private set; }
    	
    	/// <summary>
    	/// Download class.
    	/// </summary>
    	internal IDownload DownloadingCode { get; private set; }
    	
    	
	    /// <summary>
	    /// Backend container of classes to make the programmer's life a little easier.
	    /// </summary>
    	public ClassContainer()
    	{
    		
    		this.ConversionCode = new Conversion();
    		this.ValidationCode = new Validation();
    		this.IOHandlingCode = new Storage(this.ConversionCode, this.ValidationCode);
    		this.DownloadingCode = new Download(this.IOHandlingCode, this.ConversionCode);
    		
    	}
    	
    }
    
}
