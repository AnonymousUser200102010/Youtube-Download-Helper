using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using YoutubeExtractor;

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

        private readonly string tempURLListdat = "Temp\\URLList.dat";

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
			
            string[] tempStrings = new string[5];
			
            using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(registryValue, true))
            {
				
                tempStrings [0] = string.IsNullOrEmpty((string)tempKey.GetValue("Download Location")) ? "C:\\" : (string)tempKey.GetValue("Download Location");
				
                tempStrings [1] = string.IsNullOrEmpty((string)tempKey.GetValue("Temporary Download Location")) ? "C:\\" : (string)tempKey.GetValue("Temporary Download Location");
				
                tempStrings [2] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Downloads")) ? false.ToString() : (string)tempKey.GetValue("Schedual Downloads");
				
                tempStrings [3] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Time Start")) ? DateTime.Now.ToString() : (string)tempKey.GetValue("Schedual Time Start");
				
                tempStrings [4] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Time End")) ? DateTime.Now.AddMinutes(1).ToString() : (string)tempKey.GetValue("Schedual Time End");
				
            }
			
            IMainForm.DownloadLocation = tempStrings [0];
			
            IMainForm.TempDownloadLocation = tempStrings [1];
			
            IMainForm.Scheduling = bool.Parse(tempStrings [2]);
			
            IMainForm.SchedulingStart = tempStrings [3];
			
            IMainForm.SchedulingEnd = tempStrings [4];
			
        }

        public void WriteToRegistry ()
        {
			
            if (Registry.LocalMachine.OpenSubKey(registryValue) == null)
            {
				
                Registry.LocalMachine.CreateSubKey(registryValue);
				
            }
			
            using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(registryValue, true))
            {
				
                tempKey.SetValue("Download Location", IMainForm.DownloadLocation);
				
                tempKey.SetValue("Temporary Download Location", IMainForm.TempDownloadLocation);
				
                tempKey.SetValue("Schedual Downloads", IMainForm.Scheduling);
				
                tempKey.SetValue("Schedual Time Start", IMainForm.SchedulingStart);
				
                tempKey.SetValue("Schedual Time End", IMainForm.SchedulingEnd);
				
            }
			
        }
		
        public void WriteUrlsToFile (Collection<Tuple<string, int, VideoType>> urlList, bool backup)
        {
			
            Validation.CheckOrCreateFolder("Files\\");
			
            using (StreamWriter outfile = new StreamWriter (string.Format(CultureInfo.InstalledUICulture, "{0}{1}", tempURLListdat, (backup ? ".bak" : null))))
            {
				
                for (int count = 0, GlobalVariablesurlListCount = urlList.Count; count < GlobalVariablesurlListCount; count++)
                {
					
                    Tuple<string, int, VideoType> url = urlList [count];
					
                    outfile.Write(string.Format(CultureInfo.InstalledUICulture, "{0} {1} {2}\n", url.Item1, url.Item2, url.Item3));
					
                }
				
            }
			
            File.SetAttributes(tempURLListdat, FileAttributes.Compressed);
			
        }

        public Collection<Tuple<string, int, VideoType>> ReadUrlList ()
        {
			
            Validation.CheckOrCreateFolder("Files\\");
			
            Collection<Tuple<string, int, VideoType>> returnValue = new Collection<Tuple<string, int, VideoType>> ();
			
            if (File.Exists(tempURLListdat))
            {
				
                using (StreamReader sr = new StreamReader (tempURLListdat))
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
								
                                returnValue.Add(new Tuple<string, int, VideoType> (stringBuilder [0], int.Parse(stringBuilder [1], CultureInfo.InvariantCulture), MainForm.GetVideoFormat(stringBuilder [2])));
								
                            }
							
                        }
						
                    }
						
                    sr.Close();
						
                }
				
                return returnValue;
				
            }
			
            return new Collection<Tuple<string, int, VideoType>> ();
			
        }
		
    }
	
}


