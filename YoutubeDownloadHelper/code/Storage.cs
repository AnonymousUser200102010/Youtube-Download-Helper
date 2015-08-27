using System;
using System.Globalization;
using System.Linq;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Code
{
	public class Storage: IStorage
    {
        /// <summary>
        /// The root folder used for this program.
        /// </summary>
        public const string Folder = "Files\\";

        /// <summary>
        /// The root file used for this program.
        /// </summary>
        public static string File
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}URLList.dat", Folder);
            }
        }

        private const string registryRootValue = "Wow6432Node\\YDH";
        
        public Settings RegistryRead (Settings settings)
        {
        	return settings.Replace(settings.AsEnumerable().ReadFromRegistry(registryRootValue, App.IsDebugging));
        }

        public void RegistryWrite (Settings settings)
        {
        	settings.AsEnumerable().WriteToRegistry(registryRootValue, App.IsDebugging);
        }
    }
}


