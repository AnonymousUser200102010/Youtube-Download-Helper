using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{
	public static class Storage
	{
		
		private static readonly string tempURLListdat = "Temp\\URLList.dat";
		
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
		
		public static void ReadFromRegistry()
		{
			
			if (Registry.LocalMachine.OpenSubKey(registryValue) == null)
			{
				
				Registry.LocalMachine.CreateSubKey(registryValue);
				
			}
			
			string[] tempStrings = new string[5];
			
			using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(registryValue, true))
			{
				
				tempStrings[0] = string.IsNullOrEmpty((string)tempKey.GetValue("Download Location")) ? "C:\\" : (string)tempKey.GetValue("Download Location");
				
				tempStrings[1] = string.IsNullOrEmpty((string)tempKey.GetValue("Temporary Download Location")) ? "C:\\" : (string)tempKey.GetValue("Temporary Download Location");
				
				tempStrings[2] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Downloads")) ? false.ToString() : (string)tempKey.GetValue("Schedual Downloads");
				
				tempStrings[3] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Time Start")) ? DateTime.Now.ToString() : (string)tempKey.GetValue("Schedual Time Start");
				
				tempStrings[4] = string.IsNullOrEmpty((string)tempKey.GetValue("Schedual Time End")) ? DateTime.Now.AddMinutes(1).ToString() : (string)tempKey.GetValue("Schedual Time End");
				
			}
			
			MainForm.downloadLocation = tempStrings[0];
			
			MainForm.tempDownloadLocation = tempStrings[1];
			
			MainForm.scheduling = bool.Parse(tempStrings[2]);
			
			MainForm.schedulingStart = tempStrings[3];
			
			MainForm.schedulingEnd = tempStrings[4];
			
		}
		
		public static void WriteToRegistry()
		{
			
			if (Registry.LocalMachine.OpenSubKey(registryValue) == null)
			{
				
				Registry.LocalMachine.CreateSubKey(registryValue);
				
			}
			
			using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(registryValue, true))
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
					
					Tuple<string, int, VideoType> url = GlobalVariables.finishedUrlList[count];
					
					outfile.Write(string.Format("{0} {1} {2}\n", url.Item1, url.Item2, url.Item3));
					
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
					
					Tuple<string, int, VideoType> url = GlobalVariables.accessUrlList[count];
					
					outfile.Write(string.Format("{0} {1} {2}\n", url.Item1, url.Item2, url.Item3));
					
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
								
								GlobalVariables.urlList.Add(new Tuple<string, int, VideoType>(stringBuilder[0], int.Parse(stringBuilder[1]), MainForm.getVideoFormat(stringBuilder[2])));
								
							}
							
						}
						
					}
						
					sr.Close();
						
				}
				
			}
			
		}
		
	}
	
}


