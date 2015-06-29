using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace YoutubeDownloadHelper
{
	public static class GlobalVariables
	{
		
		public static Collection<KeyValuePair<string, int>> urlList = new Collection<KeyValuePair<string, int>>();
		
		public static Collection<KeyValuePair<string, int>> accessUrlList
		{
			
			get 
			{ 
				
				Collection<KeyValuePair<string, int>> returnValue = new Collection<KeyValuePair<string, int>>();
				
				for (int count = 0, GlobalVariablesurlListCount = GlobalVariables.urlList.Count; count < GlobalVariablesurlListCount; count++)
				{
					
					KeyValuePair<string, int> url = GlobalVariables.urlList[count];
					
					if(finishedUrlList.Count <= 0)
					{
						
						returnValue.Add(url);
						
					}
					else if (!finishedUrlList.Any(item => item.Key.Equals(url.Key)))
					{
						
						returnValue.Add(url);
						
					}
					
				}
				
				return returnValue;
			
			}
			
		}
		
		public static Collection<KeyValuePair<string, int>> finishedUrlList = new Collection<KeyValuePair<string, int>>();
		
		public static bool DownloadImmediately;
		
		public static bool FinishedDownloading;
		
	}
}


