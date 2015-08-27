using System;

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
			
        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
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
			
        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
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
			
        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected FatalException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
    
    public class PrebakedError : YoutubeDownloadHelper.Code.IError
    {
    	public void Alert(Exception ex)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Fatal System Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            Environment.Exit(0);
        }
    }
}
