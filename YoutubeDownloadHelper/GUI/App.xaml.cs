using System;
using System.Linq;
using System.Windows;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool downloadImmediately;

        /// <summary>
        /// The (whole) application is currently debugging.
        /// </summary>
        public static bool IsDebugging 
        { 
        	get
        	{
        		#if DEBUG
	            return true;
	            #else
	            return false;
	            #endif
        	} 
        }
        
        /// <summary>
        /// The current operating system is a version of Windows.
        /// </summary>
        public static bool IsWindowsMachine
        {
        	get
        	{
        		return Environment.OSVersion.Platform.ToString().Contains("win", StringComparison.OrdinalIgnoreCase);
        	}
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main (string[] args)
        {
            HandleArgs(args);
            string programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            
            if (BackEnd.CheckBeginningParameters(programName, IsDebugging))
            {
                (new Application ()).Run(new YoutubeDownloadHelper.Gui.MainWindow (downloadImmediately));
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} is already running!", programName), "Application Failed to Launch", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }
        }

        private static void HandleArgs (string[] args)
        {
            for (int position = 0, argsLength = args.Length; position < argsLength; position++)
            {
                string arg = args[position];
				if (arg.Contains("start", StringComparison.OrdinalIgnoreCase))
				{
					downloadImmediately = true;
				}
            }
        }
    }
}