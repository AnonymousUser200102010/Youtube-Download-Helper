using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using Microsoft.Win32;

namespace UniversalHandlersLibrary
{

    #region Global Custom Items
    /// <summary>
    /// A registry value.
    /// </summary>
    public class RegistryEntry
    {
        /// <summary>
        /// The name of the entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The entry's value
        /// </summary>
        public object Value { get; set; }
        
        /// <summary>
        /// The separator used when converting this value into a string. Can also be used to split lines read from a settings file.
        /// </summary>
        public static string Separator
        {
        	get
        	{
        		return " = ";
        	}
        }
        
		///
		///      <summary>Returns a string that represents the current object.</summary>
		///      <returns>A string that represents the current object.</returns>
		///      <filterpriority>2</filterpriority>
		///
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture ,"{0}{1}{2}", Name, Separator, Value);
		}

        /// <summary>
        /// A registry value.
        /// </summary>
        /// <param name="key">
        /// The registry value's key.
        /// </param>
        /// <param name="value">
        /// The registry value's actual held value.
        /// </param>
        public RegistryEntry (string key, object value)
        {
            this.Name = key;
            this.Value = value;
        }
    }

    /// <summary>
    /// Contains all internal parameters whose values are and must remain constant.
    /// </summary>
    internal struct InternalParamaters
    {
        internal const int minPruneVal = 25000;

        internal const int maxPruneVal = 26214400;
    }

    /// <summary>
    /// Conditionary statements for statistics-related functions.
    /// </summary>
    public enum StatisticCondition
    {
        /// <summary>
        /// Use no conditions.
        /// </summary>
        None,
        /// <summary>
        /// Return only the sum of this function's internal arithmetic.
        /// </summary>
        SumOf,
        /// <summary>
        /// Return this function's numeric value as milliseconds (multiplied by 1000).
        /// </summary>
        ReturnInMilliseconds
    }

    /// <summary>
    /// Generic conditionary statements fit for multiple uses and purposes.
    /// </summary>
    public enum GenericCondition
    {
        /// <summary>
        /// Use no conditions.
        /// </summary>
        None,
        /// <summary>
        /// When performing check operations, swap the normal position of the checked items to be in reverse.
        /// </summary>
        ReverseCheck,
        /// <summary>
        /// When performing check operations, use an equals operator instead of a contains operator.
        /// </summary>
        CheckExactly,
        /// <summary>
        /// When running this function, throw any and all built-in exceptions.
        /// </summary>
        ThrowExceptions,
        /// <summary>
        /// For functions with pruning capability, this tells the function to prune.
        /// </summary>
        Prune
    }

    /// <summary>
    /// Performance classifications. These are intended to be used internally (and then by you IN A FUNCTION HOUSED BY THIS LIBRARY). However this is not a requirement.
    /// </summary>
    public enum PerformanceClass
    {
        /// <summary>
        /// CPU performance values.
        /// </summary>
        CpuUsage,
        /// <summary>
        /// Ram performance values.
        /// </summary>
        MemoryUsage,
        /// <summary>
        /// Overall temperature of the machine.
        /// </summary>
        OverallTemperature
    }

    /// <summary>
    /// A set of variables related to the default folder tree.
    /// </summary>
    public static class GenericFolderOptions
    {
        /// <summary>
        /// The base folder used for most folder trees.
        /// </summary>
        public const string BaseFolderRoot = "Files";

        /// <summary>
        /// The old folder tree structure.
        /// </summary>
        public static IEnumerable<string> OldStyleFileTree
        {
            get { return oldStyleFileTree.AsEnumerable(); }
        }
        private static readonly List<string> oldStyleFileTree = new List<string> {
            "Text", "Backup", "Help", "Assets" };
       	
       	/// <summary>
       	/// The current folder tree structure.
       	/// </summary>
       	public static IEnumerable<string> NewStyleFileTree 
       	{ 
       		get { return newStyleFileTree.AsEnumerable(); }
       	}
       	private static readonly List<string> newStyleFileTree = new List<string> { "Text", "Text\\Help", "Backups", "Backups\\Assets" };
    }
    #endregion

    /// <summary>
    /// Global Functions
    /// </summary>
    public static class GlobalFunctions
    {
    	/// <summary>
    	/// Counts all real elements in a collection of items.
    	/// </summary>
    	/// <param name="itemToCount">
    	/// The item you wish to get the number of total items from.
    	/// </param>
    	/// <returns>
    	/// if itemToCount.Count() - 1 >= 0, returns itemToCount.Count() - 1; Else returns 0.
    	/// </returns>
    	public static int All<T>(this IEnumerable<T> itemToCount)
    	{
    		int allCount = itemToCount.Count() - 1;
    		return allCount >= 0 ? allCount : 0;
    	}
    	
        /// <summary>
        /// Returns a value indicating if the provided String object occurs within the string.
        /// </summary>
        /// <param name="source">
        /// The string to check.
        /// </param>
        /// <param name="value">
        /// The keyword to seek.
        /// </param>
        /// <param name="comp">
        /// One of the enumeration values that specifies the rules for the search.
        /// </param>
        /// <returns>
        /// true if the value occurs within the source, or if the value is empty; else false.
        /// </returns>
        public static bool Contains (this string source, string value, StringComparison comp)
        {
            return source.IndexOf(value, comp) >= 0;
        }
        
        /// <summary>
        /// Replaces one Collection with another.
        /// </summary>
        /// <param name="old">
        /// The original collection.
        /// </param>
        /// <param name="new">
        /// The collection to replace the original with.
        /// </param>
        public static void Replace<T> (this ICollection<T> old, IEnumerable<T> @new)
        {
        	old.Replace(@new.GetEnumerator());
        }

        /// <summary>
        /// Replaces one Collection with another.
        /// </summary>
        /// <param name="old">
        /// The original collection.
        /// </param>
        /// <param name="new">
        /// The collection to replace the original with.
        /// </param>
        public static void Replace<T> (this ICollection<T> old, IEnumerator<T> @new)
        {
            old.Clear();
            for (var position = @new; position.MoveNext();)
			{
				var item = position.Current;
				old.Add(item);
			}
        }
        
        /// <summary>
        /// Checks if a string is within the given parameters.
        /// </summary>
        /// <description>
        /// Checks to see if a string is within the parameters provided and, if not, trims the string to the selected length and adds a [...] to the end of it.
        /// </description>
        /// <param name="value">
        /// The object whose length you want to check.
        /// </param>
        /// <param name="truncationCutoff">
        /// The maximum length before the string is truncated.
        /// </param>
        /// <returns>
        /// Returns the full string if it is less than or equal to the provided truncation cutoff. If not it returns the same string, pruned to the the truncation cutoff length, with a singular '[...]' added to the end.
        /// </returns>
        public static string Truncate (this string value, int truncationCutoff)
        {
            return value.Length <= truncationCutoff ? value : string.Format(CultureInfo.InstalledUICulture, "{0}[...]", value.Substring(0, truncationCutoff)).ToLower(CultureInfo.InstalledUICulture);
        }
        #region Find Function and Overloads
        /// <summary>
        /// Find a desired value in a collection of objects.
        /// </summary>
        /// <param name="source">
        /// The object you're searching.
        /// </param>
        /// <param name="value">
        /// The value you wish to find.
        /// </param>
        /// <returns>
        /// If the value exists in the collection, returns that value. Else, returns the default value.
        /// </returns>
		public static T Find<T>(this IEnumerable<T> source, T value)
		{
			return source.Find(value, 0);
		}
		
		/// <summary>
        /// Find a desired value in a collection of objects.
        /// </summary>
        /// <param name="source">
        /// The object you're searching.
        /// </param>
        /// <param name="value">
        /// The value you wish to find.
        /// </param>
        /// <param name="offset">
        /// The number of items through the collection you'd like to skip to begin with.
        /// </param>
        /// <remarks>
        /// Having an offset greater than the first value that fits the value you're searching for does not mean that the first value will not be returned.
        /// </remarks>
        /// <returns>
        /// If the value exists in the collection, returns that value after offset. Else, returns the default value.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
		/// Thrown if you pass an offset less than 0 or greater than the amount of real values in the provided collection.
		/// </exception>
		public static T Find<T>(this IEnumerable<T> source, T value, int offset)
		{
			return source.Find(value, offset, (new List<GenericCondition>{ GenericCondition.ThrowExceptions }).AsEnumerable());
		}
		
		/// <summary>
        /// Find a desired value in a collection of objects.
        /// </summary>
        /// <param name="source">
        /// The object you're searching.
        /// </param>
        /// <param name="value">
        /// The value you wish to find.
        /// </param>
        /// <param name="offset">
        /// The number of items through the collection you'd like to skip to begin with.
        /// </param>
        /// <remarks>
        /// Having an offset greater than the first value that fits the value you're searching for does not mean that the first value will not be returned.
        /// </remarks>
        /// <param name="conditions">
        /// A list of conditions to be used within the function.
        /// </param>
        /// <returns>
        /// If the value exists in the collection, returns that value after offset. Otherwise it returns the default value.
        /// </returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// Thrown if you pass an offset less than 0 or greater than the amount of real values in the provided collection.
		/// </exception>
		public static T Find<T> (this IEnumerable<T> source, T value, int offset, IEnumerable<GenericCondition> conditions)
		{
			InternalFunction.CheckParameters(conditions, new List<GenericCondition>{ GenericCondition.CheckExactly, GenericCondition.ReverseCheck, GenericCondition.ThrowExceptions });
			if ((offset > source.All() || offset < 0) && conditions.Any(item => item.Equals(GenericCondition.ThrowExceptions)))
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "When trying to find {0}, you attempted to pass an invalid offset", value));
        	}
        	bool exactSearch = conditions.Any(item => item.Equals(GenericCondition.CheckExactly));
			try
			{
				return source.First(searchResult => 
	        		(
					    conditions.Any(item => item.Equals(GenericCondition.ReverseCheck)) 
				        ? 
				        	(!exactSearch ? value.ToString().Contains(searchResult.ToString(), StringComparison.OrdinalIgnoreCase) : value.ToString().Equals(searchResult.ToString(), StringComparison.OrdinalIgnoreCase))
				        : 
				        	(!exactSearch ? searchResult.ToString().Contains(value.ToString(), StringComparison.OrdinalIgnoreCase) : searchResult.ToString().Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
					)
					&&
					(
						source.ToList().IndexOf(searchResult) > offset || (source.ToList().IndexOf(searchResult) == 0 && offset == 0) && source.ToList().IndexOf(searchResult) > -1
					)
				);
			}
        	catch (InvalidOperationException) {}
			return offset > 0 ? Find(source, value, 0, conditions) : default(T);
		}
		#endregion
		
        private static bool timedout;
        private static float maxTime;
        private static float elapsedTime;
        private static Timer aTimer = new Timer ();

        /// <summary>
        /// Acts like the Windows CommandLine Timeout command.
        /// </summary>
        /// <param name="timeoutCount">
        /// The time to count to.
        /// </param>
        /// <param name="increment">
        /// The amount of time to for each tick.
        /// </param>
        public static void Timeout (float timeoutCount, float increment)
        {
            maxTime = timeoutCount;
            aTimer.Enabled = false;
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Waiting {0} seconds. Press any key to continue.", timeoutCount));
            
            aTimer = new Timer ();		
            aTimer.Interval = increment * 1000;			
            aTimer.Elapsed += OnTimedEvent;			
            aTimer.Start();
            
            while (!Console.KeyAvailable && !timedout)
            {
                System.Threading.Thread.Sleep(100);
            }
            timedout = true;	
        }

        private static void OnTimedEvent (object sender, ElapsedEventArgs e)
        {
            if (elapsedTime < maxTime && !timedout)
            {
                elapsedTime += (float)aTimer.Interval / 1000;
                Console.Clear();
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Waiting {0} seconds. Press any key to continue.", maxTime - elapsedTime));	
            }
            else if (!timedout)
            {
                timedout = true;
            }
            else
            {
                aTimer.Stop();
                aTimer.Close();
                aTimer.Dispose();
            }
        }
    }

    /// <summary>
    /// Error handler entry point.
    /// </summary>
    public static class ErrorFunc
    {
    	#region Handler Function and Overloads
		/// <summary>
		/// Handles errors.
		/// </summary>
		/// <param name="providedException">
		/// The exception that will be handled.
		/// </param>
		public static void Log (this Exception providedException)
		{
			providedException.Log((new List<GenericCondition>{ GenericCondition.Prune, GenericCondition.ThrowExceptions }).AsEnumerable());
		}
		
		/// <summary>
		/// Handles errors.
		/// </summary>
		/// <param name="providedException">
		/// The exception that will be handled.
		/// </param>
		/// <param name="condition">
		/// The condition for this function.
		/// </param>
		public static void Log (this Exception providedException, GenericCondition condition)
		{
			providedException.Log((new List<GenericCondition>{ condition }).AsEnumerable());
		}

		/// <summary>
		/// Handles errors.
		/// </summary>
		/// <param name="providedException">
		/// The exception that will be handled.
		/// </param>
		/// <param name="conditions">
		/// The conditions for this function.
		/// </param>
		public static void Log (this Exception providedException, IEnumerable<GenericCondition> conditions)
		{
			providedException.Log(conditions, 524288);
		}
		
        /// <summary>
        /// Handles errors.
        /// </summary>
        /// <param name="providedException">
        /// The exception that will be handled.
        /// </param>
        /// <param name="conditions">
        /// The conditions for this function.
        /// </param>
        /// <param name="pruneCutoff">
        /// The file size at which to prune the Error.dmp file. (default is 524288)
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Thrown if the pruneCutOff value is less than <see cref="InternalParamaters.minPruneVal"/>, 
        /// or greater than <see cref="InternalParamaters.maxPruneVal"/>.
        /// </exception>
        public static void Log (this Exception providedException, IEnumerable<GenericCondition> conditions, int pruneCutoff)
        {
        	InternalFunction.CheckParameters(conditions, new List<GenericCondition>{ GenericCondition.Prune, GenericCondition.ThrowExceptions });
        	var startingMessage = new Collection<string> 
        	{
        		string.Format(CultureInfo.InvariantCulture, "[{0}:\n\nException Message:\n\n{1}\n\nException Stack Trace:\n\n{2}{3}]\n\n", DateTime.Now.ToString("MMMM dd hh:mm:ss tt", CultureInfo.InvariantCulture), providedException.Message, providedException.StackTrace, providedException.InnerException != null ? string.Format(CultureInfo.InvariantCulture, "Inner Exception Message:\n\n{0}\n\nInner Exception Stack Trace:\n\n{1}", providedException.InnerException.Message, providedException.InnerException.StackTrace) : null)
        	};
			var filePath = InternalFunction.ReturnFilePath("Error.dmp");
			startingMessage.InternalWriter(filePath, conditions.Any(item => item.Equals(GenericCondition.Prune)), pruneCutoff);
			if (conditions.Any(item => item.Equals(GenericCondition.ThrowExceptions))) throw providedException;	
        }
        #endregion
        /// <summary>
        /// A list of premade exceptions for convenience. This does not replace custom-built exceptions, which should be used more often than premade exceptions.
        /// </summary>
        /// <param name="offendingCode">
        /// Where did the exception originate from?
        /// </param>
        /// <param name="areaWhereOffendingCodeResides">
        /// Where within the originating code did the exception occur?
        /// </param>
        /// <param name="exceptionToThrow">
        /// The exception you wish to see thrown.
        /// </param>
        /// <remarks>
        /// This function only utilizes the exception provided for it's type, so additional information in said exception (such as a message) is not required.
        /// </remarks>
        /// <returns>
        /// Returns a premade exception for that type, if one exists.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Thrown if the exceptionNumber is less than zero or greater than the current number of premade exceptions.
        /// </exception>
        public static Exception PremadeError (this Exception exceptionToThrow, string offendingCode, string areaWhereOffendingCodeResides)
		{
        	var messageToUse = string.Format(CultureInfo.InvariantCulture, "{0}: {1}: ", offendingCode, areaWhereOffendingCodeResides);
			if (exceptionToThrow is FormatException)
			{
				messageToUse += "Argument failed to produce a valid result.";
				exceptionToThrow = new FormatException(messageToUse);
			}
			else
			{
				messageToUse += "This functionality has not been implimented.";
				exceptionToThrow = new NotImplementedException(messageToUse);
			}
			return exceptionToThrow;
		}
    }

    /// <summary>
    /// Message handler entry point.
    /// </summary>
    public static class Message
    {
    	/// <summary>
        /// Message handler
        /// </summary>
        /// <param name="message">
        /// The message to be handled.
        /// </param>
        /// <param name="caller">
        /// The name you wish to be printed before the message.
        /// </param>
        public static void Log<T> (this T message, string caller)
        {
        	message.ToString().Log(caller);
        }
    	
    	/// <summary>
        /// Message handler
        /// </summary>
        /// <param name="message">
        /// The message to be handled.
        /// </param>
        /// <param name="caller">
        /// The name you wish to be printed before the message.
        /// </param>
        public static void Log (this string message, string caller)
        {
        	message.Log(caller, GenericCondition.Prune, 102400);
        }
    	
        /// <summary>
        /// Message handler
        /// </summary>
        /// <param name="message">
        /// The message to be handled.
        /// </param>
        /// <param name="caller">
        /// The name you wish to be printed before the message.
        /// </param>
        /// <param name="condition">
        /// The condition for this function.
        /// </param>
        /// <param name="pruneCutoff">
        /// The file size at which to prune the Messages.txt file. (default is 102400)
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Thrown if the pruneCutOff value is less than <see cref="InternalParamaters.minPruneVal"/>, 
        /// or greater than <see cref="InternalParamaters.maxPruneVal"/>.
        /// </exception>
        public static void Log (this string message, string caller, GenericCondition condition, int pruneCutoff)
        {
        	InternalFunction.CheckParameters(condition, GenericCondition.Prune);
        	var filePath = InternalFunction.ReturnFilePath("Messages.txt");
        	var finalMessage = new Collection<string> 
        	{
        		string.Format(CultureInfo.InvariantCulture, "{0}: {1} Time: {2}\n\n", caller, message, DateTime.Now.ToString("MM-dd-yy hh:mm tt", CultureInfo.InvariantCulture)) 
        	};
        	finalMessage.InternalWriter(filePath, condition.Equals(GenericCondition.Prune), pruneCutoff);
        }
    }

    /// <summary>
    /// Backend related functions.
    /// </summary>
    public static class BackEnd
    {
    	/// <summary>
    	/// Gets a statistics value for the currently running machine.
    	/// </summary>
    	/// <param name="classification">
    	/// The statistic classification you are seeking a value for.
    	/// </param>
    	/// <returns>
    	/// Returns a value for the currently selected classification unit.
    	/// </returns>
    	/// <remarks>
    	/// If this function has not been told to find a different value-type, the average will be found.
    	/// </remarks>
		/// <exception cref="T:System.NotImplementedException">
		/// Some classification categories have not yet been implemented. If the category you select has not been implemeneted yet, this error is thrown.
		/// </exception>
    	public static int GetStatistics(this PerformanceClass classification)
		{
    		return classification.GetStatistics(StatisticCondition.ReturnInMilliseconds);
    	}
    	/// <summary>
    	/// Gets a statistics value for the currently running machine.
    	/// </summary>
    	/// <param name="classification">
    	/// The statistic classification you are seeking a value for.
    	/// </param>
    	/// <param name="validator">
    	/// The validator which dictates how the value will be returned to you.
    	/// </param>
    	/// <returns>
    	/// Returns a value for the currently selected classification unit.
    	/// </returns>
    	/// <remarks>
    	/// If this function has not been told to find a different value-type, the average will be found.
    	/// </remarks>
		/// <exception cref="T:System.NotImplementedException">
		/// Some classification categories have not yet been implemented. If the category you select has not been implemeneted yet, this error is thrown.
		/// </exception>
    	public static int GetStatistics(this PerformanceClass classification, StatisticCondition validator)
		{
			var CPUUsage = new Collection<float>();
			int times = 0;
			const int runFor = 50;
			const int sleepFor = 250;
			float calculatedValue = 0;
			using (PerformanceCounter performanceCounter = new PerformanceCounter())
			{
				switch (classification)
				{
					case PerformanceClass.CpuUsage:
						performanceCounter.CategoryName = "Processor";
						performanceCounter.CounterName = "% Processor Time";
						performanceCounter.InstanceName = "_Total";
						//PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
						performanceCounter.NextValue();
						Console.WriteLine("Program calculating CPU usage...Please wait.\n");
						for (times = 0; times <= runFor; times++)
						{
							System.Threading.Thread.Sleep(sleepFor);
							CPUUsage.Add((float)performanceCounter.NextValue());
						}
						calculatedValue = CPUUsage.Sum();
						break;
					default:
						throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, "The program could not measure this computer's {0}, as the functionality has not yet been implimented.", classification));
				}
				performanceCounter.Close();
    		}
			if(validator != StatisticCondition.SumOf)
			{
				calculatedValue /= times;
				switch (validator)
				{
					case StatisticCondition.ReturnInMilliseconds:
						//First value in the min/max variables is the cap for that range, the second is the value to use if the cap is reached.
						var min = (new List<int> {5, 0}).AsReadOnly();
						var max = (new List<int> {25, 60}).AsReadOnly();
						if (calculatedValue >= max[0])
						{
							calculatedValue = max[1];
						}
						else if (calculatedValue <= min[0])
						{
							calculatedValue = min[1];
						}
						calculatedValue *= 1000;
						break;
				}
			}
			return (int)(Math.Round(calculatedValue, 0, MidpointRounding.AwayFromZero));
		}
    	
        /// <summary>
        /// Sets up the console.
        /// </summary>
        public static void SetupConsole ()
        {
            SafeNativeMethods.AllocConsole();
        }

        /// <summary>
        /// A way to check for required parameters you generally always use before the start of a program.
        /// </summary>
        /// <param name="applicationName">
        /// Name of the parent program.
        /// </param>
        /// <param name="debugMode">
        /// Is the parent program debugging?
        /// </param>
        public static bool CheckBeginningParameters (string applicationName, bool debugMode)
        {
        	//if (debugMode) BackEnd.SetupConsole();
			return Process.GetProcessesByName(applicationName).Count() <= 1 || debugMode;
        }
    }
    
    /// <summary>
    /// All publicly reachable reading and writing related functions.
    /// </summary>
    public static class IOFunc
    {
    	private const string localMachineRootSubKey = "SOFTWARE\\";
    	#region Registry Functions and Overloads
    		#region Clear Registry Functions and Overloads
    		/// <summary>
	    	/// Deletes a subkey and all it's child keys from the registry.
	    	/// </summary>
	    	/// <param name="mainRegistrySubkey">
	    	/// The root (sub)key.
	    	/// </param>
	    	/// <param name="debugging">
	    	/// The program who called this function is debugging.
	    	/// </param>
	    	public static void DeleteRegistrySubkey (string mainRegistrySubkey, bool debugging)
	        {
	    		var registrySubkey = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\{2}\\", localMachineRootSubKey, mainRegistrySubkey, debugging ?  "Debug" : Environment.UserName);
	    		InternalFunction.CheckParameters(registrySubkey, InternalFunction.InternalCondition.RegistrySubkey);
	            using (RegistryKey programKey = Registry.LocalMachine.OpenSubKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\", localMachineRootSubKey, mainRegistrySubkey), true))
	            {
	            	programKey.DeleteSubKeyTree(debugging ?  "Debug" : Environment.UserName);
					programKey.Close();
	            }
	        }
    		#endregion
	    	#region Read Registry Functions and Overloads
	    	/// <summary>
	    	/// Reads a series of values from the registry.
	    	/// </summary>
	    	/// <param name="returnValue">
	    	/// The collection to write the read values to.
	    	/// </param>
	    	/// <param name="mainRegistrySubkey">
	    	/// The top of the registry tree.
	    	/// </param>
	    	/// <param name="debugging">
	    	/// The program who called this function is debugging.
	    	/// </param>
	    	/// <returns>
	    	/// Returns a collection of objects whose said objects represent individual registry entries.
	    	/// </returns>
	    	public static IEnumerable<RegistryEntry> ReadFromRegistry (this IEnumerable<RegistryEntry> returnValue, string mainRegistrySubkey, bool debugging)
	        {
	    		return (new Collection<RegistryEntry>(returnValue.ToList())).ReadFromRegistry(mainRegistrySubkey, debugging);
	        }
	    	
	    	/// <summary>
	    	/// Reads a series of values from the registry.
	    	/// </summary>
	    	/// <param name="returnValue">
	    	/// The collection to write the read values to.
	    	/// </param>
	    	/// <param name="mainRegistrySubkey">
	    	/// The top of the registry tree.
	    	/// </param>
	    	/// <param name="debugging">
	    	/// The program who called this function is debugging.
	    	/// </param>
	    	/// <returns>
	    	/// Returns a collection of objects whose said objects represent individual registry entries.
	    	/// </returns>
	    	public static IEnumerable<RegistryEntry> ReadFromRegistry (this ICollection<RegistryEntry> returnValue, string mainRegistrySubkey, bool debugging)
	        {
	    		var registrySubkey = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\{2}\\", localMachineRootSubKey, mainRegistrySubkey, debugging ?  "Debug" : Environment.UserName);
	    		InternalFunction.CheckParameters(registrySubkey, InternalFunction.InternalCondition.RegistrySubkey);
	            using (RegistryKey programKey = Registry.LocalMachine.OpenSubKey(registrySubkey, true))
	            {
	            	var registryValues = programKey.GetValueNames().GetEnumerator();
	            	for (var position = registryValues; position.MoveNext();)
					{
	            		string registryEntry = position.Current.ToString();
						returnValue.Add(new RegistryEntry(registryEntry, programKey.GetValue(registryEntry)));
					}
					programKey.Close();
	            }
	            return returnValue.AsEnumerable();
	        }
	    	#endregion
	    	#region Write Registry Functions and Overloads
	    	/// <summary>
	    	/// Writes a series of values to the registry.
	    	/// </summary>
	    	/// <param name="registryValues">
	    	/// A collection of objects which represent individual registry entries.
	    	/// </param>
	    	/// <param name="mainRegistrySubkey">
	    	/// The top of the registry tree.
	    	/// </param>
	    	/// <param name="debugging">
	    	/// The program who called this function is debugging.
	    	/// </param>
	    	public static void WriteToRegistry (this IEnumerable<RegistryEntry> registryValues, string mainRegistrySubkey, bool debugging)
	        {
	    		WriteToRegistry(registryValues.GetEnumerator(), mainRegistrySubkey, debugging);
	        }
	    	
	    	/// <summary>
	    	/// Writes a series of values to the registry.
	    	/// </summary>
	    	/// <param name="registryValues">
	    	/// A collection of objects which represent individual registry entries.
	    	/// </param>
	    	/// <param name="mainRegistrySubkey">
	    	/// The top of the registry tree.
	    	/// </param>
	    	/// <param name="debugging">
	    	/// The program who called this function is debugging.
	    	/// </param>
	    	public static void WriteToRegistry (this IEnumerator<RegistryEntry> registryValues, string mainRegistrySubkey, bool debugging)
	        {
	    		var registrySubkey = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\{2}\\", localMachineRootSubKey, mainRegistrySubkey, debugging ?  "Debug" : Environment.UserName);
	    		InternalFunction.CheckParameters(registrySubkey, InternalFunction.InternalCondition.RegistrySubkey);
	            using (RegistryKey programKey = Registry.LocalMachine.OpenSubKey(registrySubkey, true))
	            {
					for (var position = registryValues; position.MoveNext();)
					{
						var item = position.Current;
						programKey.SetValue(item.Name, item.Value);
					}
					programKey.Close();
	            }
	        }
	    	#endregion
    	#endregion
    	#region File IO Functions and Overloads
    		#region Add To Functions and Overloads
			/// <summary>
			/// Adds the contents of a file to a collection.
			/// </summary>
			/// <param name="collectionToUse">
			/// The collection of objects you wish to add to.
			/// </param>
			/// <param name="fileToUse">
			/// The file whose contents you wish to add to the provided collection.
			/// </param>
			/// <returns>
			/// Returns a collection of objects containing the file's contents.
			/// </returns>
			public static IEnumerable<string> AddFileContents (this IEnumerable<string> collectionToUse, string fileToUse)
			{
				return ((Collection<string>)collectionToUse).AddFileContents(fileToUse, string.Empty);
			}
	
	    	/// <summary>
	    	/// Adds the contents of a file to a collection.
	    	/// </summary>
	    	/// <param name="collectionToUse">
	    	/// The collection of objects you wish to add to.
	    	/// </param>
	    	/// <param name="fileToUse">
	    	/// The file whose contents you wish to add to the provided collection.
	    	/// </param>
	    	/// <param name="separator">
	    	/// A singular string used to represent how entries read from a file should be separated before being added to the collection.
	    	/// </param>
	    	/// <remarks>
	    	/// The separator is mainly used for puppet collections; collections that never need to be read from, only added to. Until writing to a file, of course.
	    	/// </remarks>
	    	/// <returns>
	    	/// Returns a collection of objects containing the file's contents.
	    	/// </returns>
	    	public static IEnumerable<string> AddFileContents (this ICollection<string> collectionToUse, string fileToUse, string separator)
	        {
	            if (File.Exists(fileToUse))
				{
					using (StreamReader infile = new StreamReader(fileToUse))
					{
						String line;
						int position = 0;
						while (!infile.EndOfStream)
						{
							if(!string.IsNullOrEmpty(line = infile.ReadLine()))
							{
								collectionToUse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}", position <= 0 || string.IsNullOrEmpty(separator) ? string.Empty : separator, line));
								position++;
							}
						}
						infile.Close();
					}
				}
	            return collectionToUse.AsEnumerable();
	        }
	    	#endregion
	    	#region Write To Functions and Overloads
			/// <summary>
			/// Write a collection to a file.
			/// </summary>
			/// <param name="itemsToUse">
			/// The collection to write to file.
			/// </param>
			/// <param name="fileName">
			/// The name of the file you wish to write to with extension.
			/// </param>
			public static void WriteToFile<T> (this IEnumerable<T> itemsToUse, string fileName)
			{
				itemsToUse.WriteToFile(fileName, new List<string>());
			}
			
			/// <summary>
			/// Write a collection to a file.
			/// </summary>
			/// <param name="itemsToUse">
			/// The collection to write to file.
			/// </param>
			/// <param name="fileName">
			/// The name of the file you wish to write to with extension.
			/// </param>
			/// <param name="extension">
	    	/// The extension for the file you wish to write to.
	    	/// </param>
	    	/// <remarks>
	    	/// The file name does not have to be without an extension. Likewise the "extension" can have extra information. If you desire, of course.
	    	/// </remarks>
			public static void WriteToFile<T> (this IEnumerable<T> itemsToUse, string fileName, string extension)
			{
				itemsToUse.WriteToFile(fileName, new List<string> { extension });
			}
			
			/// <summary>
			/// Write a collection to a file.
			/// </summary>
			/// <param name="itemsToUse">
			/// The collection to write to file.
			/// </param>
			/// <param name="fileName">
			/// The name of the file you wish to write to with extension.
			/// </param>
			/// <param name="extensions">
	    	/// The extension(s) for the file you wish to write to.
	    	/// </param>
	    	/// <remarks>
	    	/// The file name does not have to be without an extension. Likewise the "extensions" can have extra information. If you desire, of course.
	    	/// </remarks>
			public static void WriteToFile<T> (this IEnumerable<T> itemsToUse, string fileName, IEnumerable<string> extensions)
			{
				itemsToUse.WriteToFile(fileName, extensions, FileAttributes.Compressed);
			}
			
			/// <summary>
	    	/// Write a collection to a file.
	    	/// </summary>
	    	/// <param name="itemsToUse">
	    	/// The collection to write to file.
	    	/// </param>
	    	/// <param name="fileName">
	    	/// The name of the file you wish to write to.
	    	/// </param>
	    	/// <param name="extensions">
	    	/// The extension(s) for the file you wish to write to.
	    	/// </param>
	    	/// <param name="fileSettings">
	    	/// The setting(s) for the file you wish to write to.
	    	/// </param>
	    	/// <remarks>
	    	/// The file name does not have to be without an extension. Likewise the "extensions" can have extra information. If you desire, of course.
	    	/// </remarks>
	    	public static void WriteToFile<T> (this IEnumerable<T> itemsToUse, string fileName, IEnumerable<string> extensions, FileAttributes fileSettings)
	        {
	    		itemsToUse.GetEnumerator().WriteToFile(fileName, extensions, fileSettings);
	        }
			
	    	/// <summary>
	    	/// Write a collection to a file.
	    	/// </summary>
	    	/// <param name="itemsToUse">
	    	/// The collection to write to file.
	    	/// </param>
	    	/// <param name="fileName">
	    	/// The name of the file you wish to write to.
	    	/// </param>
	    	/// <param name="extensions">
	    	/// The extension(s) for the file you wish to write to.
	    	/// </param>
	    	/// <param name="fileSettings">
	    	/// The setting(s) for the file you wish to write to.
	    	/// </param>
	    	/// <remarks>
	    	/// The file name does not have to be without an extension. Likewise the "extensions" can have extra information. If you desire, of course.
	    	/// </remarks>
	    	public static void WriteToFile<T> (this IEnumerator<T> itemsToUse, string fileName, IEnumerable<string> extensions, FileAttributes fileSettings)
	        {
				var fileToWriteTo = string.Format(CultureInfo.InstalledUICulture, "{0}{1}", fileName, string.Join(string.Empty, extensions));
	    		using (StreamWriter outfile = new StreamWriter(fileToWriteTo))
	            {
	    			for (var position = itemsToUse; position.MoveNext();)
					{
						outfile.Write(position.Current);
					}
	                outfile.Close();
	            }
	            File.SetAttributes(fileToWriteTo, fileSettings);
	        }
	    	#endregion
	    	#region Deletion Functions and Overloads
	        /// <summary>
	        /// Checks all files in all subdirectories of the root directory against the file provided to see if their names match. If they do, it deletes the appropriate file.
	        /// </summary>
	        /// <param name="fileToSearchWith">
	        /// The file, or logically it's information, that you'll be searching with.
	        /// </param>
	        /// <param name="rootDirectory">
	        /// The top of the directory hierarchy you'd like to search in.
	        /// </param>
	        public static void DeleteAllSameNamedFiles (FileInfo fileToSearchWith, string rootDirectory)
	        {
	            DeleteAllSameNamedFiles(fileToSearchWith, rootDirectory, false, true);
	        }
	
	        /// <summary>
	        /// Checks all files in all subdirectories of the root directory against the file provided to see if their names match. If they do, it deletes the appropriate file.
	        /// </summary>
	        /// <param name="fileToSearchWith">
	        /// The file, or logically it's information, that you'll be searching with. This is the root file.
	        /// </param>
	        /// <param name="rootDirectory">
	        /// The top of the directory hierarchy you'd like to search in.
	        /// </param>
	        /// <param name="ignoreCase">
	        /// If true, the function will ignore casing when checking the files in the hierarchy against the root file.
	        /// </param>
	        public static void DeleteAllSameNamedFiles (FileInfo fileToSearchWith, string rootDirectory, bool ignoreCase)
	        {
	            DeleteAllSameNamedFiles(fileToSearchWith, rootDirectory, ignoreCase, true);	
	        }
	
	        /// <summary>
	        /// Checks all files in all subdirectories of the root directory against the file provided to see if their names match. If they do, it deletes the appropriate file.
	        /// </summary>
	        /// <param name="fileToSearchWith">
	        /// The file, or logically it's information, that you'll be searching with. This is the root file.
	        /// </param>
	        /// <param name="rootDirectory">
	        /// The top of the directory hierarchy you'd like to search in.
	        /// </param>
	        /// <param name="ignoreCase">
	        /// If true, the function will ignore casing when checking the files in the hierarchy against the root file.
	        /// </param>
	        /// /// <param name="deleteAtDirectory">
	        /// If true, the function will delete the provided file once a file is found in the directory hierarchy with the exact same filename. If false, deletes the file in the hierarchy, and preserves the root file.
	        /// </param>
	        public static void DeleteAllSameNamedFiles (FileInfo fileToSearchWith, string rootDirectory, bool ignoreCase, bool deleteAtDirectory)
	        {
	            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Starting deletion...Looking in '{0}' for files with the exact same name as '{1}', while {2} case.", rootDirectory, fileToSearchWith.Name, ignoreCase ? "ignoring" : "including"));
				
	            bool stopChecking = false;
				
				int currentPosition = 0, maxLengthDir = Directory.GetDirectories(rootDirectory).Length;
	            while (currentPosition < maxLengthDir && !stopChecking)
				{
					string d = Directory.GetDirectories(rootDirectory)[currentPosition];
					Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Now looking in {0}", d));
					if (!fileToSearchWith.DirectoryName.Equals(d))
					{
						DirectoryInfo dI = new DirectoryInfo(d);
						for (int currentFile = 0, maxLength = dI.GetFiles().Length; currentFile < maxLength; currentFile++)
						{
							FileInfo file = dI.GetFiles()[currentFile];
							if (file != null && file.Name.Equals(fileToSearchWith.Name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
							{
								Console.WriteLine(string.Format(CultureInfo.InvariantCulture, deleteAtDirectory ? "{0}\\{1} is being deleted, {2}\\{3} will be preserved." : "{2}\\{3} is being deleted, {0}\\{1} will be preserved.", fileToSearchWith.DirectoryName, fileToSearchWith.Name, file.DirectoryName, file.Name));
								if (deleteAtDirectory) fileToSearchWith.Delete();
								else file.Delete();
								stopChecking = true;
								return;
							}
						}
					}
					else
					{
						Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Cannot search {0} as it is the directory of the file you're searching with.", d));
					}
					currentPosition++;
				}
	        }
	        #endregion
        #endregion
        #region Directory IO Functions and Overloads
	        #region Create Hierarchy Functions and Overloads
	        /// <summary>
	        /// Creates a folder hierarchy based on the default values.
	        /// </summary>
	        public static void CreateFolderTree ()
	        {
	        	CreateFolderTree(GenericFolderOptions.BaseFolderRoot, GenericFolderOptions.NewStyleFileTree);
	        }
	        
	        /// <summary>
	        /// Creates a folder hierarchy of your choosing using the directory hierarchy provided.
	        /// </summary>
	        /// <param name="directory">
	        /// An array of strings with the FULL directory path of the folder you'd like to create.
	        /// </param>
	        /// <example>
	        /// "SomeDrive\\SomeRoot\\SomeFolder\\" is a correct value for the array. You can also use "SomeFolder\\SomeSubFolder\\" as well to indicate the root directory is the application's root directory. "SomeDrive\\SomeRoot\\SomeFolder" is not correct.
	        /// </example>
	        public static void CreateFolderTree (string directory)
	        {
	        	CreateFolderTree(new Collection<string> { directory });
	        }
	        
	        /// <summary>
	        /// Creates a folder hierarchy of your choosing using the directory hierarchy provided.
	        /// </summary>
	        /// <param name="appendedDirectoryRoot">
	        /// The directory root to be applied to all folders in the directory hierarchy.
	        /// </param>
	        /// <example>
	        /// "C:\\".
	        /// </example>
	        /// <param name="directory">
	        /// A list of directories with the FULL directory path of the folder you'd like to create.
	        /// </param>
	        /// <example>
	        /// If there is no appended directory root provided: "SomeDrive\\SomeRoot\\SomeFolder\\" is a correct value for the array. You can also use "SomeFolder\\SomeSubFolder\\" as well to indicate the root directory is the application's root directory. "SomeDrive\\SomeRoot\\SomeFolder" is not correct.
	        /// </example>
	        /// <example>
	        /// If there is an appended directory root provided: "SomeFolder\\" is a correct value for the array. You can also use "SomeFolder\\SomeSubFolder\\" as well, and so on. "SomeFolder" is not correct
	        /// </example>
	        public static void CreateFolderTree (string appendedDirectoryRoot, string directory)
	        {
	        	CreateFolderTree(appendedDirectoryRoot, new Collection<string> { directory });
	        }
	
	        /// <summary>
	        /// Creates a folder hierarchy of your choosing using the directory hierarchy provided.
	        /// </summary>
	        /// <param name="directoryHierarchy">
	        /// An array of strings with the FULL directory path of the folder you'd like to create.
	        /// </param>
	        /// <example>
	        /// "SomeDrive\\SomeRoot\\SomeFolder\\" is a correct value for the array. You can also use "SomeFolder\\SomeSubFolder\\" as well to indicate the root directory is the application's root directory. "SomeDrive\\SomeRoot\\SomeFolder" is not correct.
	        /// </example>
	        public static void CreateFolderTree (IEnumerable<string> directoryHierarchy)
	        {
	            CreateFolderTree(null, directoryHierarchy);
	        }
	
	        /// <summary>
	        /// Creates a folder hierarchy of your choosing using the directory hierarchy provided.
	        /// </summary>
	        /// <param name="appendedDirectoryRoot">
	        /// The directory root to be applied to all folders in the directory hierarchy.
	        /// </param>
	        /// <example>
	        /// "C:\\".
	        /// </example>
	        /// <param name="directoryHierarchy">
	        /// A list of directories with the FULL directory path of the folder you'd like to create.
	        /// </param>
	        /// <example>
	        /// If there is no appended directory root provided: "SomeDrive\\SomeRoot\\SomeFolder\\" is a correct value for the array. You can also use "SomeFolder\\SomeSubFolder\\" as well to indicate the root directory is the application's root directory. "SomeDrive\\SomeRoot\\SomeFolder" is not correct.
	        /// </example>
	        /// <example>
	        /// If there is an appended directory root provided: "SomeFolder\\" is a correct value for the array. You can also use "SomeFolder\\SomeSubFolder\\" as well, and so on. "SomeFolder" is not correct
	        /// </example>
	        public static void CreateFolderTree (string appendedDirectoryRoot, IEnumerable<string> directoryHierarchy)
	        {
	            if (directoryHierarchy != null && directoryHierarchy.Any())
	            {
	            	var directoriesToCreate = directoryHierarchy.Where(dir => !Directory.Exists(dir));
	            	for (var position = directoriesToCreate.GetEnumerator(); position.MoveNext();)
	                {
	                	string correctedDirectory = string.IsNullOrEmpty(appendedDirectoryRoot) ? position.Current : string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\", appendedDirectoryRoot, position.Current);
						Directory.CreateDirectory(correctedDirectory);
	                }
	            }
	        }
	        #endregion
        #endregion
    }

    /// <summary>
    /// Dunno, FxCop told me to call it this.
    /// </summary>
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole ();
    }
	
    /// <summary>
    /// DO NOT USE ANY OF THESE FUNCTIONS OUTSIDE THE UNIVERSALHANDLERSLIBRARY MAIN CODE.
    /// </summary>
    internal static class InternalFunction
    {
    	#region Internal Custom Items
    	/// <summary>
    	/// Internal conditions meant only for use within the library.
    	/// </summary>
    	internal enum InternalCondition
    	{
    		/// <summary>
    		/// Tells the function to prune.
    		/// </summary>
    		Prune,
    		/// <summary>
    		/// Tells the function to look for a registry subkey and, if one does not exist, create it.
    		/// </summary>
    		RegistrySubkey
    	}
    	#endregion
    	#region Parameter Validation Functions and Overloads
        /// <summary>
        /// Depending on the parameter name provided, it makes sure the provided value is within the conditions specified within the code.
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter you are currently checking.
        /// </param>
        internal static void CheckParameters<T> (T value, InternalCondition parameterName)
        {
			string valueAsString = value.ToString();
			int valueAsInt = 0;
			if(!int.TryParse(valueAsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out valueAsInt)) valueAsInt = default(int);
			switch (parameterName)
			{
				case InternalCondition.Prune:
					if (valueAsInt < InternalParamaters.minPruneVal || valueAsInt > InternalParamaters.maxPruneVal)
					{
						throw new ArgumentOutOfRangeException("value", value, string.Format(CultureInfo.CurrentCulture, "UniversalHandlersLibrary: value must be between {0} and {1}", InternalParamaters.minPruneVal, InternalParamaters.maxPruneVal));
					}
					break;
				case InternalCondition.RegistrySubkey:
					using (RegistryKey localKey = Registry.LocalMachine)
           	 		{
						if (localKey.OpenSubKey(valueAsString) == null) localKey.CreateSubKey(valueAsString);
						localKey.Close();
					}
					break;
				default:
					throw new ArgumentException("When checking parameters, the passed parameter was not a valid selection.");
			}
        }
        
        /// <summary>
        /// Depending on the parameter name provided, it makes sure the provided value is within the conditions specified within the code.
        /// </summary>
        /// <param name="valueToCheck">
        /// The value to check.
        /// </param>
		/// <param name = "acceptableValue">
		/// The acceptable value to check against valueToCheck.
		/// </param>
        internal static void CheckParameters<T> (T valueToCheck, T acceptableValue)
        {
        	CheckParameters(new List<T> { valueToCheck }.AsEnumerable(), new List<T> { acceptableValue }.AsEnumerable());
        }
        
        /// <summary>
        /// Depending on the parameter name provided, it makes sure the provided value is within the conditions specified within the code.
        /// </summary>
        /// <param name="valuesToCheck">
        /// The values to check.
        /// </param>
		/// <param name = "acceptableValues">
		/// The acceptable values to check against valueToCheck.
		/// </param>
        internal static void CheckParameters<T> (IEnumerable<T> valuesToCheck, IEnumerable<T> acceptableValues)
        {
        	if(!valuesToCheck.Any() || !acceptableValues.Any())
        	{
        		throw new ArgumentOutOfRangeException("A critical collection of values was passed with no values held (empty).", new ArgumentException("When checking parameters, neither the values you are checking nor the acceptable values can be empty."));
        	}
        	if(valuesToCheck.All(item => item is GenericCondition))
        	{
        		if (!valuesToCheck.All(item => acceptableValues.Any(checkedItem => checkedItem.Equals(item))) && !valuesToCheck.All(item => item.Equals(GenericCondition.None)))
				{
					throw new FormatException("The condition(s) passed was/were not one of the acceptable conditions for that purpose.");
				}
        	}
        	else
        	{
        		throw new ArgumentException("The type of value you are checking is not valid.");
        	}
        }
        
        /// <summary>
        /// Checks to see if the provided file should be pruned.
        /// </summary>
        /// <param name="fileToCheck">
        /// File whose size is to be checked in this operation.
        /// </param>
        /// <param name="pruneCutoff">
        /// The size at which a file is pruned.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Thrown if the pruneCutOff value is less than <see cref="InternalParamaters.minPruneVal"/>, 
        /// or greater than <see cref="InternalParamaters.maxPruneVal"/>.
        /// </exception>
        internal static void PruneCheck (this string fileToCheck, int pruneCutoff)
        {
            if (File.Exists(fileToCheck))
            {
                long fileSize = new FileInfo (fileToCheck).Length;
                CheckParameters(pruneCutoff, InternalCondition.Prune);
				if (fileSize < pruneCutoff) return;
				
                //<see cref="UniversalHandlersLibrary.ConstParameters.minPruneVal"/> regular pruneCutoff Messages
                //524288 regular pruneCutoff Errors
				
				var prunedFile = string.Format(CultureInfo.CurrentCulture, "{0}.old", fileToCheck);
				if (File.Exists(prunedFile)) File.Delete(prunedFile);
				File.Copy(fileToCheck, prunedFile);
                File.Delete(fileToCheck);
                File.WriteAllText(fileToCheck, string.Empty);
            }
        }
        #endregion

        /// <summary>
        /// Central writing code for most writing operations required by this library.
        /// </summary>
        /// <param name="fileContents">
        /// Message to write to the file.
        /// </param>
        /// <param name="file">
        /// The file to write to.
        /// </param>
        /// <param name="prune">
        /// Is this operation checking the file to see if it should be pruned?
        /// </param>
        /// <remarks>
        /// Does not check after the file has been written to.
        /// </remarks>
        /// <param name="pruneCutoff">
        /// The file size at which to prune the file.
        /// </param>
        internal static void InternalWriter (this ICollection<string> fileContents, string file, bool prune, int pruneCutoff)
        {
        	fileContents.AddFileContents(file, "\n\n");
			if (prune) file.PruneCheck(pruneCutoff);
			fileContents.WriteToFile(file);
        }
        #region Return Path Functions, Overloads, and Misc
		/// <summary>
		/// Returns the most logical path available for the provided file.
		/// </summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		/// <returns>
		/// Returns the desired file path of the provided file name.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">
		/// Thrown if, when finding the position in the searched folders housing the passed file, this function attempts to use a position that should not exist.
		/// </exception>
		internal static string ReturnFilePath(string fileName)
		{
			return ReturnFilePath(fileName, GenericFolderOptions.BaseFolderRoot, GenericFolderOptions.NewStyleFileTree, null);
		}
		
		/// <summary>
		/// Represents a file extension.
		/// </summary>
        internal class FileDefinition
        {
        	/// <summary>
        	/// The extension's directory.
        	/// </summary>
        	public string Key { get; set; }
        	
        	/// <summary>
        	/// The extension.
        	/// </summary>
        	public string Value { get; set; }
        	
        	/// <summary>
        	/// Represents a file extension.
        	/// </summary>
        	/// <param name="key">
        	/// The extension's directory.
        	/// </param>
        	/// <param name="value">
        	/// The extension.
        	/// </param>
        	public FileDefinition(string key, string value)
        	{
        		this.Key = key;
        		this.Value = value;
        	}
        }

        /// <summary>
        /// Returns the most logical path available for the provided file.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file.
        /// </param>
        /// <param name = "BaseRoot">
        /// The root appended to all found file paths
        /// </param>
		/// <param name = "directoryHierarchy">
		/// The list of directories to use.
		/// </param>
		/// <param name = "Definitions">
		/// List of extensions and their respective folder to be used as sorting rules when determining a file path.
		/// </param>
        /// <returns>
        /// Returns the desired file path of the provided file name.
        /// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">
		/// Thrown if, when finding the position in the searched folders housing the passed file, this function attempts to use a position that should not exist.
		/// </exception>
        internal static string ReturnFilePath (string fileName, string BaseRoot, IEnumerable<string> directoryHierarchy, IEnumerable<FileDefinition> Definitions)
        {
        	var FileDefinitions = Definitions != null && Definitions.Any() ? Definitions : new List<FileDefinition>
        	{
        	    new FileDefinition("text", ".txt"),
				new FileDefinition("text", ".text"),
        		new FileDefinition("text", ".dat"),
        		new FileDefinition("text", ".data"),
        		new FileDefinition("text", ".log"),
 
        		new FileDefinition("asset", ".jpg"),
        		new FileDefinition("asset", ".jpeg"),
        		new FileDefinition("asset", ".bmp"),
        		new FileDefinition("asset", ".gif"),
        		new FileDefinition("asset", ".png"),
        		new FileDefinition("asset", ".ico"),
                
        		new FileDefinition("help", "NOTHINGYET!!!!11!!!1"),
        		
        		new FileDefinition("backup", ".bak"),
        		new FileDefinition("backup", ".old")
        	}.AsEnumerable();
        	string returnPath = string.Empty;
        	try
        	{
        		returnPath += string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\{2}", BaseRoot, directoryHierarchy.First(item =>
					Directory.Exists(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", BaseRoot, item))
					&&					
	        		(
	        			FileDefinitions.Any(checkedItem => 
	        		    	(
	        		        	!checkedItem.Key.Equals("text", StringComparison.OrdinalIgnoreCase)
				        		? 
				        			item.Contains(checkedItem.Key, StringComparison.OrdinalIgnoreCase)
				        		: 
				        			item.Equals(checkedItem.Key, StringComparison.OrdinalIgnoreCase)
			        		)
	        		        && fileName.Contains(checkedItem.Value, StringComparison.OrdinalIgnoreCase) 
		        		)
        		    )
				), fileName);
        	}
        	catch (InvalidOperationException) {}
        	if(string.IsNullOrEmpty(returnPath))
			{
				returnPath = Directory.Exists(BaseRoot) ? string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", BaseRoot, fileName) : fileName;
			}
			return returnPath;
        }	
    }	
    #endregion
}