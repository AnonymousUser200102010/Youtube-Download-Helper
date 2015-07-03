using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using UniversalHandlersLibrary;

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
		private static void Main(string[] args)
		{
			
			bool debug = false;
			
			#if DEBUG
			
			debug = true;
			
			#endif
			
			if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).InternalName)).Count() <= 1 || debug)
			{
				
				#if DEBUG
				//BackEnd.SetupConsole(true);
				#endif
				
				handleArgs(args);
				
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
				
			}
			else
			{
				
				MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "{0} is already running!", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).InternalName), "Application Failed to Launch");
				
				Environment.Exit(0);
				
			}
			
		}
		
		private static void handleArgs(string[] args)
		{
			
			foreach(string arg in args)
			{
				
				if(arg.Contains("start"))
				{
					
					Console.WriteLine(arg);
					
					GlobalVariables.DownloadImmediately = true;
					
				}
				
			}
			
		}
		
	}
}
