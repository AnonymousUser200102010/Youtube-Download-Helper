using System;
using System.Linq;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Code
{
	/// <summary>
	/// Description of Extension.
	/// </summary>
	public static class Extension
	{
		/// <summary>
        /// Reads the url file.
        /// </summary>
        /// <param name="collectionToUse">
        /// The collection to dump the values from the url file into.
        /// </param>
        /// <returns>
        /// Returns a collection of items containing the contents of the url file.
        /// </returns>
        /// <exception cref="T:YoutubeDownloadHelper.InvalidConversionException">
        /// Thrown if the process of reading the file or writing to the collection fails in some way.
        /// </exception>
        public static System.Collections.ObjectModel.ObservableCollection<Video> ReadUrls (this System.Collections.ObjectModel.ObservableCollection<Video> collectionToUse)
        {
            try
            {
            	var urlList = (new System.Collections.ObjectModel.Collection<string>()).AddFileContents(Storage.QueueFile);
            	if (urlList.Any()) collectionToUse.Replace(urlList.ConvertToVideoCollection(0));
            }
            catch (Exception ex)
			{
				if (ex is System.IO.IOException) { throw new InvalidConversionException(ex.Message, ex); }
				throw;
			}
            return collectionToUse;
        }
        
        public static System.Collections.Generic.IEnumerable<Video> Sort (this System.Collections.Generic.IEnumerable<Video> collectionToSort)
        {
        	var readOnlySortCollection = collectionToSort.ToList().AsReadOnly();
        	for (var video = collectionToSort.GetEnumerator(); video.MoveNext();)
			{
        		video.Current.Position = readOnlySortCollection.IndexOf(video.Current);
			}
			return collectionToSort;
        }
        
        /// <summary>
		/// Find a key by it's value in a dictionary.
		/// </summary>
		/// <param name="dictionaryToSearch">
		/// The dictionary whose key you wish to find with the value.
		/// </param>
		/// <param name="value">
		/// The value to use in the search.
		/// </param>
		/// <returns>
		/// Returns the key associated with the value provided, if it exists. If it does not exist, the default will be returned.
		/// </returns>
		public static T AtValue<T>(this System.Collections.Generic.Dictionary<T, string> dictionaryToSearch, string value)
		{
			return dictionaryToSearch.FirstOrDefault(name => name.Value.Contains(value, StringComparison.OrdinalIgnoreCase)).Key;
		}
	}
}
