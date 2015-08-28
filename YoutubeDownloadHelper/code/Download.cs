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
                    retryCount[position] += ex is NotSupportedException ? maxRetrys + 1 : 1;
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
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(video.Location, false);
		    
            VideoInfo currentVideo = videoInfos.FirstOrDefault(info => !video.IsAudioFile ? (info.VideoType == video.VideoFormat && info.Resolution == video.Quality) : (video.AudioFormat == AudioType.Mp3 && info.AudioType == video.AudioFormat && info.AudioBitrate == video.Quality));
            
            if (currentVideo != default(VideoInfo) || (video.VideoFormat == VideoType.Mp4 && video.Quality == 360 && !video.IsAudioFile))
            {
                mainWindow.Dispatcher.Invoke((Action)(() =>
                {
            		mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Downloading '{0}{1}' at {2}{3}", currentVideo.Title.Truncate(56), !video.IsAudioFile ? currentVideo.VideoExtension : currentVideo.AudioExtension, !video.IsAudioFile ? currentVideo.Resolution : currentVideo.AudioBitrate, !video.IsAudioFile ? "p resolution" : " bitrate");
                }));
				
            	await this.BackgroundDownloader(videoInfos, mainWindow, video);;
                return true;
            } 
            else
            {      	
                var formatsEstablished = new List<VideoType> ();
                var qualitiesEstablished = new List<int> ();
                using (StreamWriter outfile = new StreamWriter ("Acceptable Options.txt"))
                {
                    outfile.Write(string.Format(CultureInfo.CurrentCulture, "This file will show you all formats available for the current URL, as well as the resolutions that are acceptable for that URL.\n\n{0}:\n\nVideo Formats:\n", video.Location));
                    for (var position = videoInfos.Where(info => info.VideoType != VideoType.Unknown && formatsEstablished.All(format => info.VideoType != format)).Select(info => info.VideoType).GetEnumerator(); position.MoveNext();)
                    {
                        VideoType format = position.Current;
                        formatsEstablished.Add(format);
                        switch (format)
                        {
                            case VideoType.Mp4:
                                outfile.Write(string.Format(CultureInfo.CurrentCulture, "Format: {0} | Resolution: {1}p\n", format, "360"));
                                break;
                            default:
                                var validVideos = videoInfos.Where(videoInfo => (videoInfo.Resolution >= UrlShaping.MinimumQuality[typeof(VideoType).ToString()] && videoInfo.Resolution <= UrlShaping.MaximumQuality) && videoInfo.VideoType.Equals(format) && qualitiesEstablished.All(quality => videoInfo.Resolution != quality)).Select(videoInfo => videoInfo.Resolution);
								foreach (int currentResolution in validVideos)
								{
									outfile.Write(string.Format(CultureInfo.CurrentCulture, "Format: {0} | Resolution: {1}p\n", format, currentResolution));
								}
                                break;
                        }
                        qualitiesEstablished.Clear();
                    }
                    
                    outfile.Write("\nAudio Formats:\n");
                    var audioQualitiesEstablished = new List<AudioType> ();
                    
                    for (var position = videoInfos.Where(info => info.AudioType != AudioType.Unknown && audioQualitiesEstablished.All(quality => info.AudioType != quality)).Select(info => info.AudioType).GetEnumerator(); position.MoveNext();)
                    {
                        AudioType format = position.Current;
                        audioQualitiesEstablished.Add(format);
						var validAudioTracks = videoInfos.Where(videoInfo => (videoInfo.AudioBitrate >= UrlShaping.MinimumQuality[typeof(AudioType).ToString()] && videoInfo.AudioBitrate <= UrlShaping.MaximumQuality) && videoInfo.AudioType.Equals(format)).Select(videoInfo => videoInfo.AudioBitrate);
						foreach (int currentQuality in validAudioTracks)
						{
							outfile.Write(string.Format(CultureInfo.CurrentCulture, "Format: {0} | Bitrate: {1}\n", format, currentQuality));
						}
                        qualitiesEstablished.Clear();
                    }
                }
                throw new NotSupportedException ("An acceptable options file has been exported to the program's root folder. Check there for more information.");
            }
        }

        private async Task<int> BackgroundDownloader (IEnumerable<VideoInfo> videoInfos, MainWindow mainWindow, Video videoToUse)
        {
            VideoInfo video = videoInfos.First(info => !videoToUse.IsAudioFile ? (info.VideoType == videoToUse.VideoFormat && info.Resolution == videoToUse.Quality) : (info.AudioType == videoToUse.AudioFormat && info.AudioBitrate == videoToUse.Quality));
            
            if (video.RequiresDecryption) DownloadUrlResolver.DecryptDownloadUrl(video);
            return await FinalDownloadStep(mainWindow, video, videoToUse.IsAudioFile);
        }
        
        private async Task<int> FinalDownloadStep (MainWindow mainWindow, VideoInfo video, bool audioTrack)
        {
        	var classCont = new ClassContainer();
            Settings settings = classCont.IOCode.RegistryRead(new Settings());
            var videoName = string.Format("{0}{1}", RemoveIllegalPathCharacters(video.Title), !audioTrack ? video.VideoExtension : video.AudioExtension);
            var videoPath = Path.Combine(settings.TemporarySaveLocation, videoName);
            var finalPath = Path.Combine(settings.MainSaveLocation, videoName);
        	
        	if (!File.Exists(finalPath))
            {
        		if(audioTrack)
        		{
			        var audioDownloader = new AudioDownloader (video, videoPath);;
			        
			        mainWindow.Dispatcher.Invoke((Action)(() =>
		            {
		            	audioDownloader.AudioExtractionProgressChanged += 
		            		(sender, args) => mainWindow.MainProgramElements.CurrentDownloadProgress = (int)(85 + args.ProgressPercentage * 0.15);
		            }));
			        
			        audioDownloader.Execute();
        		}
        		else
        		{
        			var videoDownloader = new VideoDownloader (video, videoPath);
        			
	                mainWindow.Dispatcher.Invoke((Action)(() =>
	                {
	                    videoDownloader.DownloadProgressChanged += (
	                        (sender, args) => mainWindow.MainProgramElements.CurrentDownloadProgress = (int)args.ProgressPercentage
	                    );
	                }));
	                
	                videoDownloader.Execute();
        		}
        		if (!videoPath.Equals(finalPath, StringComparison.OrdinalIgnoreCase)) File.Move(videoPath, finalPath);
            }
        	return 0;
        }
    }
}


