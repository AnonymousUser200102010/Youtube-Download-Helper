using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private static string RemoveIllegalPathCharacters (string path)
        {
            string regexSearch = new string (Path.GetInvalidFileNameChars()) + new string (Path.GetInvalidPathChars());
            var r = new Regex (string.Format(CultureInfo.CurrentCulture, "[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
        
        private static Collection<Video> Sort (Collection<Video> collectionToSort)
        {
        	for (var position = collectionToSort.GetEnumerator(); position.MoveNext();)
			{
        		position.Current.Position = collectionToSort.IndexOf(position.Current);
			}
			return collectionToSort;
        }

        public void DownloadHandler (MainWindow mainWindow, int selectedIndex)
        {
            var urlList = new Collection<Video> ();
            mainWindow.Dispatcher.Invoke((Action)(() =>
            	urlList.Replace(mainWindow.MainProgramElements.Videos))
            );
            urlList.WriteToFile(Storage.QueueFile, ".bak");
            int finishedVideoCount = 0;
            int[] retryCount = new int[urlList.Count + 1];
            const int maxRetrys = 4;
            
			int position = 0, urlListCount = urlList.Count;
            while (position < urlListCount)
			{
            	bool exceptionWasCaught = false;
				var video = urlList[position];
				try
				{
					mainWindow.Dispatcher.Invoke((Action)(() => 
					{
						mainWindow.MainProgramElements.CurrentlySelectedQueueIndex = position;
						if (retryCount[position] <= 0)
						{
							mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Beginning download from '{0}'", video.Location);
						}
						mainWindow.MainProgramElements.CurrentDownloadProgress = 0;
					}));
					
					//var result = await Task.Run(() => Download.SetupDownload(video, mainWindow));
					var result = Download.SetupDownload(video, mainWindow);
					
					if (!result.Item1)
					{
						urlList.Remove(video);
						finishedVideoCount++;
					}
					else if (result.Item2 is OperationCanceledException)
					{
						mainWindow.Dispatcher.Invoke((Action)(() => 
						{
							mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "The download of '{0}' has been canceled for a non-fatal reason.", video.Location);
						}));
						if (App.IsDebugging) result.Item2.Message.Log("Youtube Download Helper");
						Thread.Sleep(850);
					}
					else throw result.Item2;
				}
				catch (Exception ex)
				{
					exceptionWasCaught = true;
					var exceptionMessage = ex.Message;
					retryCount[position] += ex is NotSupportedException ? maxRetrys + 1 : 1;
					if (retryCount[position] <= maxRetrys)
					{
						mainWindow.Dispatcher.Invoke((Action)(() => 
						{
							mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "URL {0}: {1}. Retrying.... ({2}/{3})", video.Position, exceptionMessage.Truncate(50), (retryCount[position]).ToString(CultureInfo.CurrentCulture), maxRetrys);
						}));
						Thread.Sleep(850);
					}
					else
					{
						mainWindow.Dispatcher.Invoke((Action)(() => 
						{
							mainWindow.MainProgramElements.CurrentDownloadOutputText = exceptionMessage.Truncate(100);
						}));
						ex.Log(GenericCondition.None);
						break;
					}
				}
				if (!exceptionWasCaught) position++;
			}
            mainWindow.Dispatcher.Invoke((Action)(() =>
            {
				if (!urlList.Equals(mainWindow.MainProgramElements.Videos) && finishedVideoCount > 5)
				{
					Download.Sort(urlList);
					mainWindow.MainProgramElements.Videos.Replace(urlList);
				}
                if (mainWindow.MainProgramElements.Videos.Any())
                {
                	mainWindow.RefreshQueue(mainWindow.MainProgramElements.Videos, selectedIndex > mainWindow.MainProgramElements.Videos.All() ? mainWindow.MainProgramElements.Videos.All() : selectedIndex);
                }
                mainWindow.MainProgramElements.Videos.WriteToFile(Storage.QueueFile);
				if (retryCount.All(count => count <= maxRetrys)) mainWindow.MainProgramElements.CurrentDownloadOutputText = "Finished!";
				else if (string.IsNullOrWhiteSpace(mainWindow.MainProgramElements.CurrentDownloadOutputText))
				{
					mainWindow.MainProgramElements.CurrentDownloadOutputText = "An Error Has Occurred!";
				}
                mainWindow.MainProgramElements.CurrentDownloadProgress = 0;
                mainWindow.MainProgramElements.WindowEnabled = true;
            }));
        }

//        private void DownloadThread ()
//        {
//        	
//        }
        
        private static Tuple<bool, Exception> SetupDownload (Video video, MainWindow mainWindow)
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
	                mainWindow.Dispatcher.Invoke((Action)(() =>
	                {
	            		mainWindow.MainProgramElements.CurrentDownloadOutputText = string.Format(CultureInfo.InstalledUICulture, "Downloading (#{0}) '{1}{2}' at {3}{4}", video.Position, currentVideo.Title.Truncate(56), !video.IsAudioFile ? currentVideo.VideoExtension : currentVideo.AudioExtension, !video.IsAudioFile ? currentVideo.Resolution : currentVideo.AudioBitrate, !video.IsAudioFile ? "p resolution" : " bitrate");
	                }));
					
	        		Download.Downloader(videoInfos, mainWindow, video);
	        		
	        		return new Tuple<bool, Exception> (false, null);
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
        	catch (Exception ex) { return new Tuple<bool, Exception> (true, ex); }
        }

        private static void Downloader (IEnumerable<VideoInfo> videoInfos, MainWindow mainWindow, Video videoToUse)
        {
        	bool audioTrack = videoToUse.IsAudioFile;
            VideoInfo video = videoInfos.First(info => !audioTrack ? (info.VideoType == videoToUse.VideoFormat && info.Resolution == videoToUse.Quality) : (info.AudioType == videoToUse.AudioFormat && info.AudioBitrate == videoToUse.Quality));
            
            if (video.RequiresDecryption) DownloadUrlResolver.DecryptDownloadUrl(video);
            var classCont = new ClassContainer();
            var settings = classCont.IOCode.RegistryRead(new Settings());
            
            string videoName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", RemoveIllegalPathCharacters(video.Title), !audioTrack ? video.VideoExtension : video.AudioExtension);
            string temporaryDownloadPath = Path.Combine(settings.TemporarySaveLocation, videoName);
            string movingPath = Path.Combine(settings.MainSaveLocation, videoName);
            if (settings.ValidationLocations.All(path => !File.Exists(Path.Combine(path, videoName))) && !File.Exists(movingPath))
            {
        		if(audioTrack)
        		{
			        var audioDownloader = new AudioDownloader (video, temporaryDownloadPath);;
			        
			        mainWindow.Dispatcher.Invoke((Action)(() =>
		            {
		            	audioDownloader.AudioExtractionProgressChanged += 
		            		(sender, args) => mainWindow.MainProgramElements.CurrentDownloadProgress = (int)(85 + args.ProgressPercentage * 0.15);
		            }));
			        
			        audioDownloader.Execute();
        		}
        		else
        		{
        			var videoDownloader = new VideoDownloader (video, temporaryDownloadPath);
        			
	                mainWindow.Dispatcher.Invoke((Action)(() =>
	                {
	                    videoDownloader.DownloadProgressChanged += (
	                        (sender, args) => mainWindow.MainProgramElements.CurrentDownloadProgress = (int)args.ProgressPercentage
	                    );
	                }));
	                
	                videoDownloader.Execute();
        		}
        		if (!temporaryDownloadPath.Equals(movingPath, StringComparison.OrdinalIgnoreCase)) File.Move(temporaryDownloadPath, movingPath);
            }
            else
            {
            	throw new OperationCanceledException();
            }
        }
    }
}


