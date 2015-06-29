using System;
using System.IO;

namespace YoutubeDownloadHelper
{
	public static class Validation
	{
		
		public static void CheckOrCreateFolder(string folderName)
		{
			
			if(!Directory.Exists(folderName))
			{
				
				Directory.CreateDirectory(folderName);
				
			}
			
		}
		
	}
}


