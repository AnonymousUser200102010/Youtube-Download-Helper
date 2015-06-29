using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace YoutubeDownloadHelper
{
	public static class Storage
	{
		
		private static readonly string tempURLListdat = "Temp\\URLList.dat";
		
		private const string RegistryAppendedValue = "SOFTWARE\\Wow6432Node\\";
		
		public static void ReadFromRegistry()
		{
			
			if (Registry.LocalMachine.OpenSubKey(string.Format("{0}YDH\\", RegistryAppendedValue)) == null)
			{
				
				Registry.LocalMachine.CreateSubKey(string.Format("{0}YDH\\", RegistryAppendedValue));
				
			}
			
			string[] tempStrings = new string[5];
			
			using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(string.Format("{0}YDH\\", RegistryAppendedValue), true))
			{
				
				tempStrings[0] = (string)tempKey.GetValue("Download Location");
				
				tempStrings[1] = (string)tempKey.GetValue("Temporary Download Location");
				
				tempStrings[2] = (string)tempKey.GetValue("Schedual Downloads");
				
				tempStrings[3] = (string)tempKey.GetValue("Schedual Time Start");
				
				tempStrings[4] = (string)tempKey.GetValue("Schedual Time End");
				
			}
			
			MainForm.downloadLocation = tempStrings[0];
			
			MainForm.tempDownloadLocation = tempStrings[1];
			
			MainForm.scheduling = bool.Parse(tempStrings[2]);
			
			MainForm.schedulingStart = tempStrings[3];
			
			MainForm.schedulingEnd = tempStrings[4];
			
		}
		
		public static void WriteToRegistry()
		{
			
			if (Registry.LocalMachine.OpenSubKey(string.Format("{0}YDH\\", RegistryAppendedValue)) == null)
			{
				
				Registry.LocalMachine.CreateSubKey(string.Format("{0}YDH\\", RegistryAppendedValue));
				
			}
			
			using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(string.Format("{0}YDH\\", RegistryAppendedValue), true))
			{
				
				tempKey.SetValue("Download Location", MainForm.downloadLocation);
				
				tempKey.SetValue("Temporary Download Location", MainForm.tempDownloadLocation);
				
				tempKey.SetValue("Schedual Downloads", MainForm.scheduling);
				
				tempKey.SetValue("Schedual Time Start", MainForm.schedulingStart);
				
				tempKey.SetValue("Schedual Time End", MainForm.schedulingEnd);
				
			}
			
		}
		
		public static void WriteUrlsToBackupFile ()
		{
			
			Validation.CheckOrCreateFolder("Temp\\");
			
			using (StreamWriter outfile = new StreamWriter(string.Format("{0}.bak", tempURLListdat)))
			{
				
				for (int count = 0, GlobalVariablesfinishedUrlListCount = GlobalVariables.finishedUrlList.Count; count < GlobalVariablesfinishedUrlListCount; count++)
				{
					
					KeyValuePair<string, int> url = GlobalVariables.finishedUrlList[count];
					
					outfile.Write(string.Format("{0} {1}\n", url.Key, url.Value));
					
				}
				
			}
			
			File.SetAttributes(tempURLListdat, FileAttributes.Compressed);
			
		}
		
		public static void WriteUrlsToFile ()
		{
			
			Validation.CheckOrCreateFolder("Temp\\");
			
			using (StreamWriter outfile = new StreamWriter (tempURLListdat))
			{
				
				for (int count = 0, GlobalVariablesurlListCount = GlobalVariables.accessUrlList.Count; count < GlobalVariablesurlListCount; count++)
				{
					
					KeyValuePair<string, int> url = GlobalVariables.accessUrlList[count];
					
					outfile.Write(string.Format("{0} {1}\n", url.Key, url.Value));
					
				}
				
			}
			
			File.SetAttributes(tempURLListdat, FileAttributes.Compressed);
			
		}
		
		public static void readUrlList()
		{
			
			Validation.CheckOrCreateFolder("Temp\\");
			
			if(File.Exists(tempURLListdat))
			{
				
				using (StreamReader sr = new StreamReader(tempURLListdat))
				{
			
					String line;
					
					while (!string.IsNullOrEmpty((line = sr.ReadLine())))
					{
						
						int position = 0;
					
						string[] stringBuilder = {
							null,
							null
						};
						
						for(int i = 0; i < line.Length; i++)
						{
							
							string character = line.Substring(i, 1);
							
							if (character.Equals(" ", StringComparison.CurrentCultureIgnoreCase))
							{
								
								position++;
								
								
							}
							else
							{
								
								stringBuilder[position] += character;
								
							}
							
							if (i >= line.Length - 1)
							{
								
								GlobalVariables.urlList.Add(new KeyValuePair<string, int>(stringBuilder[0], int.Parse(stringBuilder[1])));
								
							}
							
						}
						
					}
						
					sr.Close();
						
				}
				
			}
			
		}
		
	}
	
}


