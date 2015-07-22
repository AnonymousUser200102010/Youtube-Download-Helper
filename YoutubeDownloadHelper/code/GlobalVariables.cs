using System;
using System.Collections.ObjectModel;
using System.Linq;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{
    
    struct VideoQueue
    {
    	
    	public static ObservableCollection<Video> Items { get; set; }
    	
    }
    
    public class Video
    {
    	
    	internal string UrlName { get; private set; }
    	internal int Resolution { get; private set; }
    	internal VideoType Format { get; private set; }
    	
    	/// <summary>
    	/// Represents a video through a set of locally held attributes.
    	/// </summary>
    	/// <param name="name">
    	/// The "name" of the video represented as a url string. NOT THE ACTUAL NAME!
    	/// </param>
    	/// <param name="res">
    	/// The resolution of the video.
    	/// </param>
    	/// <param name="format">
    	/// The format (or extension) of the video.
    	/// </param>
    	public Video(string name, int res, VideoType format)
    	{
    		
    		this.UrlName = name;
    		this.Resolution = res;
    		this.Format = format;
    		
    	}
    	
    }
    
}


