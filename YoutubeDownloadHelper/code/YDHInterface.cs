using System;

namespace YoutubeDownloadHelper.Code
{
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
    	void DownloadHandler (YoutubeDownloadHelper.Gui.MainWindow mainWindow, int selectedIndex);
    	
    	/// <summary>
    	/// Removes all finished urls from a url list and returns the result.
    	/// </summary>
    	/// <param name="finishedUrls">
    	/// The list of finished urls.
    	/// </param>
    	/// <returns>
    	/// Returns a list with only unfinished urls.
    	/// </returns>
    	System.Collections.ObjectModel.ObservableCollection<Video> GetUnfinishedDownloads (System.Collections.ObjectModel.ObservableCollection<Video> finishedUrls);
    }
    
    public interface IStorage
    {
    	
    	/// <summary>
    	/// Reads persistent values from the registry.
    	/// </summary>
    	Settings ReadFromRegistry ();
    	
    	/// <summary>
    	/// Writes persistent values to the registry.
    	/// </summary>
    	void WriteToRegistry (Settings settings);
    	
    	/// <summary>
    	/// Writes the provided url list to a file.
    	/// </summary>
    	/// <param name="urls">
    	/// The url list to write. It does not have to be exclusive to the queue.
    	/// </param>
    	/// <param name="backup">
    	/// Is this a backup list? (adds .bak to the filename)
    	/// </param>
    	void WriteUrlsToFile (System.Collections.ObjectModel.Collection<Video> urls, bool backup);
    	
    	/// <summary>
    	/// Reads the file provided and converts it into a url list. (Currently there is only queue support)
    	/// </summary>
    	/// <returns>
    	/// Returns the parsed url list of the file that was read.
    	/// </returns>
    	System.Collections.ObjectModel.ObservableCollection<Video> ReadUrls ();
    	
    }
    
    public interface IConversion
    {
    	
    	/// <summary>
        /// Transforms the selected format parameter into it's logical equivalent.
        /// </summary>
        /// <param name="value">
        /// The parameter to convert.
        /// </param>
        /// <returns>
        /// The VideoType most closely related to the string provided.
        /// </returns>
        /// <exception cref="T:YoutubeDownloadHelper.InvalidConversionException">
        /// Thrown when the value is not a recognized VideoType.
        /// </exception>
    	YoutubeExtractor.VideoType GetVideoFormat (string value);
    	
    	/// <summary>
    	/// Checks if a string is within the given parameters.
    	/// </summary>
    	/// <param name="value">
    	/// The object whose length you want to check.
    	/// </param>
    	/// <param name="truncationCutoff">
    	/// The length before the string is truncated.
    	/// </param>
    	/// <returns>
    	/// Returns the full string if it is less than or equal to the provided truncation cutoff. If not, returns the same string, with all characters after the truncation cutoff length into a singular '[...]'.
    	/// </returns>
    	string Truncate (string value, int truncationCutoff);
    	
    	/// <summary>
    	/// Convert a url list.
    	/// </summary>
    	/// <param name="value">
    	/// The string representation of a list.
    	/// </param>
    	/// <param name="initialPosition">
    	/// The position to be added to the count position when adding items to the parsed url list.
    	/// </param>
    	/// <returns>
    	/// Returns a parsed url list.
    	/// </returns>
    	/// <exception cref="T:YoutubeDownloadHelper.UnparsableException">
    	/// Thrown when a Url in the value array could not be parsed.
    	/// </exception>
    	System.Collections.ObjectModel.ObservableCollection<Video> ConvertUrl (int initialPosition, string[] value);
    	
    }
    
}