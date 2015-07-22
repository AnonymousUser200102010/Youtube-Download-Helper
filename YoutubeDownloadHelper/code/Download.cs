using System;
using System.Collections;
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

        public Download (IMainForm mainForm, IStorage store)
        {
    		
            this.MainForm = mainForm;
    		
            this.Storage = store;
    		
        }

        private static ObservableCollection<Video> GetUnfinishedDownloads (ObservableCollection<Video> finishedUrls)
        {
			
            ObservableCollection<Video> returnValue = new ObservableCollection<Video> ();
			
			for (int count = 0, numberOfVideos = finishedUrls.Count; count < numberOfVideos; count++)
			{
				Video videoInfo = finishedUrls[count];
				if (finishedUrls.Count <= 0 || !finishedUrls.Any(item => item.UrlName.Equals(videoInfo.UrlName)))
				{
					returnValue.Add(videoInfo);
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

        private static string Truncate (string stringToCheck, int trunicationCutoff)
        {
    		
            return stringToCheck.Length <= trunicationCutoff ? stringToCheck : string.Format(CultureInfo.InstalledUICulture, "{0}[...]", stringToCheck.Substring(0, trunicationCutoff)).ToLower(CultureInfo.InstalledUICulture);
    		
        }

        public void HandleDownloadingProcesses (int retryCount, ObservableCollection<Video> Urls)
        {
    		
            int previouslySelectedIndex = MainForm.SelectedQueueIndex;
    		
            ObservableCollection<Video> finishedUrls = new ObservableCollection<Video> ();
        	
            if (MainForm.UrlsNumberItems > 0)
            {
            	
                MainForm.StartDownloadingSession(true);
            	
                int position = 0;
				
                for (int count = 0, numberOfVideos = Urls.Count; count < numberOfVideos; count++)
                {
					
                    if (MainForm.CurrentlyDownloading)
                    {
						
                        try
                        {
							
                            Video url = Urls [count];
							
                            MainForm.SelectedQueueIndex = position;
							
                            position++;
							
                            if (DownloadVideo(url, position) != null)
                            {
								
                                finishedUrls.Add(url);
								
                            }
							
                            Storage.WriteUrlsToFile(finishedUrls, true);
						
                        }
                        catch (Exception ex)
                        {
						
                            var exceptionMessage = ex.Message;
							
                            if (retryCount <= 3)
                            {
								
                            	MainForm.StatusBar = string.Format(CultureInfo.InstalledUICulture, "URL {0}: {1}. Retrying.... ({2}/{3})", count + 1, Truncate(exceptionMessage, 50), (retryCount + 1).ToString(CultureInfo.CurrentCulture), "3");
								
                                System.Threading.Thread.Sleep(850);
								
                                HandleDownloadingProcesses((retryCount + 1), Urls);
								
                            }
                            else
                            {
								
                            	if(finishedUrls.Count < 10)
                            	{
                            		
                                	finishedUrls.Clear();
                                	
                            	}
								
                                MainForm.StatusBar = Truncate(exceptionMessage, 100);
								
                            }
						
                        }
						
                    }
					
                }
                
            }
            
            MainForm.CurrentlyDownloading = false;
            
            MainForm.StartDownloadingSession(false);
                
            MainForm.StartDownButtonEnabled = true;
            
            VideoQueue.Items = GetUnfinishedDownloads(finishedUrls);
            
            Storage.WriteUrlsToFile(VideoQueue.Items, false);
            
            MainForm.RefreshQueue(previouslySelectedIndex, true);
    		
        }

        private string DownloadVideo (Video video, int position)
        {
        	
            int previouslySelectedIndex = MainForm.SelectedQueueIndex;
 					
            MainForm.DownloadFinishedPercent = 0;
            		
            MainForm.DownloadLabel = string.Format(CultureInfo.InstalledUICulture, "Beginning download from '{0}'", video.UrlName);
		
            /*
		    * Get the available video formats.
		    * We'll work with them in the video and audio download examples.
		    */
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(video.UrlName, false);
		    
            if (MainForm.CurrentlyDownloading)
            {  
		    	
                if ((video.Format != VideoType.Mp4 && videoInfos.Any(info => (info.Resolution == video.Resolution && info.VideoType == video.Format)) || video.Format == VideoType.Mp4 && video.Resolution == 360))
                {
       				
                    VideoInfo currentVideo = videoInfos.First(info => info.VideoType == video.Format && info.Resolution == video.Resolution);
				            
                    MainForm.DownloadLabel = string.Format(CultureInfo.InstalledUICulture, "Downloading '{0}{1}' at {2}p resolution", Truncate(currentVideo.Title, 56), currentVideo.VideoExtension, currentVideo.Resolution);
				
                    //DownloadAudio(videoInfos);
			                   
                    DownloadVideo(videoInfos, video.Resolution, position, video.Format);
			        
                    return "Success!";
	                    	
                }
       				
	            
                if (videoInfos.Where(info => info.VideoType == video.Format).All(info => info.Resolution != video.Resolution) || (video.Format == VideoType.Mp4 && video.Resolution != 360))
                {
		                    	
                    List<int> resolutionsEstablished = new List<int> ();
					
                    List<VideoType> formatsEstablished = new List<VideoType> ();
		             
                    using (StreamWriter outfile = new StreamWriter ("Acceptable Options.txt"))
                    {
						
                        outfile.Write(string.Format(CultureInfo.InstalledUICulture, "This file will show you all formats available for the current URL, as well as the resolutions that are acceptable for that URL.\n\n{0}:\n", video.UrlName));
						
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
            	
                MainForm.StatusBar = string.Format(CultureInfo.InstalledUICulture, "{0}({1}) already exists! Download process has been aborted and considered successful.", Truncate(video.Title, 18), position).ToLower(CultureInfo.InstalledUICulture);
            	
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


