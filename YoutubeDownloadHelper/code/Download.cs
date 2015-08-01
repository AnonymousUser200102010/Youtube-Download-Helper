using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExtractor;
using YoutubeDownloadHelper.Gui;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Code
{

    public class Download : IDownload
    {
        private readonly IStorage Storage;
        private readonly IConversion Conversion;
        private delegate MainProgramElements GetMPE();

        public Download (IStorage store, IConversion convert)
        {
    		
            this.Storage = store;
            this.Conversion = convert;
    		
        }

        public ObservableCollection<Video> GetUnfinishedDownloads (ObservableCollection<Video> finishedUrls)
        {
			VideosToDownload videoQueue = new VideosToDownload();
            ObservableCollection<Video> returnValue = new ObservableCollection<Video> ();
            ObservableCollection<Video> urlList = videoQueue.Items;
            
            foreach (Video vid in urlList.Where(video => finishedUrls.Any(item => video == item)))
            {
            	
                returnValue.Add(vid);
            	
            }
				
            return returnValue;
			
        }

        private static string RemoveIllegalPathCharacters (string path)
        {
            string regexSearch = new string (Path.GetInvalidFileNameChars()) + new string (Path.GetInvalidPathChars());
            var r = new Regex (string.Format(CultureInfo.CurrentCulture, "[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
        
        public async void DownloadHandler (MainWindow mainWindow, int selectedIndex)
        {
        	ObservableCollection<Video> urlList = new ObservableCollection<Video>();
        	mainWindow.Dispatcher.Invoke((Action)(() =>
		    {
		        urlList = mainWindow.MainProgramElements.Videos;
		    }));
            ObservableCollection<Video> finishedUrls = new ObservableCollection<Video> ();
            ClassContainer classCont = new ClassContainer();
            int[] retryCount = new int[urlList.Count+1];
            const int maxRetrys = 4;
            
            for (int position = 0, urlListCount = urlList.Count; position < urlListCount; position++)
            {
	            try
	            {
	            	Video vid = urlList [position];
	            	mainWindow.Dispatcher.Invoke((Action)(() =>
				    {
				        mainWindow.MainProgramElements.CurrentlySelectedQueueIndex = position;
				        if(retryCount[position] <= 0)
			        	{
			        		mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Beginning download from '{0}'", vid.Location);
			        	}
			        	mainWindow.MainProgramElements.CurrentDownloadProgress = 0;
				    }));
								
	                if (await DownloadVideo(vid, mainWindow))
	                {
	                    finishedUrls.Add(vid);
	                }
	            }
	            catch (Exception ex)
	            {	
	                var exceptionMessage = ex.Message;
					retryCount[position]++;
	                if (retryCount[position] <= maxRetrys)
	                {
	                	mainWindow.Dispatcher.Invoke((Action)(() =>
					    {
					        mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "URL {0}: {1}. Retrying.... ({2}/{3})", position + 1, classCont.ConversionCode.Truncate(exceptionMessage, 50), (retryCount[position]).ToString(CultureInfo.CurrentCulture), maxRetrys);
					    }));
	                    Thread.Sleep(850);						
	                    position--;
	                }
	                else
	                {	
	                    if (finishedUrls.Count < 5)
	                    {           		
	                        finishedUrls.Clear();                   	
	                    }
	                    mainWindow.Dispatcher.Invoke((Action)(() =>
					    {
					        mainWindow.MainProgramElements.CurrentDownloadOutputText = classCont.ConversionCode.Truncate(exceptionMessage, 100);
	                    	mainWindow.MainProgramElements.CurrentDownloadProgress = 0;
					    }));
	                    break;
	                }
	            }
	        }
            classCont.IOHandlingCode.WriteUrlsToFile(finishedUrls, true);
            mainWindow.Dispatcher.Invoke((Action)(() =>
			{
	            if(finishedUrls.Count > 0)
	            {
	            	mainWindow.Videos.Replace((new ObservableCollection<Video>(urlList.Where(video => finishedUrls.All(item => item != video)).Select(video => video))));
	            }
				if (mainWindow.MainProgramElements.Videos.Count > 0)
	            {
					mainWindow.RefreshQueue(mainWindow.MainProgramElements.Videos, selectedIndex < mainWindow.MainProgramElements.Videos.Count ? 0 : selectedIndex);
	            }
				classCont.IOHandlingCode.WriteUrlsToFile(mainWindow.Videos, false);
				mainWindow.MainProgramElements.CurrentDownloadOutputText = retryCount.Any(count => count > maxRetrys) ? "An Error Has Occurred!" : "Finished!";
				mainWindow.MainProgramElements.CurrentDownloadProgress = 0;
				mainWindow.MainProgramElements.WindowEnabled = true;
			}));
        }

        private async Task<bool> DownloadVideo (Video video, MainWindow mainWindow)
        {       	
		
            /*
		    * Get the available video formats.
		    * We'll work with them in the video and audio download examples.
		    */
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(video.Location, false);
		    
            if ((video.Format != VideoType.Mp4 && videoInfos.Any(info => (info.Resolution == video.Resolution && info.VideoType == video.Format)) || video.Format == VideoType.Mp4 && video.Resolution == 360))
            {
       				
                VideoInfo currentVideo = videoInfos.First(info => info.VideoType == video.Format && info.Resolution == video.Resolution);
                mainWindow.Dispatcher.Invoke((Action)(() =>
				{
					mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Downloading '{0}{1}' at {2}p resolution", Conversion.Truncate(currentVideo.Title, 56), currentVideo.VideoExtension, currentVideo.Resolution);
				}));
				
                //DownloadAudio(videoInfos);
                await this.Download_Actual(videoInfos, mainWindow, video);
	            return true;
            }
            
            if (videoInfos.Where(info => info.VideoType == video.Format).All(info => info.Resolution != video.Resolution) || (video.Format == VideoType.Mp4 && video.Resolution != 360))
            {      	
                List<int> resolutionsEstablished = new List<int> ();
                List<VideoType> formatsEstablished = new List<VideoType> ();
                using (StreamWriter outfile = new StreamWriter ("Acceptable Options.txt"))
                {
                    outfile.Write(string.Format(CultureInfo.CurrentCulture, "This file will show you all formats available for the current URL, as well as the resolutions that are acceptable for that URL.\n\n{0}:\n", video.Location));
						
                    foreach (VideoType format in videoInfos.Where(info => info.VideoType != VideoType.Unknown && formatsEstablished.All(format => info.VideoType != format)).Select(info => info.VideoType))
                    {		
                        if (format == VideoType.Mp4)
                        {
                            outfile.Write(string.Format(CultureInfo.CurrentCulture, "Format: {0} | Resolution: {1}p\n", format, "360"));
                        }
                        else
                        {
                            foreach (int resolution in videoInfos.Where(info => info.Resolution >= 144 && info.Resolution < 720 && resolutionsEstablished.All(res => info.Resolution != res) && info.VideoType == format).Select(info => info.Resolution))
                            {    		
                                outfile.Write(string.Format(CultureInfo.CurrentCulture, "Format: {0} | Resolution: {1}p\n", format, resolution));	
                                resolutionsEstablished.Add(resolution);	
                            }
                        }
                        resolutionsEstablished.Clear();
                        formatsEstablished.Add(format);
                    }
                }
                throw new NotSupportedException("An acceptable options file has been exported to the program's root folder. Check there for more information.");
            }
            return false;
        }

        private async Task<int> Download_Actual (IEnumerable<VideoInfo> videoInfos, MainWindow mainWindow, Video videoToUse)
		{
			/*
             * Select the first .mp4 video with 360p resolution
             */
			VideoInfo video = videoInfos
                .First(info => info.VideoType == videoToUse.Format && info.Resolution == videoToUse.Resolution);

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
            
			Settings settings = Storage.ReadFromRegistry();
			var videoName = RemoveIllegalPathCharacters(video.Title) + video.VideoExtension;
			var videoPath = Path.Combine(settings.TemporarySaveLocation, videoName);
			var finalPath = Path.Combine(settings.MainSaveLocation, videoName);
            
			if (!File.Exists(finalPath))
			{
				var videoDownloader = new VideoDownloader(video, videoPath);
				// Register the ProgressChanged event and print the current progress
				mainWindow.Dispatcher.Invoke((Action)(() =>
				{
					videoDownloader.DownloadProgressChanged += (
					    (sender, args) => mainWindow.MainProgramElements.CurrentDownloadProgress = (int)args.ProgressPercentage
					);
				}));
				
				/*
	             * Execute the video downloader.
	             * For GUI applications note, that this method runs synchronously.
	             */
				videoDownloader.Execute();
				if(!videoPath.Equals(finalPath, StringComparison.OrdinalIgnoreCase))
				{
					File.Move(videoPath, finalPath);
				}
			}
			return 0;
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


