using System;
using System.Collections.ObjectModel;
using System.IO;
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
        public static ObservableCollection<Video> ReadUrls (this ObservableCollection<Video> collectionToUse)
        {
            try
            {
            	var urlList = (new Collection<string>()).AddFileContents(Storage.File);
            	if (urlList.Any()) collectionToUse.Replace(urlList.ConvertToVideoCollection(0));
            }
            catch (Exception ex)
			{
				if (ex is IOException) { throw new InvalidConversionException(ex.Message, ex); }
				throw;
			}
            return collectionToUse;
        }
	}
}
