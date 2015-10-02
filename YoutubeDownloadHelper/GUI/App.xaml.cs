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
        public static readonly string Name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

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
        public static readonly bool IsWindowsMachine = Environment.OSVersion.Platform.ToString().StartsWith("win", StringComparison.OrdinalIgnoreCase);

        private static YoutubeDownloadHelper.Code.EnumeratorDictionaries backingEnumDictionaries;
        
        /// <summary>
        /// A dictionary of enumerator values used within this program.
        /// </summary>
        public static YoutubeDownloadHelper.Code.EnumeratorDictionaries EnumDictionaries { get { return backingEnumDictionaries; } }

        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main (string[] args)
        {
            if (BackEnd.CheckBeginningParameters(App.Name, IsDebugging))
            {
            	backingEnumDictionaries = new YoutubeDownloadHelper.Code.EnumeratorDictionaries();
            	HandleArgs(args.ToList().AsReadOnly());
            	
                (new Application ()).Run(new YoutubeDownloadHelper.Gui.MainWindow (downloadImmediately));
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} is already running!", App.Name), "Application Failed to Launch", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                Environment.Exit(0);
            }
        }

        private static void HandleArgs (System.Collections.ObjectModel.ReadOnlyCollection<string> args)
        {
        	for (var position = args.GetEnumerator(); position.MoveNext();)
            {
        		string arg = position.Current;
				downloadImmediately = arg.Contains("start", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}