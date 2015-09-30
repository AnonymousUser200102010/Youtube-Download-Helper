using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using VideoLibrary;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper.Code
{
    public class VideosToDownload
    {
        private readonly ObservableCollection<Video> _items = new ObservableCollection<Video> ();

        /// <summary>
        /// The items within this collection.
        /// </summary>
        public ObservableCollection<Video> Items { get { return _items; } }

        /// <summary>
        /// This is the video queue.
        /// </summary>
        public VideosToDownload ()
        {
            this.Items.ReadUrls();
        }
    }

    public class Video : INotifyPropertyChanged
    {
        private int _position;
        /// <summary>
        /// The position of the Video.
        /// </summary>
        /// <remarks>
        /// This is in relation to it's containing object.
        /// </remarks>
        public int Position
        {
            get{ return this._position + 1; }
            set
            {
                this._position = value;
                RaisePropertyChanged("Position");
            }
        }

        /// <summary>
        /// The url location of this video.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// The video/audio quality this video will be downloaded in.
        /// </summary>
        public int Quality { get; private set; }

        /// <summary>
        /// The format that the GUI will show.
        /// </summary>
        public string Format { get { return this.IsAudioFile ? this.AudioFormat.ToString() : this.VideoFormat.ToString(); } }

        /// <summary>
        /// The format (or extension) of the video.
        /// </summary>
        public VideoFormat VideoFormat { get; private set; }

        /// <summary>
        /// The format (or extension) of the audio track.
        /// </summary>
        public AudioFormat AudioFormat { get; private set; }

        /// <summary>
        /// The download process for this video will only return an audio track.
        /// </summary>
        public bool IsAudioFile { get; private set; }

        public override string ToString ()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0} {1} {2}\n", this.Location, this.Quality, this.Format);
        }
		
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private void RaisePropertyChanged (string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs (propertyName));
            }
        }
        #endregion
        
        /// <summary>
        /// Represents a video through a set of locally held attributes.
        /// </summary>
        /// <param name="pos">
        /// The position of the Video within the current list.
        /// </param>
        /// <param name="location">
        /// The "location" of the video on the internet. In other words, the string representation of the url for the video.
        /// </param>
        /// <param name="quality">
        /// The consumption quality of the video.
        /// </param>
        /// <param name="format">
        /// The format (or extension) of the video.
        /// </param>
        public Video (int pos, string location, int quality, VideoFormat format)
        {
            this.Position = pos;
            this.Location = location;
            this.Quality = quality;
            this.VideoFormat = format;
            this.AudioFormat = default(AudioFormat);
            this.IsAudioFile = false;
        }
        
        /// <summary>
        /// Represents a video through a set of locally held attributes.
        /// </summary>
        /// <param name="pos">
        /// The position of the Video within the current list.
        /// </param>
        /// <param name="location">
        /// The "location" of the video on the internet. In other words, the string representation of the url for the video.
        /// </param>
        /// <param name="quality">
        /// The consumption quality of the video.
        /// </param>
        /// <param name="format">
        /// The format (or extension) of the video.
        /// </param>
        public Video (int pos, string location, int quality, AudioFormat format)
        {
            this.Position = pos;
            this.Location = location;
            this.Quality = quality;
            this.VideoFormat = default(VideoFormat);
            this.AudioFormat = format;
            this.IsAudioFile = true;
        }
    }
    
    public enum SettingsReturnType
    {
	    /// <summary>
	    /// Returns all user settings which are held within the settings container provided.
	    /// </summary>
	    Full,
	    /// <summary>
	    /// Returns only essential user settings which must be set and grabbed for the container to work properly.
	    /// </summary>
	    Essential
    }
    
    public class Settings : INotifyPropertyChanged
    {
        private bool schedulingEnabled;
        private bool continueOnFail;
        private IEnumerable<string> saveLocations = new Collection<string> 
        { 
        	string.Format(CultureInfo.InvariantCulture, "{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Finished Downloads\\"), 
        	string.Format(CultureInfo.InvariantCulture, "{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Temp\\") 
        };
        private ObservableCollection<string> validationLocations = new ObservableCollection<string>(new List<string>());
        private IEnumerable<string> schedulingTimes = new Collection<string> 
        {
            DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture),
            DateTime.Now.AddMinutes(1).ToString("hh:mm:ss tt", CultureInfo.InvariantCulture)
        };
        private const string scheduling = "Schedule Downloads";
        private const string schedualStart = "Schedule Time Start";
        private const string schedualEnd = "Schedule Time End";
        private const string mainDownloadLocation = "Download Location";
        private const string temporaryDownloadLocation = "Temporary Download Location";
        private const string validationDirectory = "Directory To Validate";
        private const string continueDownloadOnFail = "Force Continue Downloading";
        
        public const string QueuePositionRegEntry = "Queue: Position Tag Width";
        public const string QueueLocationRegEntry = "Queue: Location Tag Width";
        public const string QueueQualityRegEntry = "Queue: Quality Tag Width";
        public const string QueueFormatRegEntry = "Queue: Format Tag Width";
        public const string QueueIsAudioRegEntry = "Queue: IsAudio Tag Width";
        
        private Collection<int> queueTagWidths = new Collection<int>
        {
        	20,
        	393,
        	80,
        	55,
        	90
        };
        
        public void ResetTagWidthsToDefault()
        {
        	IEnumerable<int> defaultTagWidth = new Collection<int>
	        {
	        	20,
	        	393,
	        	80,
	        	55,
	        	90
        	}.AsEnumerable();
        	
			for (int position = 0, queueTagWidthsCount = queueTagWidths.Count; position < queueTagWidthsCount; position++)
			{
				queueTagWidths[position] = defaultTagWidth.ElementAt(position);
			}
        }
        
        /// <summary>
        /// The length of the UI queue position "tag".
        /// </summary>
        public int QueuePositionTagWidth 
        { 
        	get
        	{
        		return this.queueTagWidths.ElementAtOrDefault(0);
        	}
        	set
        	{
        		this.queueTagWidths[0] = value;
        	}
        }
        
        /// <summary>
        /// The length of the UI queue location "tag".
        /// </summary>
        public int QueueLocationTagWidth 
        { 
        	get
        	{
        		return this.queueTagWidths.ElementAtOrDefault(1);
        	}
        	set
        	{
        		this.queueTagWidths[1] = value;
        	}
        }
        
        /// <summary>
        /// The length of the UI queue quality "tag".
        /// </summary>
        public int QueueQualityTagWidth 
        { 
        	get
        	{
        		return this.queueTagWidths.ElementAtOrDefault(2);
        	}
        	set
        	{
        		this.queueTagWidths[2] = value;
        	}
        }
        
        /// <summary>
        /// The length of the UI format quality "tag".
        /// </summary>
        public int QueueFormatTagWidth 
        { 
        	get
        	{
        		return this.queueTagWidths.ElementAtOrDefault(3);
        	}
        	set
        	{
        		this.queueTagWidths[3] = value;
        	}
        }
        
        /// <summary>
        /// The length of the UI format quality "tag".
        /// </summary>
        public int QueueIsAudioTagWidth 
        { 
        	get
        	{
        		return this.queueTagWidths.ElementAtOrDefault(4);
        	}
        	set
        	{
        		this.queueTagWidths[4] = value;
        	}
        }

        /// <summary>
        /// The value indicating whether scheduling within the program is enabled.
        /// </summary>
        /// <description>
        /// The user has enabled scheduling.
        /// </description>
        public bool Scheduling
        { 
            get
            {
                return this.schedulingEnabled;
            } 
            set
            {
                this.schedulingEnabled = value;
                RaisePropertyChanged("Scheduling");
            } 
        }
        
        /// <summary>
        /// The value indicating whether, during download, the downloader will quit or continue on an error or other fatal condition.
        /// </summary>
        /// <description>
        /// The user has disabled stopping the downloading process upon failing.
        /// </description>
        public bool ContinueOnFail
        {
        	get
        	{
        		return this.continueOnFail;
        	}
        	set
        	{
        		this.continueOnFail = value;
        		RaisePropertyChanged("ContinueOnFail");
        	}
        }

        /// <summary>
        /// The save directory where finished files will be moved to.
        /// </summary>
        public string MainSaveLocation
        { 
            get
            {
            	return this.saveLocations.ElementAtOrDefault(0);
            } 
            set
            {
            	this.saveLocations = new Collection<string>{ value, this.saveLocations.ElementAt(1) };
                RaisePropertyChanged("MainSaveLocation");
            } 
        }

        /// <summary>
        /// The save directory where files will be downloaded to.
        /// </summary>
        public string TemporarySaveLocation
        { 
            get
            {
                return this.saveLocations.ElementAtOrDefault(1);
            } 
            set
            {
            	this.saveLocations = new Collection<string>{ this.saveLocations.ElementAt(0), value };
                RaisePropertyChanged("TemporarySaveLocation");
            } 
        }
        
        public ObservableCollection<string> ValidationLocations
        {
        	get
        	{
        		return this.validationLocations;
        	}
        	set
        	{
        		IOFunc.DeleteRegistrySubkey(Storage.RegistryRoot, App.IsDebugging);
        		this.validationLocations = value;
        		RaisePropertyChanged("ValidationLocations");
        	}
        }

        /// <summary>
        /// An array containing the scheduling times.
        /// </summary>
        public IEnumerable<string> Schedule
        { 
            get { return this.schedulingTimes; }
            //TO-DO: IMPLIMENT SCHEDULING.
        }
        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        #region Methods

        private void RaisePropertyChanged (string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs (propertyName));
            }
        }
        #endregion
        #region Collection Functions and Overloads
        /// <summary>
        /// Convert the current settings into an enumerable.
        /// </summary>
        /// <returns>
        /// Returns a collection of objects which represent the individual user-settings in a format suitable for use in the registry.
        /// </returns>
        public IEnumerable<RegistryEntry> AsEnumerable (SettingsReturnType returnType)
        {
			var list = new List<RegistryEntry>();
			list.Add(new RegistryEntry(scheduling, this.Scheduling));
			list.Add(new RegistryEntry(continueDownloadOnFail, this.ContinueOnFail));
			list.Add(new RegistryEntry(mainDownloadLocation, this.MainSaveLocation));
			list.Add(new RegistryEntry(temporaryDownloadLocation, this.TemporarySaveLocation));
			list.Add(new RegistryEntry(schedualStart, this.Schedule.ElementAtOrDefault(0)));
			list.Add(new RegistryEntry(schedualEnd, this.Schedule.ElementAtOrDefault(1)));
			if(returnType.Equals(SettingsReturnType.Full))
			{
				list.Add(new RegistryEntry(QueuePositionRegEntry, this.QueuePositionTagWidth));
				list.Add(new RegistryEntry(QueueLocationRegEntry, this.QueueLocationTagWidth));
				list.Add(new RegistryEntry(QueueQualityRegEntry, this.QueueQualityTagWidth));
				list.Add(new RegistryEntry(QueueFormatRegEntry, this.QueueFormatTagWidth));
				list.Add(new RegistryEntry(QueueIsAudioRegEntry, this.QueueIsAudioTagWidth));
			}
            var returnValue = list;
            for (var position = ValidationLocations.GetEnumerator(); position.MoveNext();)
			{
				string directory = position.Current;
				returnValue.Add(new RegistryEntry(string.Format(CultureInfo.CurrentCulture, "{0}:{1}", validationDirectory, ValidationLocations.IndexOf(directory)), directory));
			}
            return returnValue.AsEnumerable();
        }
        
        /// <summary>
        /// Replaces these settings with a collection of objects with comperable entries.
        /// </summary>
        /// <param name="newSettings">
        /// The object whose values you will replace these settings with.
        /// </param>
        /// <returns>
        /// Returns this settings container with the values of the provided object.
        /// </returns>
        /// <exception cref="T:YoutubeDownloadHelper.ParsingException">
        /// Thrown if even a single value in the provided object does not contain a "Settings definition"; that is, it cannot be parsed.
        /// </exception>
        public Settings Replace (IEnumerable<RegistryEntry> newSettings)
        {
        	return this.Replace(newSettings.GetEnumerator());
        }

        /// <summary>
        /// Replaces these settings with a collection of objects with comperable entries.
        /// </summary>
        /// <param name="newSettings">
        /// The object whose values you will replace these settings with.
        /// </param>
        /// <returns>
        /// Returns this settings container with the values of the provided object.
        /// </returns>
        /// <exception cref="T:YoutubeDownloadHelper.ParsingException">
        /// Thrown if even a single value in the provided object does not contain a "Settings definition"; that is, it cannot be parsed.
        /// </exception>
        public Settings Replace (IEnumerator<RegistryEntry> newSettings)
        {
            for (var position = newSettings; position.MoveNext();)
            {
            	var currentItem = position.Current;
                string name = currentItem.Name;
                var valueAsString = currentItem.Value.ToString();
                if (!string.IsNullOrEmpty(valueAsString))
                {
                	if (name.Contains(validationDirectory, StringComparison.OrdinalIgnoreCase))
	                {
                		this.ValidationLocations.Add(valueAsString);
	                }
                	else
                	{
	                    switch (name)
	                    {
	                        case scheduling:
	                            this.Scheduling = bool.Parse(valueAsString);
	                            break;
	                        case mainDownloadLocation:
	                            this.MainSaveLocation = valueAsString;
	                            break;
	                        case temporaryDownloadLocation:
	                            this.TemporarySaveLocation = valueAsString;
	                            break;
	                        case schedualStart:
	                            this.schedulingTimes = new Collection<string>{ valueAsString, this.Schedule.ElementAtOrDefault(1) };
	                            break;
	                        case schedualEnd:
	                            this.schedulingTimes = new Collection<string>{ this.Schedule.ElementAtOrDefault(0), valueAsString };
	                            break;
	                        case continueDownloadOnFail:
	                            this.ContinueOnFail = bool.Parse(valueAsString);
	                            break;
	                        case QueuePositionRegEntry:
	                            this.QueuePositionTagWidth = int.Parse(valueAsString, CultureInfo.InvariantCulture);
	                            break;
	                        case QueueLocationRegEntry:
	                            this.QueueLocationTagWidth = int.Parse(valueAsString, CultureInfo.InvariantCulture);
	                            break;
	                        case QueueQualityRegEntry:
	                            this.QueueQualityTagWidth = int.Parse(valueAsString, CultureInfo.InvariantCulture);
	                            break;
	                        case QueueFormatRegEntry:
	                            this.QueueFormatTagWidth = int.Parse(valueAsString, CultureInfo.InvariantCulture);
	                            break;
	                        case QueueIsAudioRegEntry:
	                            this.QueueIsAudioTagWidth = int.Parse(valueAsString, CultureInfo.InvariantCulture);
	                            break;
	                        default:
	                            throw new ParsingException (string.Format(CultureInfo.CurrentCulture, "'{0}' could not be assimilated into the current instance of settings because no value in settings contain '{0}'", name));
	                    }
                	}
                }
            }
            return this;
        }
        #endregion
    }

    public class ClassContainer
    {
    	/// <summary>
    	/// Pre-Baked Exceptions class.
    	/// </summary>
    	public IError BakedExceptionCode { get; private set; }
    	
    	/// <summary>
    	/// Storage class.
    	/// </summary>
    	public IStorage IOCode { get; private set; }
    	
        /// <summary>
        /// Download class.
        /// </summary>
        public IDownload DownloadingCode { get; private set; }

    	
        /// <summary>
        /// Backend container of classes to make the programmer's life a little easier.
        /// </summary>
        public ClassContainer ()
        {
            this.IOCode = new Storage();
            this.DownloadingCode = new Download (this.IOCode);
            this.BakedExceptionCode = new PrebakedError ();
        }
    }

    /// <summary>
    /// Originating party category.
    /// </summary>
    public enum Party
    {
        /// <summary>
        /// First Party.
        /// </summary>
        First,
        /// <summary>
        /// Second Party.
        /// </summary>
        Second,
        /// <summary>
        /// Third Party.
        /// </summary>
        Third
    }

    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
    public class ProjectAssemblies
    {
        private readonly ICollection<Library> assemblyStore = new Collection<Library> ();
        /// <summary>
        /// All assemblies used in the project.
        /// </summary>
        /// <remarks>
        ///  Excludes the project assembly itself and all system assemblies.
        /// </remarks>
        public IEnumerable<Library> Assemblies { get { return assemblyStore.OrderBy(o => o.Name); } }
        
        private bool isSafeToRecall;
        /// <summary>
        /// If true, this container is safe to discard when finished, and recall again when needed.
        /// </summary>
        public bool RecallIsSafe { get { return this.isSafeToRecall; } }
        
        /// <summary>
        /// A container for the external assemblies referenced in the project.
        /// </summary>
        /// <param name="doNotStoreAssemblyReferences">
        /// Do not store any of the read libraries as an assembly reference.
        /// </param>
        /// <exception cref="T:YoutubeDownloadHelper.ParsingException">
        /// Thrown during the initialization process of this class if some portion fails.
        /// </exception>
        public ProjectAssemblies (bool doNotStoreAssemblyReferences)
        {
        	var AssembliesToFind = new List<string> 
        	{
        		"youtubeextractor",
        		"xceed.wpf.toolkit",
        		"universalhandlerslibrary",
        		"newtonsoft.json",
        		"microsoft.windowsapicodepack.shell",
        		"microsoft.windowsapicodepack",
        		"costura.fody",
        		"fody",
        		"htmlagilitypack"
        	};
			var currentAssembly = Assembly.GetExecutingAssembly();
			
			var referencedAssemblies = currentAssembly.GetReferencedAssemblies().Where(n => AssembliesToFind.Any(subN => n.Name.Equals(subN, StringComparison.OrdinalIgnoreCase))).GetEnumerator();
			for (var position = referencedAssemblies; position.MoveNext();)
			{
				AssemblyName assembly = position.Current;
				AssembliesToFind.RemoveAll(n => assembly.Name.Equals(n, StringComparison.OrdinalIgnoreCase));
				
				assemblyStore.Add(new Library(assembly));
				if(doNotStoreAssemblyReferences) assemblyStore.Last().ReleaseAssembly();
			}
			int numberOfAssemblies = assemblyStore.Count;
			
			if(AssembliesToFind.Any())
			{
				var embeddedAssemblies = currentAssembly.GetManifestResourceNames().Where(n => n.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).GetEnumerator();
	            for (var position = embeddedAssemblies; position.MoveNext();)
	            {
	                string resource = position.Current;
	                using (var stream = currentAssembly.GetManifestResourceStream(resource))
	                {
	                    if (stream == null) continue;
	                    try
	                    {
	                        var bytes = new byte[stream.Length];
	                        stream.Read(bytes, 0, bytes.Length);
							var assembly = Assembly.ReflectionOnlyLoad(bytes).GetName();
							if(AssembliesToFind.Any(n => assembly.Name.Contains(n, StringComparison.OrdinalIgnoreCase)))
							{
		                        assemblyStore.Add(new Library(assembly));
		                        if(doNotStoreAssemblyReferences) assemblyStore.Last().ReleaseAssembly();
							}
	                    }
	                    catch (Exception ex)
	                    {
							Debug.Print(string.Format(CultureInfo.CurrentCulture, "Failed to load: {0}, Exception: {1}", resource, ex.Message));
	                        throw new ParsingException (string.Format(CultureInfo.CurrentCulture, "While pulling the libraries from the program for use in the frontend, an error caused the process to stop. ({0})", ex.Message), ex);
	                    }
	                    stream.Close();
	                }
	            }
	            if(numberOfAssemblies >= assemblyStore.Count) this.isSafeToRecall = true;
			}
			else this.isSafeToRecall = true;
        }

        /// <summary>
        /// Generates a table-like string of assembly's names and versions.
        /// </summary>
        /// <param name="partyToGet">
        /// With whom the libraries you are seeking originated from.
        /// </param>
        /// <returns>
        /// Returns a list of assembly names and versions.
        /// </returns>
        public string ToString (Party partyToGet)
        {
        	var librariesToCheck = new List<string> { "UniversalHandlersLibrary" }.AsReadOnly();
            string returnString = string.Empty;
            var compiledParties = this.Assemblies.Where(item => partyToGet == Party.Third ? !librariesToCheck.Any(checkedItem => item.Name.Contains(checkedItem, StringComparison.OrdinalIgnoreCase)) : librariesToCheck.Any(checkedItem => item.Name.Contains(checkedItem, StringComparison.OrdinalIgnoreCase)));
            for (var position = compiledParties.GetEnumerator(); position.MoveNext();)
            {
                var currentLibrary = position.Current;
                returnString += string.Format(CultureInfo.CurrentCulture, "{0}: V. {1}", currentLibrary.Name, currentLibrary.Version);
				if (currentLibrary != compiledParties.Last()) returnString += "\n";
            }
            return returnString;
        }
    }

    public class Library
    {
    	private AssemblyName assembly = new AssemblyName();
    	/// <summary>
    	/// The actual assembly this library is based on.
    	/// </summary>
    	/// <description>
    	/// This is held only because a call to grab this assembly again would throw an exception. So, as a precaution, the actual assembly file is held internally.
    	/// </description>
    	/// <remarks>
    	/// This is NOT to be used regularly. This is mainly if a functionality has not yet been implimented or you need something very specific from the actual assembly rarely. Otherwise all functions should use other members, and not this one.
    	/// </remarks>
    	public AssemblyName Assembly 
    	{ 
    		get
			{
				if (this.assembly == null)
				{
					FatalException thrownException = new FatalException("The assembly reference you are attempting to access has been nullified or was never initiated.");
					(new ClassContainer()).BakedExceptionCode.Alert(thrownException);
					throw thrownException;
				}
				return this.assembly;
			}
    	}
    	
        /// <summary>
        /// The name of the referenced Assembly.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The version of the referenced Assembly.
        /// </summary>
        public Version Version { get; private set; }

        /// <summary>
        /// Container for an Assembly file.
        /// </summary>
        /// <param name="file">
        /// The Assembly reference.
        /// </param>
        public Library (AssemblyName file)
        {
            this.assembly = file;
            this.Name = this.Assembly.Name;
            this.Version = this.Assembly.Version;
        }
        
        public void Modify (string name, Version version)
        {
        	this.Name = name;
        	this.Version = version;
        }
        
        /// <summary>
        /// Use this to manually free up resources by nullifying the held assembly.
        /// </summary>
        public void ReleaseAssembly()
        {
        	this.assembly = null;
        }
        
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", this.Name, this.Version);
		}
    }
}
