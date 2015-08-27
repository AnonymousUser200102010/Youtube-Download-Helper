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
        private static string RemoveIllegalPathCharacters (string path)
        {
            string regexSearch = new string (Path.GetInvalidFileNameChars()) + new string (Path.GetInvalidPathChars());
            var r = new Regex (string.Format(CultureInfo.CurrentCulture, "[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        public async void DownloadHandler (MainWindow mainWindow, int selectedIndex)
        {
        	Console.WriteLine(mainWindow.Dispatcher.CheckAccess());
            var urlList = new Collection<Video> ();
            var finishedUrls = new Collection<Video> ();
            mainWindow.Dispatcher.Invoke((Action)(() =>
            {
                urlList = mainWindow.MainProgramElements.Videos;
            }));
            var retryCount = new int[urlList.Count + 1];
            const int maxRetrys = 4;
            
            for (int position = 0, urlListCount = urlList.Count; position < urlListCount; position++)
            {
                try
                {
                    var vid = urlList[position];
                    mainWindow.Dispatcher.Invoke((Action)(() =>
                    {
                        mainWindow.MainProgramElements.CurrentlySelectedQueueIndex = position;
                        if (retryCount[position] <= 0)
                        {
                            mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Beginning download from '{0}'", vid.Location);
                        }
                        mainWindow.MainProgramElements.CurrentDownloadProgress = 0;
                    }));
                    if (await DownloadVideo(vid, mainWindow)) finishedUrls.Add(vid);
                }
                catch (Exception ex)
                {	
                    var exceptionMessage = ex.Message;
                    retryCount[position]++;
                    if (retryCount[position] <= maxRetrys)
                    {
                        mainWindow.Dispatcher.Invoke((Action)(() =>
                        {
                            mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "URL {0}: {1}. Retrying.... ({2}/{3})", position + 1, exceptionMessage.Truncate(50), (retryCount[position]).ToString(CultureInfo.CurrentCulture), maxRetrys);
                        }));
                        Thread.Sleep(850);						
                        position--;
                    }
                    else
                    {	
                        if (finishedUrls.Count < 5) finishedUrls.Clear();                   	
                        mainWindow.Dispatcher.Invoke((Action)(() =>
                        {
                            mainWindow.MainProgramElements.CurrentDownloadOutputText = exceptionMessage.Truncate(100);
                        }));
                        ex.Log(GenericCondition.None);
                        break;
                    }
                }
            }
            finishedUrls.WriteToFile(Storage.File, ".bak");
            mainWindow.Dispatcher.Invoke((Action)(() =>
            {
				if (finishedUrls.Any()) mainWindow.MainProgramElements.Videos.Replace(urlList.Where(video => finishedUrls.All(item => item != video)));
                if (mainWindow.MainProgramElements.Videos.Any())
                {
                	mainWindow.RefreshQueue(mainWindow.MainProgramElements.Videos, selectedIndex > mainWindow.MainProgramElements.Videos.All() ? mainWindow.MainProgramElements.Videos.All() : selectedIndex);
                }
                mainWindow.MainProgramElements.Videos.WriteToFile(Storage.File);
				if (retryCount.All(count => count <= maxRetrys)) mainWindow.MainProgramElements.CurrentDownloadOutputText = "Finished!";
				else if (string.IsNullOrWhiteSpace(mainWindow.MainProgramElements.CurrentDownloadOutputText))
				{
					mainWindow.MainProgramElements.CurrentDownloadOutputText = "An Error Has Occurred!";
				}
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
		    
            if ((video.VideoFormat != VideoType.Mp4 && videoInfos.Any(info => (info.Resolution == video.Quality && info.VideoType == video.VideoFormat)) || video.VideoFormat == VideoType.Mp4 && video.Quality == 360) || video.IsAudioFile)
            {
                VideoInfo currentVideo = videoInfos.First(info => !video.IsAudioFile ? (info.VideoType == video.VideoFormat && info.Resolution == video.Quality) : (info.AudioType == video.AudioFormat && info.AudioBitrate == video.Quality));
                mainWindow.Dispatcher.Invoke((Action)(() =>
                {
                    mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Downloading '{0}{1}' at {2}{3}", currentVideo.Title.Truncate(56), currentVideo.VideoExtension, currentVideo.Resolution, !video.IsAudioFile ? "p resolution" : " bitrate");
                }));
				
                //DownloadAudio(videoInfos);
                await this.Download_Actual(videoInfos, mainWindow, video);
                return true;
            }
            
            if (videoInfos.Where(info => info.VideoType == video.VideoFormat).All(info => info.Resolution != video.Quality) || (video.VideoFormat == VideoType.Mp4 && video.Quality != 360 && !video.IsAudioFile))
            {      	
                var resolutionsEstablished = new List<int> ();
                var formatsEstablished = new List<VideoType> ();
                var audioQualitiesEstablished = new List<AudioType> ();
                using (StreamWriter outfile = new StreamWriter ("Acceptable Options.txt"))
                {
                    outfile.Write(string.Format(CultureInfo.CurrentCulture, "This file will show you all formats available for the current URL, as well as the resolutions that are acceptable for that URL.\n\n{0}:\nVideo Formats:\n", video.Location));
                    for (var position = videoInfos.Where(info => info.VideoType != VideoType.Unknown && formatsEstablished.All(format => info.VideoType != format)).Select(info => info.VideoType).GetEnumerator(); position.MoveNext();)
                    {
                        VideoType format = position.Current;
                        switch (format)
                        {
                            case VideoType.Mp4:
                                outfile.Write(string.Format(CultureInfo.CurrentCulture, "Format: {0} | Resolution: {1}p\n", format, "360"));
                                break;
                            default:
                                for (var subPosition = videoInfos.Where(info => info.Resolution >= 144 && info.Resolution < 720 && resolutionsEstablished.All(res => info.Resolution != res) && info.VideoType == format).Select(info => info.Resolution).GetEnumerator(); position.MoveNext();)
                                {
                                    int resolution = subPosition.Current;
                                    outfile.Write(string.Format(CultureInfo.CurrentCulture, "Format: {0} | Resolution: {1}p\n", format, resolution));
                                    resolutionsEstablished.Add(resolution);
                                }
                                break;
                        }
                        resolutionsEstablished.Clear();
                        formatsEstablished.Add(format);
                    }
                    outfile.Write("Audio Formats:\n");
                    for (var position = videoInfos.Where(info => info.AudioType != AudioType.Unknown && audioQualitiesEstablished.All(quality => info.AudioType != quality)).Select(info => info.AudioType).GetEnumerator(); position.MoveNext();)
                    {
                        AudioType format = position.Current;
                        for (var subPosition = videoInfos.Where(info => resolutionsEstablished.All(bitrate => info.AudioBitrate != bitrate) && info.AudioType == format).Select(info => info.AudioBitrate).GetEnumerator(); position.MoveNext();)
                        {
                            int resolution = subPosition.Current;
                            outfile.Write(string.Format(CultureInfo.CurrentCulture, "Format: {0} | Bitrate: {1}p\n", format, resolution));
                            resolutionsEstablished.Add(resolution);
                        }
                        resolutionsEstablished.Clear();
                        audioQualitiesEstablished.Add(format);
                    }
                }
                throw new NotSupportedException ("An acceptable options file has been exported to the program's root folder. Check there for more information.");
            }
            return false;
        }

        private async Task<int> Download_Actual (IEnumerable<VideoInfo> videoInfos, MainWindow mainWindow, Video videoToUse)
        {
            /*
             * Select the first .mp4 video with 360p resolution
             */
            VideoInfo video = videoInfos
                .First(info => info.VideoType == videoToUse.VideoFormat && info.Resolution == videoToUse.Quality);

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption) DownloadUrlResolver.DecryptDownloadUrl(video);

            /*
             * Create the video downloader.
             * The first argument is the video to download.
             * The second argument is the path to save the video file.
             */
            
            var classCont = new ClassContainer();
            Settings settings = classCont.IOCode.RegistryRead(new Settings());
            var videoName = RemoveIllegalPathCharacters(video.Title) + video.VideoExtension;
            var videoPath = Path.Combine(settings.TemporarySaveLocation, videoName);
            var finalPath = Path.Combine(settings.MainSaveLocation, videoName);
            
            if (!File.Exists(finalPath))
            {
                var videoDownloader = new VideoDownloader (video, videoPath);
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
                if (!videoPath.Equals(finalPath, StringComparison.OrdinalIgnoreCase)) File.Move(videoPath, finalPath);
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


