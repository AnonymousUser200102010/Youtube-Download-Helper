using System;
using System.Collections.ObjectModel;
using System.Globalization;
using YoutubeExtractor;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Code
{
	/// <summary>
	/// Description of Conversion.
	/// </summary>
	public class Conversion : IConversion
	{
		public VideoType GetVideoFormat (string value)
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
        	
            throw new InvalidConversionException ("getVideoFormat: String format not recognized.");
        }
		
		public string Truncate (string value, int truncationCutoff)
        {
    		
            return value.Length <= truncationCutoff ? value : string.Format(CultureInfo.InstalledUICulture, "{0}[...]", value.Substring(0, truncationCutoff)).ToLower(CultureInfo.InstalledUICulture);
    		
        }
		
		public ObservableCollection<Video> ConvertUrl (int initialPosition, string[] value)
		{
			
			ObservableCollection<Video> queue = new ObservableCollection<Video>();
			for (int stringPosition = 0, valueLength = value.Length; stringPosition < valueLength; stringPosition++)
			{
				try
				{
					string[] vagueVideoInfo = value[stringPosition].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
					queue.Add(new Video(initialPosition + queue.Count, vagueVideoInfo[0], int.Parse(vagueVideoInfo[1], CultureInfo.InvariantCulture), this.GetVideoFormat(vagueVideoInfo[2])));
				}
				catch (Exception ex)
				{
					throw new UnparsableException(string.Format(CultureInfo.CurrentCulture, "'{0}' could not be converted to a usable format ({1})", value[stringPosition], ex.Message));
				}
			}
			return queue;
		}
	}
}
