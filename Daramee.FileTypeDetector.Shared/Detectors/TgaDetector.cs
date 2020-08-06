using System.IO;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class TgaDetector : IDetector
	{
		public string Extension => "tga";

		public string Precondition => null;

		public bool Detect ( Stream stream )
		{
			stream.Position = 3;
			int compressionType = stream.ReadByte ();
			if ( !( compressionType == 0 || compressionType == 1 || compressionType == 2
				|| compressionType == 3 || compressionType == 9 || compressionType == 10
				|| compressionType == 11 || compressionType == 32 || compressionType == 33 ) )
				return false;

			stream.Position = 17;
			int depth = stream.ReadByte ();
			if ( !( depth == 8 || depth == 24 || depth == 15 || depth == 16 || depth == 32 ) )
				return false;

			stream.Position = 1;
			int mapType = stream.ReadByte ();
			if ( !( mapType == 0 || ( mapType == 1 && depth == 8 ) ) )
				return false;

			return true;
		}

		public override string ToString () => "Targa Detector (Experimental)";
	}
}
