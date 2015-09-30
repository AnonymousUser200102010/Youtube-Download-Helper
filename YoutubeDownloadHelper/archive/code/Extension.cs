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
        	for (var position = collectionToSort.GetEnumerator(); position.MoveNext();)
			{
        		position.Current.Position = readOnlySortCollection.IndexOf(position.Current);
			}
			return collectionToSort;
        }
	}
}
