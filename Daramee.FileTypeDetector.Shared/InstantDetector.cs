using System;
using System.IO;

namespace Daramee.FileTypeDetector
{
	public class InstantDetector : IDetector
	{
		public string Description { get; private set; }
		public string Extension { get; private set; }
		public string Precondition { get; private set; }

		public event Func<Stream, bool> DetectionLogic;

		private int cachedHashcode;

		public InstantDetector ( string extension, Func<Stream, bool> logic, string description, string precondition = null )
		{
			Extension = extension;
			Precondition = precondition;
			Description = description;
			DetectionLogic += logic;

			cachedHashcode = $"{Description}{Extension}{Precondition}".GetHashCode ();
		}

		public bool Detect ( Stream stream ) { return DetectionLogic ( stream ); }

		public override string ToString () { return Description; }
		public override int GetHashCode () { return cachedHashcode; }
	}
}
