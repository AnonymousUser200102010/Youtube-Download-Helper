using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace UniversalFormsHandlerLibrary
{
	
	internal static class strawMan
	{
		
		internal static void Main()
		{
			
			//REQUIRED, BUT YOU DO NOT NEED TO PUT ANY CODE. IT JUST NEEDS TO BE HERE.
			
		}
		
	}
	
	public partial class MainForm : Form
	{
		
		public MainForm ()
        {
        	
            //new Form();
            
            Console.WriteLine("This has been a test of the UFHLib's form functionality.");
            
		}
		
	}
	
	public static class BackEnd
	{
		
		/// <summary>
    	/// A way to check for required parameters you generally always use before the start of a program.
    	/// </summary>
    	/// <param name="applicationName">
    	/// Name of the parent program.
    	/// </param>
    	/// <param name="debugMode">
    	/// Is the parent program debugging?
    	/// </param>
    	/// <param name="formToLoad">
    	/// The form you wish to load if all required parameters are met.
    	/// </param>
    	public static void CheckBeginningParameters(string applicationName, Form formToLoad, bool debugMode)
		{
    		
			if (Process.GetProcessesByName(applicationName).Count() <= 1 || debugMode)
			{
				
				Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(formToLoad);
				
			}
			else
			{
				
				MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "{0} is already running!", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).InternalName), "Application Failed to Launch");
            	
            	Environment.Exit(0);
				
			}
            
		}
		
	}
	
}
