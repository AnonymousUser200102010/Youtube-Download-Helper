using System;
using System.Collections.ObjectModel;
using System.Linq;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{
	public static class GlobalVariables
	{
		
		public static Collection<Tuple<string, int, VideoType>> urlList = new Collection<Tuple<string, int, VideoType>>();
		
		public static Collection<Tuple<string, int, VideoType>> accessUrlList
		{
			
			get 
			{ 
				
				Collection<Tuple<string, int, VideoType>> returnValue = new Collection<Tuple<string, int, VideoType>>();
				
				for (int count = 0, GlobalVariablesurlListCount = GlobalVariables.urlList.Count; count < GlobalVariablesurlListCount; count++)
				{
					
					Tuple<string, int, VideoType> url = GlobalVariables.urlList[count];
					
					if(finishedUrlList.Count <= 0)
					{
						
						returnValue.Add(url);
						
					}
					else if (!finishedUrlList.Any(item => item.Item1.Equals(url.Item1)))
					{
						
						returnValue.Add(url);
						
					}
					
				}
				
				return returnValue;
			
			}
			
		}
		
		public static Collection<Tuple<string, int, VideoType>> finishedUrlList = new Collection<Tuple<string, int, VideoType>>();
		
		public static bool DownloadImmediately;
		
		public static bool FinishedDownloading;
		
	}
}


