using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using YoutubeExtractor;
using YoutubeDownloadHelper.Gui;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Code
{
    public class Download : IDownload
    {
    	private Settings settingsStore;
    	private Settings UserSettings { get { return settingsStore; } }
    	public Download(IStorage storage)
    	{
    		this.settingsStore = storage.RegistryRead(new Settings());
    	}
    	
        private static string RemoveIllegalPathCharacters (string path)
        {
            string regexSearch = new string (Path.GetInvalidFileNameChars()) + new string (Path.GetInvalidPathChars());
            var r = new Regex (string.Format(CultureInfo.CurrentCulture, "[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
        
        public void DownloadHandler (MainProgramElements mainWindow)
        {
	    	#region Initialization
	    	mainWindow.WindowEnabled = false;
	    	mainWindow.CurrentDownloadOutputText = "Starting Downloading Process....";
	    	
        	int selectedIndex = mainWindow.CurrentlySelectedQueueIndex;
        	var urlList = mainWindow.Videos.ToList().AsReadOnly();
            urlList.WriteToFile(Storage.QueueFile, ".bak");
            List<Video> finishedVideos = new List<Video>();
            List<int> retryCount = new int[urlList.Count + 1].ToList();
            const int maxRetrys = 4;
            
            Thread.Sleep(1000);
            #endregion  
            #region Handle Download Cycle
			int position = 0, urlListCount = urlList.Count;
            while (position < urlListCount)
			{
            	bool exceptionWasCaught = false;
            	bool ableToRetryOnFail = retryCount[position] < maxRetrys;
				var video = urlList[position];
				try
				{
					mainWindow.CurrentlySelectedQueueIndex = position;
					if (retryCount[position] <= 0)
					{
						mainWindow.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Beginning download from '{0}'", video.Location);
					}
					mainWindow.CurrentDownloadProgress = 0;
					
					//var result = await Task.Run(() => Download.SetupDownload(video, mainWindow));
					var result = this.SetupDownload(video, mainWindow);
					
					if (result is DownloadCanceledException)
					{
						mainWindow.CurrentDownloadOutputText = result.Message;
						if (App.IsDebugging) result.Message.Log("Youtube Download Helper");
						Thread.Sleep(!UserSettings.ContinueOnFail ? 1000 : 850);
					}
					else if (result != null) throw result;
					finishedVideos.Add(video);
				}
				catch (Exception ex)
				{
					exceptionWasCaught = true;
					var exceptionMessage = ex.Message;
					retryCount[position] += ex is NotSupportedException ? maxRetrys + 1 : 1;
					if (ableToRetryOnFail)
					{
						mainWindow.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "URL {0}: {1}. Retrying.... ({2}/{3})", video.Position, exceptionMessage.Truncate(50), (retryCount[position]).ToString(CultureInfo.CurrentCulture), maxRetrys);
					}
					else if (!UserSettings.ContinueOnFail)
					{
						mainWindow.CurrentDownloadOutputText = exceptionMessage.Truncate(100);
						ex.Log(GenericCondition.None);
						break;
					}
					if(!UserSettings.ContinueOnFail)
					{
						Thread.Sleep(850);
					}
				}
				if (!exceptionWasCaught || (!ableToRetryOnFail && UserSettings.ContinueOnFail)) position++;
			}
            #endregion
            #region Final Steps
            bool noMajorErrors = retryCount.All(count => count <= maxRetrys);
            if (noMajorErrors || (!noMajorErrors && finishedVideos.Count() > 5))
			{
            	IEnumerable<Video> leftOverVideos = urlList.Where(url => finishedVideos.All(finishedUrl => !url.ToString().Equals(finishedUrl.ToString())));
            	leftOverVideos.Sort();
				mainWindow.Videos = new ObservableCollection<Video>(leftOverVideos);
			}
            
            if (noMajorErrors) mainWindow.CurrentDownloadOutputText = "Finished!";
			else if (string.IsNullOrWhiteSpace(mainWindow.CurrentDownloadOutputText))
			{
				mainWindow.CurrentDownloadOutputText = "An Error Has Occurred!";
			}
			
            if (mainWindow.Videos.Any())
            {
            	mainWindow.RefreshQueue(mainWindow.Videos, selectedIndex > mainWindow.Videos.All() ? 0 : selectedIndex);
            }
            mainWindow.Videos.WriteToFile(Storage.QueueFile);
            mainWindow.WindowEnabled = true;
            #endregion
        }

//        private void DownloadThread ()
//        {
//        	
//        }
        
        private Exception SetupDownload (Video video, MainProgramElements mainWindow)
        {
        	try
        	{
	        	IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(video.Location, false);
	        	VideoInfo currentVideo = default(VideoInfo);
	        	
	        	if(video.IsAudioFile || video.VideoFormat != VideoType.Mp4 || (!video.IsAudioFile && video.VideoFormat == VideoType.Mp4 && video.Quality == 360))
	        	{
	        		currentVideo = videoInfos.FirstOrDefault(info => !video.IsAudioFile ? (info.VideoType == video.VideoFormat && info.Resolution == video.Quality) : (video.AudioFormat == AudioType.Mp3 && info.AudioType == video.AudioFormat && info.AudioBitrate == video.Quality));
	        	}
	        		
	        	if (currentVideo != default(VideoInfo))
	            {
	        		mainWindow.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Downloading (#{0}/{1}) '{2}{3}' at {4}{5}", video.Position, mainWindow.Videos.Count(), currentVideo.Title.Truncate(56), !video.IsAudioFile ? currentVideo.VideoExtension : currentVideo.AudioExtension, !video.IsAudioFile ? currentVideo.Resolution : currentVideo.AudioBitrate, !video.IsAudioFile ? "p resolution" : " bitrate");
					
	        		this.Downloader(videoInfos, mainWindow, video);
	        		
	        		return null;
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
	                                var validVideos = videoInfos.Where(videoInfo => (videoInfo.Resolution >= UrlShaping.MinimumQuality[typeof(VideoType)] && videoInfo.Resolution <= UrlShaping.MaximumQuality) && videoInfo.VideoType.Equals(format) && qualitiesEstablished.All(quality => videoInfo.Resolution != quality)).Select(videoInfo => videoInfo.Resolution);
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
							var validAudioTracks = videoInfos.Where(videoInfo => (videoInfo.AudioBitrate >= UrlShaping.MinimumQuality[typeof(AudioType)] && videoInfo.AudioBitrate <= UrlShaping.MaximumQuality) && videoInfo.AudioType.Equals(format)).Select(videoInfo => videoInfo.AudioBitrate);
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
        	catch (Exception ex) { return ex; }
        }

        private void Downloader (IEnumerable<VideoInfo> videoInfos, MainProgramElements mainWindow, Video videoToUse)
        {
        	bool audioTrack = videoToUse.IsAudioFile;
            VideoInfo video = videoInfos.First(info => !audioTrack ? (info.VideoType == videoToUse.VideoFormat && info.Resolution == videoToUse.Quality) : (info.AudioType == videoToUse.AudioFormat && info.AudioBitrate == videoToUse.Quality));
            
            if (video.RequiresDecryption) DownloadUrlResolver.DecryptDownloadUrl(video);
            
            string videoName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", RemoveIllegalPathCharacters(video.Title), !audioTrack ? video.VideoExtension : video.AudioExtension);
            string temporaryDownloadPath = Path.Combine(this.UserSettings.TemporarySaveLocation, videoName);
            string movingPath = Path.Combine(this.UserSettings.MainSaveLocation, videoName);
            if (this.UserSettings.ValidationLocations.All(path => !File.Exists(Path.Combine(path, videoName))) && !File.Exists(movingPath))
            {
        		if(audioTrack)
        		{
			        var audioDownloader = new AudioDownloader (video, temporaryDownloadPath);;
			        audioDownloader.AudioExtractionProgressChanged += (sender, args) => mainWindow.CurrentDownloadProgress = (int)(85 + args.ProgressPercentage * 0.15);
			        audioDownloader.Execute();
        		}
        		else
        		{
        			var videoDownloader = new VideoDownloader (video, temporaryDownloadPath);
        			videoDownloader.DownloadProgressChanged += ((sender, args) => mainWindow.CurrentDownloadProgress = (int)args.ProgressPercentage);
	                videoDownloader.Execute();
        		}
        		if (!temporaryDownloadPath.Equals(movingPath, StringComparison.OrdinalIgnoreCase)) File.Move(temporaryDownloadPath, movingPath);
            }
            else
            {
            	throw new DownloadCanceledException(string.Format(CultureInfo.CurrentCulture, "The download of #{0} '{1}({2})' has been canceled because it already existed.", videoToUse.Position, RemoveIllegalPathCharacters(video.Title).Truncate(10), videoToUse.Location.Truncate(100)));
            }
        }
    }
}


