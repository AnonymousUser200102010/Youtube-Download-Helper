using System;
using System.Collections.ObjectModel;
using System.Linq;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{
    
    struct VideoQueue
    {
    	
    	/// <summary>
    	/// A collection of videos within the queue.
    	/// </summary>
    	public static ObservableCollection<Video> Items { get; set; }
    	
    }
    
    public class Video
    {
    	
    	/// <summary>
    	/// The string value of the url of this video.
    	/// </summary>
    	internal string Location { get; private set; }
    	
    	/// <summary>
    	/// The resolution of this video.
    	/// </summary>
    	internal int Resolution { get; private set; }
    	
    	/// <summary>
    	/// The format (or extension) of the video.
    	/// </summary>
    	internal VideoType Format { get; private set; }
    	
    	/// <summary>
    	/// Represents a video through a set of locally held attributes.
    	/// </summary>
    	/// <param name="location">
    	/// The "location" of the video on the internet. In other words, the string representation of the url for the video.
    	/// </param>
    	/// <param name="res">
    	/// The resolution of the video.
    	/// </param>
    	/// <param name="format">
    	/// The format (or extension) of the video.
    	/// </param>
    	public Video(string location, int res, VideoType format)
    	{
    		
    		this.Location = location;
    		this.Resolution = res;
    		this.Format = format;
    		
    	}
    	
    }
    
}


