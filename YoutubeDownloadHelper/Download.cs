using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{

    public static class Download
    {
    	
    	public static void delegate_DownloadVideos()
    	{
    		
    		int previouslySelectedIndex = MainForm.selectedQueueIndex;
        	
            if (MainForm.urlList.Items.Count > 0)
            {
            	
                MainForm.startDownloadingSession = true;
            	
                int position = 0;
				
				for (int count = 0, GlobalVariablesurlListCount = GlobalVariables.urlList.Count; count < GlobalVariablesurlListCount; count++)
				{
					
					if(MainForm.currentlyDownloading)
					{
						
						try
						{
						
							KeyValuePair<string, int> url = GlobalVariables.urlList[count];
							
							MainForm.selectedQueueIndex = position;
							
							position++;
							
							setup_DownloadVideos(url, position);
							
							Storage.WriteUrlsToFile();
						
						}
						catch(Exception ex)
						{
						
							var exceptionMessage = ex.Message;
							
							MainForm.statusBar = exceptionMessage.Length <= 18 ? exceptionMessage : string.Format("{0}[...]", exceptionMessage.Substring(0, 18)).ToLower();
						
						}
						
					}
					
				}
                
                MainForm.refreshQueue(previouslySelectedIndex);
                 
                MainForm.startDownloadingSession = false;
                
                MainForm.startDownButton.Enabled = true;
                
            }
    		
    	}
    	
        private static void setup_DownloadVideos (KeyValuePair<string, int> url, int position)
        {
        	
        	int previouslySelectedIndex = MainForm.selectedQueueIndex;
 					
            MainForm.downloadFinishedPercent = 0;
            		
            MainForm.downloadLabel = string.Format("Beginning download from '{0}'", url.Key);
		
           /*
		    * Get the available video formats.
		    * We'll work with them in the video and audio download examples.
		    */
       		IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url.Key, false);
		    
       		if(MainForm.currentlyDownloading)
			{  
		    	
	            if(videoInfos.Any(info => info.Resolution == url.Value))
	            {
	                    	
	            	VideoInfo tempVideo = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == url.Value);
			            
	            	MainForm.downloadLabel = string.Format("Downloading '{0}{1}' at {2}p resolution", (tempVideo.Title.Length <= 56 ? tempVideo.Title : string.Format("{0}[...]", tempVideo.Title.Substring(0, 56))).ToLower(), tempVideo.VideoExtension, tempVideo.Resolution);
				
		            //DownloadAudio(videoInfos);
		                    
		            DownloadVideo(videoInfos, url.Value);
				            
		            GlobalVariables.finishedUrlList.Add(url);
				           
		            Storage.WriteUrlsToBackupFile();
	                    	
	            }
	            else
	            {
	                   	
		            string acceptableResolutions = null;
		                    	
		            List<int> resolutionsEstablished = new List<int>();
		                    	
		            foreach(int resolution in videoInfos.Where(info => info.Resolution >= 144 && info.Resolution < 720 && resolutionsEstablished.All(res => info.Resolution != res)).Select(info => info.Resolution))
		            {
		                    		
		            	acceptableResolutions += string.Format("{0}p ", resolution);
		               		
		            	resolutionsEstablished.Add(resolution);
		               		
		             }
		                    	
		             MainForm.statusBar = string.Format("resolution for URL #{0} is not corrent. Please choose any of the following: {1}", position, acceptableResolutions);
	                    	
	            }
	            
		    }
			
        }

        private static void DownloadVideo (IEnumerable<VideoInfo> videoInfos, int resolution)
        {
            /*
             * Select the first .mp4 video with 360p resolution
             */
            VideoInfo video = videoInfos
                .First(info => info.VideoType == VideoType.Mp4 && info.Resolution == resolution);

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the video downloader.
             * The first argument is the video to download.
             * The second argument is the path to save the video file.
             */
            
            var videoPath = Path.Combine(MainForm.tempDownloadLocation, MainForm.RemoveIllegalPathCharacters(video.Title) + video.VideoExtension);
            
            var finalPath = Path.Combine(MainForm.downloadLocation, MainForm.RemoveIllegalPathCharacters(video.Title) + video.VideoExtension);
			
            if (!File.Exists(finalPath))
            {
				
                var videoDownloader = new VideoDownloader (video, videoPath);
	
                // Register the ProgressChanged event and print the current progress
                videoDownloader.DownloadProgressChanged += (sender, args) => MainForm.downloadFinishedPercent = (int)args.ProgressPercentage;
	
                /*
	             * Execute the video downloader.
	             * For GUI applications note, that this method runs synchronously.
	             */
                videoDownloader.Execute();
                
                File.Move(videoPath, finalPath);
				
            }
            else
            {
            	
            	MainForm.statusBar = string.Format("{0} already exists! Download process has been aborted and considered successful.", video.Title.Length <= 18 ? video.Title : string.Format("{0}[...]", video.Title.Substring(0, 18))).ToLower();
            	
            }
			
        }
		        
        
        private static void DownloadAudio (IEnumerable<VideoInfo> videoInfos)
        {
            /*
             * We want the first extractable video with the highest audio quality.
             */
            VideoInfo video = videoInfos
                .Where(info => info.CanExtractAudio)
                .OrderByDescending(info => info.AudioBitrate)
                .First();

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the audio downloader.
             * The first argument is the video where the audio should be extracted from.
             * The second argument is the path to save the audio file.
             */

            var audioDownloader = new AudioDownloader (video, Path.Combine(MainForm.tempDownloadLocation, MainForm.RemoveIllegalPathCharacters(video.Title) + video.AudioExtension));

            // Register the progress events. We treat the download progress as 85% of the progress
            // and the extraction progress only as 15% of the progress, because the download will
            // take much longer than the audio extraction.
            
            //audioDownloader.DownloadProgressChanged += (sender, args) => MainForm.downloadFinishedPercent = (int)(args.ProgressPercentage * 0.85);
            
            audioDownloader.AudioExtractionProgressChanged += (sender, args) => MainForm.downloadFinishedPercent = (int)(85 + args.ProgressPercentage * 0.15);

            /*
             * Execute the audio downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            audioDownloader.Execute();
            
        }
        
    }
	
}


