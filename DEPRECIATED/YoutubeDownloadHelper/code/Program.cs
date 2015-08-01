using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace YoutubeDownloadHelper
{

    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {

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
			
            handleArgs(args);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            UniversalFormsHandlerLibrary.BackEnd.CheckBeginningParameters(Path.GetFileNameWithoutExtension(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).InternalName), new MainForm(downloadImmediately), debug);
            
			
        }
        
        private static bool downloadImmediately;

        private static void handleArgs (string[] args)
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
