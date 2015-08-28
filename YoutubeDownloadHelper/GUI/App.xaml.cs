using System;
using System.Linq;
using System.Windows;

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
        		var debug = false;
        		#if DEBUG
	            debug = true;
	            #endif
	            return debug;
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
            
            if (UniversalHandlersLibrary.BackEnd.CheckBeginningParameters(programName, IsDebugging))
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
                if (UniversalHandlersLibrary.GlobalFunctions.Contains(arg, "start", StringComparison.OrdinalIgnoreCase))
                {
                    downloadImmediately = true;
                }
            }
        }
    }
}