using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{

    public static class Download
    {
    	
    	public static void delegate_DownloadVideos(int retryCount)
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
							
							Tuple<string, int, VideoType> url = GlobalVariables.urlList[count];
							
							MainForm.selectedQueueIndex = position;
							
							position++;
							
							setup_DownloadVideos(url, position);
							
							Storage.WriteUrlsToFile();
						
						}
						catch(Exception ex)
						{
						
							
							if(retryCount <= 3)
							{
								
								MainForm.statusBar = string.Format("URL {0}: An error has occurred. Retrying.... ({1})", count, retryCount < 3 ? (retryCount + 1).ToString() : "Final Try");
								
								delegate_DownloadVideos(retryCount + 1);
								
							}
							else
							{
								
								var exceptionMessage = ex.Message;
								
								MainForm.statusBar = exceptionMessage.Length <= 100 ? exceptionMessage : string.Format("{0}[...]", exceptionMessage.Substring(0, 100)).ToLower();
								
							}
						
						}
						
					}
					
				}
                
                MainForm.refreshQueue(previouslySelectedIndex);
                 
                MainForm.startDownloadingSession = false;
                
                MainForm.startDownButton.Enabled = true;
                
            }
    		
    	}
    	
        private static void setup_DownloadVideos (Tuple<string, int, VideoType> url, int position)
        {
        	
        	int previouslySelectedIndex = MainForm.selectedQueueIndex;
        	
        	bool restricted = false;
 					
            MainForm.downloadFinishedPercent = 0;
            		
            MainForm.downloadLabel = string.Format("Beginning download from '{0}'", url.Item1);
		
           /*
		    * Get the available video formats.
		    * We'll work with them in the video and audio download examples.
		    */
       		IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url.Item1, false);
		    
       		if(MainForm.currentlyDownloading)
			{  
		    	
       			if((url.Item3 != VideoType.Mp4 && videoInfos.Any(info => (info.Resolution == url.Item2 && info.VideoType == url.Item3)) || url.Item3 == VideoType.Mp4 && url.Item2 == 360))
	            {
       				
		            VideoInfo tempVideo = videoInfos.First(info => info.VideoType == url.Item3 && info.Resolution == url.Item2);
				            
		            MainForm.downloadLabel = string.Format("Downloading '{0}{1}' at {2}p resolution", (tempVideo.Title.Length <= 56 ? tempVideo.Title : string.Format("{0}[...]", tempVideo.Title.Substring(0, 56))).ToLower(), tempVideo.VideoExtension, tempVideo.Resolution);
				
			        //DownloadAudio(videoInfos);
			                   
			        DownloadVideo(videoInfos, url.Item2, url.Item3);
					           
			        GlobalVariables.finishedUrlList.Add(url);
					          
			        Storage.WriteUrlsToBackupFile();
	                    	
	            }
       			else
       			{
       				
       				restricted = true;
       				
       			}
	            
	            if (videoInfos.Where(info => info.VideoType == url.Item3).All(info => info.Resolution != url.Item2) || restricted)
				{
		                    	
					List<int> resolutionsEstablished = new List<int>();
					
					List<VideoType> formatsEstablished = new List<VideoType>();
		             
					using (StreamWriter outfile = new StreamWriter("Acceptable Options.txt"))
					{
						
						outfile.Write(string.Format("This file will show you all formats available for the current URL, as well as the resolutions that are acceptable for that URL.\n\n{0}:\n", url.Item1));
						
						foreach (VideoType format in videoInfos.Where(info => info.VideoType != VideoType.Unknown && formatsEstablished.All(format => info.VideoType != format)).Select(info => info.VideoType))
						{		
							
							if(format == VideoType.Mp4)
							{
								
								outfile.Write(string.Format("Format: {0} | Resolution: {1}p\n", format, "360"));
								
							}
							else
							{
								
								foreach (int resolution in videoInfos.Where(info => info.Resolution >= 144 && info.Resolution < 720 && resolutionsEstablished.All(res => info.Resolution != res) && info.VideoType == format).Select(info => info.Resolution))
								{
					                    		
									outfile.Write(string.Format("Format: {0} | Resolution: {1}p\n", format, resolution));
					               		
									resolutionsEstablished.Add(resolution);
					               		
								}
								
							}
							
							resolutionsEstablished.Clear();
							
							formatsEstablished.Add(format);
							
						}
						
					}
		                    	
					MainForm.statusBar = "An acceptable options file has been exported to the program's root folder. Check there for more information.";
					
					MainForm.currentlyDownloading = false;
	                    	
				}
	            
		    }
			
        }

        private static void DownloadVideo (IEnumerable<VideoInfo> videoInfos, int resolution, VideoType format)
        {
            /*
             * Select the first .mp4 video with 360p resolution
             */
            VideoInfo video = videoInfos
                .First(info => info.VideoType == format && info.Resolution == resolution);

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


