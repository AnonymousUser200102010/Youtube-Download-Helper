using System;

namespace YoutubeDownloadHelper
{
	/// <summary>
	/// A generic exception thrown when a converting function could not complete for some reason.
	/// </summary>
	[Serializable]
	public class InvalidConversionException : Exception
	{
		public InvalidConversionException() : base() { }
		public InvalidConversionException(string message) : base(message) { }
		public InvalidConversionException(string message, Exception inner) : base(message, inner) { }
			
		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
		protected InvalidConversionException(System.Runtime.Serialization.SerializationInfo info,
			                                  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
	/// <summary>
	/// Thrown when a parsing operation could not finish.
	/// </summary>
	[Serializable]
	public class UnparsableException : Exception
	{
		public UnparsableException() : base() { }
		public UnparsableException(string message) : base(message) { }
		public UnparsableException(string message, Exception inner) : base(message, inner) { }
			
		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
		protected UnparsableException(System.Runtime.Serialization.SerializationInfo info,
			                                  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
