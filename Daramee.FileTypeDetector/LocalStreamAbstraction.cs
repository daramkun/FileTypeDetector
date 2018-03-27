using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static TagLib.File;

namespace Daramee.FileTypeDetector
{
	class LocalStreamAbstraction : IFileAbstraction
	{
		public string Name => "";

		public Stream ReadStream { get; private set; }

		public Stream WriteStream => throw new NotImplementedException ();

		public LocalStreamAbstraction ( Stream stream )
		{
			ReadStream = stream;
		}

		public void CloseStream ( Stream stream )
		{
			stream.Close ();
		}
	}
}
