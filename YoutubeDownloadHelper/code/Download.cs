using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{

	public class Download : IDownload
    {
    	
    	private readonly IMainForm MainForm;
    	
    	private readonly IStorage Storage;
    	
    	public Download(IMainForm mainForm, IStorage store)
    	{
    		
    		this.MainForm = mainForm;
    		
    		this.Storage = store;
    		
    	}
    	
    	private static ObservableCollection<Tuple<string, int, VideoType>> GetUnfinishedDownloads(ObservableCollection<Tuple<string, int, VideoType>> finishedUrlList)
		{
			
			ObservableCollection<Tuple<string, int, VideoType>> returnValue = new ObservableCollection<Tuple<string, int, VideoType>>();
			
			for (int count = 0, GlobalVariablesurlListCount = GlobalVariables.urlList.Count; count < GlobalVariablesurlListCount; count++)
			{
				
				Tuple<string, int, VideoType> url = GlobalVariables.urlList[count];
				
				if (finishedUrlList.Count <= 0 || !finishedUrlList.Any(item => item.Item1.Equals(url.Item1)))
				{
					
					returnValue.Add(url);
					
				}
				
			}
				
			return returnValue;
			
		}
    	
    	private static string RemoveIllegalPathCharacters (string path)
        {
            string regexSearch = new string (Path.GetInvalidFileNameChars()) + new string (Path.GetInvalidPathChars());
            var r = new Regex (string.Format(CultureInfo.CurrentCulture, "[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
    	
    	private static string Trunicate(string stringToCheck, int trunicationCutoff)
    	{
    		
    		return stringToCheck.Length <= trunicationCutoff ? stringToCheck : string.Format(CultureInfo.InstalledUICulture, "{0}[...]", stringToCheck.Substring(0, trunicationCutoff)).ToLower(CultureInfo.InstalledUICulture);
    		
    	}
    	
    	public void SetupDownloadingProcess(int retryCount, Collection<Tuple<string, int, VideoType>> urlList)
    	{
    		
    		int previouslySelectedIndex = MainForm.SelectedQueueIndex;
    		
    		ObservableCollection<Tuple<string, int, VideoType>> finishedUrlList = new ObservableCollection<Tuple<string, int, VideoType>>();
        	
            if (MainForm.UrlListNumberItems > 0)
            {
            	
            	MainForm.StartDownloadingSession(true);
            	
                int position = 0;
				
				for (int count = 0, GlobalVariablesurlListCount = urlList.Count; count < GlobalVariablesurlListCount; count++)
				{
					
					if(MainForm.CurrentlyDownloading)
					{
						
						try
						{
							
							Tuple<string, int, VideoType> url = urlList[count];
							
							MainForm.SelectedQueueIndex = position;
							
							position++;
							
							Tuple<string, int, VideoType> tempTuple = DownloadVideos(url, position);
							
							if (tempTuple != null)
							{
								
								finishedUrlList.Add(tempTuple);
								
							}
							
							Storage.WriteUrlsToFile(finishedUrlList, true);
						
						}
						catch(Exception ex)
						{
						
							var exceptionMessage = ex.Message;
							
							if(retryCount <= 3)
							{
								
								MainForm.StatusBar = string.Format(CultureInfo.InstalledUICulture, "URL {0}: {1}. Retrying.... ({2})", count + 1, Trunicate(exceptionMessage, 50), retryCount < 3 ? (retryCount + 1).ToString(CultureInfo.CurrentCulture) : "Final Try");
								
								System.Threading.Thread.Sleep(850);
								
								SetupDownloadingProcess((retryCount + 1), urlList);
								
							}
							else
							{
								
								finishedUrlList.Clear();
								
								MainForm.StatusBar = Trunicate(exceptionMessage, 100);
								
							}
						
						}
						
					}
					
				}
                
            }
            
            MainForm.CurrentlyDownloading = false;
            
            MainForm.StartDownloadingSession(false);
                
            MainForm.StartDownButtonEnabled = true;
            
            GlobalVariables.urlList = GetUnfinishedDownloads(finishedUrlList);
            
            Storage.WriteUrlsToFile(GlobalVariables.urlList, false);
            
            MainForm.RefreshQueue(previouslySelectedIndex, true);
    		
    	}
    	
        private Tuple<string, int, VideoType> DownloadVideos (Tuple<string, int, VideoType> url, int position)
        {
        	
        	int previouslySelectedIndex = MainForm.SelectedQueueIndex;
 					
            MainForm.DownloadFinishedPercent = 0;
            		
            MainForm.DownloadLabel = string.Format(CultureInfo.InstalledUICulture, "Beginning download from '{0}'", url.Item1);
		
           /*
		    * Get the available video formats.
		    * We'll work with them in the video and audio download examples.
		    */
       		IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url.Item1, false);
		    
       		if(MainForm.CurrentlyDownloading)
			{  
		    	
				if ((url.Item3 != VideoType.Mp4 && videoInfos.Any(info => (info.Resolution == url.Item2 && info.VideoType == url.Item3)) || url.Item3 == VideoType.Mp4 && url.Item2 == 360))
				{
       				
					VideoInfo tempVideo = videoInfos.First(info => info.VideoType == url.Item3 && info.Resolution == url.Item2);
				            
					MainForm.DownloadLabel = string.Format(CultureInfo.InstalledUICulture, "Downloading '{0}{1}' at {2}p resolution", Trunicate(tempVideo.Title, 56), tempVideo.VideoExtension, tempVideo.Resolution);
				
					//DownloadAudio(videoInfos);
			                   
					DownloadVideo(videoInfos, url.Item2, position, url.Item3);
			        
					return url;
	                    	
				}
       				
	            
				if (videoInfos.Where(info => info.VideoType == url.Item3).All(info => info.Resolution != url.Item2) || (url.Item3 == VideoType.Mp4 && url.Item2 != 360))
				{
		                    	
					List<int> resolutionsEstablished = new List<int>();
					
					List<VideoType> formatsEstablished = new List<VideoType>();
		             
					using (StreamWriter outfile = new StreamWriter("Acceptable Options.txt"))
					{
						
						outfile.Write(string.Format(CultureInfo.InstalledUICulture, "This file will show you all formats available for the current URL, as well as the resolutions that are acceptable for that URL.\n\n{0}:\n", url.Item1));
						
						foreach (VideoType format in videoInfos.Where(info => info.VideoType != VideoType.Unknown && formatsEstablished.All(format => info.VideoType != format)).Select(info => info.VideoType))
						{		
							
							if (format == VideoType.Mp4)
							{
								
								outfile.Write(string.Format(CultureInfo.InstalledUICulture, "Format: {0} | Resolution: {1}p\n", format, "360"));
								
							}
							else
							{
								
								foreach (int resolution in videoInfos.Where(info => info.Resolution >= 144 && info.Resolution < 720 && resolutionsEstablished.All(res => info.Resolution != res) && info.VideoType == format).Select(info => info.Resolution))
								{
					                    		
									outfile.Write(string.Format(CultureInfo.InstalledUICulture, "Format: {0} | Resolution: {1}p\n", format, resolution));
					               		
									resolutionsEstablished.Add(resolution);
					               		
								}
								
							}
							
							resolutionsEstablished.Clear();
							
							formatsEstablished.Add(format);
							
						}
						
					}
		                    	
					MainForm.StatusBar = "An acceptable options file has been exported to the program's root folder. Check there for more information.";
					
					MainForm.CurrentlyDownloading = false;
	                    	
				}
	            
			}
			
       		return null;
       		
        }

        private void DownloadVideo (IEnumerable<VideoInfo> videoInfos, int resolution, int position, VideoType format)
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
            
			var videoName = RemoveIllegalPathCharacters(video.Title) + video.VideoExtension;
			
            var videoPath = Path.Combine(MainForm.TempDownloadLocation, videoName);
            
            var finalPath = Path.Combine(MainForm.DownloadLocation, videoName);
			
            if (!File.Exists(finalPath))
            {
				
                var videoDownloader = new VideoDownloader (video, videoPath);
	
                // Register the ProgressChanged event and print the current progress
                videoDownloader.DownloadProgressChanged += (sender, args) => MainForm.DownloadFinishedPercent = (int)args.ProgressPercentage;
	
                /*
	             * Execute the video downloader.
	             * For GUI applications note, that this method runs synchronously.
	             */
                videoDownloader.Execute();
                
                File.Move(videoPath, finalPath);
				
            }
            else
            {
            	
            	MainForm.StatusBar = string.Format(CultureInfo.InstalledUICulture, "{0}({1}) already exists! Download process has been aborted and considered successful.", Trunicate(video.Title, 18), position).ToLower(CultureInfo.InstalledUICulture);
            	
            }
			
        }
		
//				TBD        
//        private void DownloadAudio (IEnumerable<VideoInfo> videoInfos)
//        {
//            /*
//             * We want the first extractable video with the highest audio quality.
//             */
//            VideoInfo video = videoInfos
//                .Where(info => info.CanExtractAudio)
//                .OrderByDescending(info => info.AudioBitrate)
//                .First();
//
//            /*
//             * If the video has a decrypted signature, decipher it
//             */
//            if (video.RequiresDecryption)
//            {
//                DownloadUrlResolver.DecryptDownloadUrl(video);
//            }
//
//            /*
//             * Create the audio downloader.
//             * The first argument is the video where the audio should be extracted from.
//             * The second argument is the path to save the audio file.
//             */
//
//            var audioDownloader = new AudioDownloader (video, Path.Combine(MainForm.tempDownloadLocation, RemoveIllegalPathCharacters(video.Title) + video.AudioExtension));
//
//            // Register the progress events. We treat the download progress as 85% of the progress
//            // and the extraction progress only as 15% of the progress, because the download will
//            // take much longer than the audio extraction.
//            
//            //audioDownloader.DownloadProgressChanged += (sender, args) => MainForm.downloadFinishedPercent = (int)(args.ProgressPercentage * 0.85);
//            
//            audioDownloader.AudioExtractionProgressChanged += (sender, args) => MainForm.downloadFinishedPercent = (int)(85 + args.ProgressPercentage * 0.15);
//
//            /*
//             * Execute the audio downloader.
//             * For GUI applications note, that this method runs synchronously.
//             */
//            audioDownloader.Execute();
//            
//        }
//        
    }
	
}


