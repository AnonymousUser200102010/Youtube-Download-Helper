using System;
using System.Collections.Generic;
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
        /// The url file used for the queue.
        /// </summary>
        public static string QueueFile
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}URLList.dat", Folder);
            }
        }
        
        /// <summary>
        /// The registry-like settings file used for this program.
        /// </summary>
        public static string RegistryFile
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}Settings.dat", Folder);
            }
        }
        
        private const string registryRootBacking = "Wow6432Node\\YDH";
        /// <summary>
        /// The registry root key to be used with the Windows Registry.
        /// </summary>
        public static string RegistryRoot
        {
        	get
        	{
        		return registryRootBacking;
        	}
        }
        
        public Settings RegistryRead (Settings settings)
        {
			var settingsInformation = new List<RegistryEntry>();
			if (App.IsWindowsMachine) settingsInformation = (new Settings()).AsEnumerable(SettingsReturnType.Essential).ReadFromRegistry(RegistryRoot, App.IsDebugging).ToList();
			else
			{
				var settingsFromFile = new System.Collections.ObjectModel.Collection<string>().AddFileContents(RegistryFile);
				if(settingsFromFile.Any())
				{
					for (var position = settingsFromFile.GetEnumerator(); position.MoveNext();)
					{
						var entry = position.Current.Split(new[] { RegistryEntry.Separator }, StringSplitOptions.RemoveEmptyEntries);
						settingsInformation.Add(new RegistryEntry(entry.First(), entry.Last()));
					}
				}
			}
			return settings.Replace(settingsInformation);
        }

        public void RegistryWrite (IEnumerable<RegistryEntry> settings)
        {
			if (App.IsWindowsMachine) settings.WriteToRegistry(RegistryRoot, App.IsDebugging);
			else
			{
				var linesToWrite = new List<string>();
				for (var position = settings.GetEnumerator(); position.MoveNext();)
				{
					linesToWrite.Add(string.Format(CultureInfo.CurrentCulture, "{0}\n", position.Current.ToString()));
				}
				linesToWrite.WriteToFile(RegistryFile);
			}
        }
    }
}


