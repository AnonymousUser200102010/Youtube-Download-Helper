using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using YoutubeExtractor;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Code
{
    /// <summary>
    /// Description of Conversion.
    /// </summary>
    public static class Conversion
    {
    	/// <summary>
        /// Convert a url list.
        /// </summary>
        /// <description>
        /// Converts an array of urls into a list of videos and returns said list for use.
        /// </description>
        /// <param name="value">
        /// The string representation of a list of urls.
        /// </param>
        /// <param name="initialPosition">
        /// The initial position in the list to be used when creating the list.
        /// </param>
        /// <returns>
        /// Returns a parsed list of videos.
        /// </returns>
        /// <exception cref="T:YoutubeDownloadHelper.ParsingException">
        /// Thrown when a url in the value array could not be parsed.
        /// </exception>
        public static ObservableCollection<Video> ConvertToVideoCollection (this IEnumerable<string> value, int initialPosition)
        {
        	return value.GetEnumerator().ConvertToVideoCollection(initialPosition);
        }
    	
        /// <summary>
        /// Convert a url list.
        /// </summary>
        /// <description>
        /// Converts an array of urls into a list of videos and returns said list for use.
        /// </description>
        /// <param name="value">
        /// The string representation of a list of urls.
        /// </param>
        /// <param name="initialPosition">
        /// The initial position in the list to be used when creating the list.
        /// </param>
        /// <returns>
        /// Returns a parsed list of videos.
        /// </returns>
        /// <exception cref="T:YoutubeDownloadHelper.ParsingException">
        /// Thrown when a url in the value array could not be parsed.
        /// </exception>
        public static ObservableCollection<Video> ConvertToVideoCollection (this IEnumerator<string> value, int initialPosition)
        {
            var queue = new ObservableCollection<Video> ();
            for (var position = value; position.MoveNext();)
            {
            	var stringPosition = position.Current;
                try
                {
                    var vagueVideoInfo = stringPosition.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    
                    var format = bool.Parse(vagueVideoInfo[3]) ? VideoType.Mp4 : (VideoType)Enum.Parse(typeof(VideoType), Enum.GetNames(typeof(VideoType)).First(name => name.Contains(vagueVideoInfo[2], StringComparison.OrdinalIgnoreCase)));
                    var aFormat = bool.Parse(vagueVideoInfo[3]) ? (AudioType)Enum.Parse(typeof(AudioType), Enum.GetNames(typeof(AudioType)).First(name => name.Contains(vagueVideoInfo[2], StringComparison.OrdinalIgnoreCase))) : AudioType.Mp3;
                    
                    queue.Add(new Video (initialPosition + queue.Count(), vagueVideoInfo[0], int.Parse(vagueVideoInfo[1], CultureInfo.InvariantCulture), format, aFormat, bool.Parse(vagueVideoInfo[3])));
                }
                catch (Exception ex)
                {
                    throw new ParsingException (string.Format(CultureInfo.CurrentCulture, "'{0}' could not be converted to a usable format ({1})", stringPosition, ex.Message));
                }
            }
            return queue;
        }
    }
}
