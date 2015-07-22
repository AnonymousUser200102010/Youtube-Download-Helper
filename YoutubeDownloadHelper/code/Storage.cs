using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace YoutubeDownloadHelper
{

	public class Storage : IStorage
    {
		
        private readonly IMainForm IMainForm;
        
        private readonly IValidation Validation;
        
        public Storage(IMainForm mainForm, IValidation valid)
        {
        	
        	this.IMainForm = mainForm;
        	
        	this.Validation = valid;
        	
        }
        
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

        public void ReadFromRegistry ()
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
			
            IMainForm.DownloadLocation = registryEntries [0];
			
            IMainForm.TempDownloadLocation = registryEntries [1];
			
            IMainForm.Scheduling = bool.Parse(registryEntries [2]);
			
            IMainForm.SchedulingStart = registryEntries [3];
			
            IMainForm.SchedulingEnd = registryEntries [4];
			
        }

        public void WriteToRegistry ()
        {
			
            if (Registry.LocalMachine.OpenSubKey(registryValue) == null)
            {
				
                Registry.LocalMachine.CreateSubKey(registryValue);
				
            }
			
            using (RegistryKey programKey = Registry.LocalMachine.OpenSubKey(registryValue, true))
            {
				
                programKey.SetValue("Download Location", IMainForm.DownloadLocation);
				
                programKey.SetValue("Temporary Download Location", IMainForm.TempDownloadLocation);
				
                programKey.SetValue("Schedual Downloads", IMainForm.Scheduling);
				
                programKey.SetValue("Schedual Time Start", IMainForm.SchedulingStart);
				
                programKey.SetValue("Schedual Time End", IMainForm.SchedulingEnd);
				
            }
			
        }
		
        public void WriteUrlsToFile (Collection<Video> videos, bool backup)
        {
			
            Validation.CheckOrCreateFolder(folderToUse);
			
            using (StreamWriter outfile = new StreamWriter (string.Format(CultureInfo.InstalledUICulture, "{0}{1}", UrlFile, (backup ? ".bak" : null))))
            {
				
                for (int count = 0, numberOfVideos = videos.Count; count < numberOfVideos; count++)
                {
					
                    Video video = videos [count];
					
                    outfile.Write(string.Format(CultureInfo.InstalledUICulture, "{0} {1} {2}\n", video.UrlName, video.Resolution, video.Format));
					
                }
				
            }
			
            File.SetAttributes(UrlFile, FileAttributes.Compressed);
			
        }

        public ObservableCollection<Video> ReadUrls ()
        {
			
            Validation.CheckOrCreateFolder(folderToUse);
			
            ObservableCollection<Video> returnValue = new ObservableCollection<Video> ();
			
            if (File.Exists(UrlFile))
            {
				
                using (StreamReader sr = new StreamReader (UrlFile))
                {
			
                    String line;
					
                    while (!string.IsNullOrEmpty((line = sr.ReadLine())))
                    {
						
                        int position = 0;
					
                        string[] stringBuilder = {
                            null,
                            null,
                            null
                        };
						
                        for (int i = 0; i < line.Length; i++)
                        {
							
                            string character = line.Substring(i, 1);
							
                            if (character.Equals(" ", StringComparison.CurrentCultureIgnoreCase))
                            {
								
                                position++;
								
								
                            }
                            else
                            {
								
                                stringBuilder [position] += character;
								
                            }
							
                            if (i >= line.Length - 1)
                            {
								
                                returnValue.Add(new Video (stringBuilder [0], int.Parse(stringBuilder [1], CultureInfo.InvariantCulture), MainForm.GetVideoFormat(stringBuilder [2])));
								
                            }
							
                        }
						
                    }
						
                    sr.Close();
						
                }
				
                return returnValue;
				
            }
			
            return new ObservableCollection<Video> ();
			
        }
		
    }
	
}


