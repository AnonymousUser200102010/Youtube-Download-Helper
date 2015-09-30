using System;
using UniversalHandlersLibrary;

namespace YoutubeDownloadHelper
{
	/// <summary>
	/// A generic exception thrown when converting operations could not complete for some reason.
	/// </summary>
    [Serializable]
	public class InvalidConversionException : Exception
	{
		public InvalidConversionException ()
		{
		}

		public InvalidConversionException (string message) : base(message)
		{
		}

		public InvalidConversionException (string message, Exception inner) : base(message, inner)
		{
		}

		protected InvalidConversionException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}

	/// <summary>
	/// Thrown when parsing operations could not finish.
	/// </summary>
    [Serializable]
	public class ParsingException : Exception
	{
		public ParsingException ()
		{
		}

		public ParsingException (string message) : base(message)
		{
		}

		public ParsingException (string message, Exception inner) : base(message, inner)
		{
		}
		
        protected ParsingException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
    
    /// <summary>
    /// Generic error thrown when an exception that might normally not crash the program, but corrupt data, occurs.
    /// </summary>
    [Serializable]
    public class FatalException : Exception
    {
        public FatalException ()
        {
        }

        public FatalException (string message) : base(message)
        {
        }

        public FatalException (string message, Exception inner) : base(message, inner)
        {
        }
			
        protected FatalException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
    
    /// <summary>
    /// Used as a generic "stop" and to communicate that the download cancelled non-fatally.
    /// </summary>
    [Serializable]
    public class DownloadCanceledException : Exception
    {
        public DownloadCanceledException ()
        {
        }

        public DownloadCanceledException (string message) : base(message)
        {
        }

        public DownloadCanceledException (string message, Exception inner) : base(message, inner)
        {
        }
			
        protected DownloadCanceledException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
    
    public class PrebakedError : YoutubeDownloadHelper.Code.IError
    {
    	public void Alert(Exception ex)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Fatal System Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            Environment.Exit(0);
            new FatalException("A fatal exception has occurred.", ex).Log();
        }
    }
}
