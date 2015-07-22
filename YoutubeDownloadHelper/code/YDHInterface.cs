using System;

namespace YoutubeDownloadHelper
{

    public interface IMainForm
    {
    		
        /// <summary>
        /// The listBox containing the queued url list.
        /// </summary>
        int UrlsNumberItems { get; }

        /// <summary>
        /// The "flag" that determines whether an asynchronous downloading operation is currently taking place.
        /// </summary>
        bool CurrentlyDownloading { get; set; }

        /// <summary>
        /// Refresh the queue.
        /// </summary>
        /// <param name="previouslySelectedIndex">
        /// The queue index selected before this operation took place.
        /// </param>
        /// <param name="forceFocusOnQueue">
        /// Forces the url queue control object to take focus from wherever the user is/was currently focusing.
        /// </param>
        void RefreshQueue (int previouslySelectedIndex, bool forceFocusOnQueue);

        /// <summary>
        /// Adds the selected parameter to the url queue.
        /// </summary>
        /// <param name="queueItem">
        /// The parameter to add to the queue.
        /// </param>
        void AddToQueue (Video queueItem);

        /// <summary>
        /// Transforms the selected format parameter into it's logical equivalent.
        /// </summary>
        /// <param name="value">
        /// The parameter to convert.
        /// </param>
        /// <returns>
        /// The VideoType most closely related to the string provided.
        /// </returns>
        //YoutubeExtractor.VideoType GetVideoFormat (string value);

        /// <summary>
        /// The selected index in the url queue.
        /// </summary>
        int SelectedQueueIndex { get; set; }

        /// <summary>
        /// The status bar text for the program.
        /// </summary>
        string StatusBar { set; }

        /// <summary>
        /// The location that the completely downloaded video will be moved to. This is mainly used for downloading operations only.
        /// </summary>
        string DownloadLocation { get; set; }

        /// <summary>
        /// The location that the video will be download to until it is finished. This is mainly used for downloading operations only.
        /// </summary>
        string TempDownloadLocation { get; set; }

        /// <summary>
        /// The "flag" that determines whether scheduling is currently available.
        /// </summary>
        bool Scheduling { get; set; }

        /// <summary>
        /// The start time for scheduling operations.
        /// </summary>
        string SchedulingStart { get; set; }

        /// <summary>
        /// The end time for scheduling operations.
        /// </summary>
        string SchedulingEnd { get; set; }

        /// <summary>
        /// Performs a set of operations to either initiate or end a downloading session.
        /// </summary>
        /// <param name="initiate">
        /// Is this operation initiating a download session.
        /// </param>
        void StartDownloadingSession (bool initiate);

        /// <summary>
        /// The main button used for starting (and stopping) a downloading session.
        /// </summary>
        bool StartDownButtonEnabled { get; set; }

        /// <summary>
        /// The download progress for the current downloading session (if applicable).
        /// </summary>
        int DownloadFinishedPercent { set; }

        /// <summary>
        /// The text used to inform the user of the frontend progress of a downloading session (if applicable).
        /// </summary>
        string DownloadLabel { set; }
		
    }
	
    
    public interface IValidation
    {
    	
    	/// <summary>
    	/// Checks if the provided folder exists. If it doesn't, it creates it.
    	/// </summary>
    	/// <param name="folderName">
    	/// The folder path, including the name of the folder.
    	/// </param>
    	void CheckOrCreateFolder (string folderName);
    	
    }
    
    public interface IDownload
    {
    	
    	/// <summary>
    	/// Initializes the batch downloading process.
    	/// </summary>
    	/// <param name="retryCount">
    	/// Internal retry count in case of errors. When used outside the function, the value should always be 0.
    	/// </param>
    	/// <param name="Urls">
    	/// The queued url list.
    	/// </param>
    	void HandleDownloadingProcesses(int retryCount, System.Collections.ObjectModel.ObservableCollection<Video> Urls);
    	
    }
    
    public interface IStorage
    {
    	
    	/// <summary>
    	/// Reads persistent values from the registry.
    	/// </summary>
    	void ReadFromRegistry ();
    	
    	/// <summary>
    	/// Writes persistent values to the registry.
    	/// </summary>
    	void WriteToRegistry ();
    	
    	/// <summary>
    	/// Writes the provided url list to a file.
    	/// </summary>
    	/// <param name="Urls">
    	/// The url list to write. It does not have to be exclusive to the queue.
    	/// </param>
    	/// <param name="backup">
    	/// Is this a backup list? (adds .bak to the filename)
    	/// </param>
    	void WriteUrlsToFile (System.Collections.ObjectModel.Collection<Video> Urls, bool backup);
    	
    	/// <summary>
    	/// Reads the file provided and converts it into a url list. (Currently there is only queue support)
    	/// </summary>
    	/// <returns>
    	/// Returns the parsed url list of the file that was read.
    	/// </returns>
    	System.Collections.ObjectModel.ObservableCollection<Video> ReadUrls ();
    	
    }
    
}


