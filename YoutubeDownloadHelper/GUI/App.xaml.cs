using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main (string[] args)
        {
			
            bool debug = false;
			
            #if DEBUG
			
            debug = true;
			
            #endif
			
            HandleArgs(args);
            
            if(UniversalHandlersLibrary.BackEnd.CheckBeginningParameters(Path.GetFileNameWithoutExtension(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).InternalName), debug))
            {
            	
            	Application app = new Application();
            	app.Run(new YoutubeDownloadHelper.Gui.MainWindow(downloadImmediately));
            	
            }
            
			
        }

        private static void HandleArgs (string[] args)
        {
			
            foreach (string arg in args)
            {
				
                if (arg.Contains("start"))
                {
					
                    Console.WriteLine(arg);
					
                    downloadImmediately = true;
					
                }
				
            }
			
        }
		
	}
}