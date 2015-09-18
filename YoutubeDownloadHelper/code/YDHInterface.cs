using System;
using YoutubeDownloadHelper.Code;

namespace YoutubeDownloadHelper.Code
{
    public interface IDownload
    {
        /// <summary>
        /// The entry-point for the video downloading process.
        /// </summary>
        /// <description>
        /// Initiates the downloading of all videos currently queued by the user and/or their url list.
        /// </description>
        /// <param name="mainWindow">
        /// The window this process will interact with.
        /// </param>
        /// <remarks>
        /// Usually this is the main application window.
        /// </remarks>
        void DownloadHandler (YoutubeDownloadHelper.Gui.MainProgramElements mainWindow);
    }
    
    public interface IStorage : IRegEdit {}
    
    public interface IRegEdit
    {
    	/// <summary>
        /// Reads the registry.
        /// </summary>
        /// <param name="settings">
        /// The container to write the registry values found to.
        /// </param>
        /// <returns>
        /// Returns a settings file containing usable values from the registry.
        /// </returns>
        Settings RegistryRead (Settings settings);

        /// <summary>
        /// Writes to the registry.
        /// </summary>
        /// <param name="settings">
        /// Values to write to the registry.
        /// </param>
        void RegistryWrite (Settings settings);
    }
    
    public interface IError
    {
    	void Alert(Exception ex);
    }
}