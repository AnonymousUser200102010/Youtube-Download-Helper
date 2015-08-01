using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace YoutubeDownloadHelper.Code
{

	public class Storage : IStorage
    {
        private readonly IConversion IConvert;
        private readonly IValidation Validation;
        private const string folderToUse = "Files\\";
        private readonly string UrlFile = string.Format(CultureInfo.InvariantCulture, "{0}URLList.dat", folderToUse);

        private static string registryValue
        {
			
            get
            { 
                string returnValue = "SOFTWARE\\Wow6432Node\\YDH\\";
				
                #if DEBUG
                returnValue += "Debug\\";
                #else
				returnValue += Environment.UserName;
                #endif
				
                return returnValue;
            }
			
        }
        
        public Storage(IConversion convert, IValidation valid)
        {
        	this.IConvert = convert;
        	this.Validation = valid;
        }
        
        public Settings ReadFromRegistry ()
        {
            if (Registry.LocalMachine.OpenSubKey(registryValue) == null)
            {
                Registry.LocalMachine.CreateSubKey(registryValue);	
            }
            string[] registryEntries = new string[5];
			
            using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(registryValue, true))
            {
                registryEntries [0] = string.IsNullOrEmpty((string)tempKey.GetValue("Download Location")) ? "C:\\" : (string)tempKey.GetValue("Download Location");

                registryEntries [1] = string.IsNullOrEmpty((string)tempKey.GetValue("Temporary Download Location")) ? "C:\\" : (string)tempKey.GetValue("Temporary Download Location");
				
                registryEntries [2] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Downloads")) ? false.ToString() : (string)tempKey.GetValue("Schedual Downloads");
				
                registryEntries [3] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Time Start")) ? DateTime.Now.ToString() : (string)tempKey.GetValue("Schedual Time Start");
				
                registryEntries [4] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Time End")) ? DateTime.Now.AddMinutes(1).ToString() : (string)tempKey.GetValue("Schedual Time End");
            }
            
            Collection<string> times = new Collection<string>();
            times.Add(registryEntries[3]);
            times.Add(registryEntries[4]);
            return new Settings(bool.Parse(registryEntries[2]), registryEntries[0], registryEntries[1], times);
        }

        public void WriteToRegistry (Settings settings)
        {
			
            if (Registry.LocalMachine.OpenSubKey(registryValue) == null)
            {
                Registry.LocalMachine.CreateSubKey(registryValue);
            }
			
            using (RegistryKey programKey = Registry.LocalMachine.OpenSubKey(registryValue, true))
            {
                programKey.SetValue("Download Location", settings.MainSaveLocation);
                programKey.SetValue("Temporary Download Location", settings.TemporarySaveLocation);
                programKey.SetValue("Schedual Downloads", settings.Scheduling);
                programKey.SetValue("Schedual Time Start", settings.Schedule[0]);
                programKey.SetValue("Schedual Time End", settings.Schedule[1]);
            }
        }
		
        public void WriteUrlsToFile (Collection<Video> urls, bool backup)
        {
            Validation.CheckOrCreateFolder(folderToUse);
			
            using (StreamWriter outfile = new StreamWriter (string.Format(CultureInfo.InstalledUICulture, "{0}{1}", UrlFile, (backup ? ".bak" : null))))
            {
                for (int count = 0, numberOfVideos = urls.Count; count < numberOfVideos; count++)
                {
                    outfile.Write(urls[count].ToString());
                }
            }
            File.SetAttributes(UrlFile, FileAttributes.Compressed);
        }

        public ObservableCollection<Video> ReadUrls ()
        {
            Validation.CheckOrCreateFolder(folderToUse);
            List<string> linesToConvert = new List<string>();
            
            if (File.Exists(UrlFile))
            {
                using (StreamReader sr = new StreamReader (UrlFile))
                {
                    String line;
                    while (!string.IsNullOrEmpty((line = sr.ReadLine())))
                    {
                    	linesToConvert.Add(line);
	            	}
                    sr.Close();	
                }
                return this.IConvert.ConvertUrl(0, linesToConvert.ToArray());
            }
            return new ObservableCollection<Video> ();
        }
    }
}


