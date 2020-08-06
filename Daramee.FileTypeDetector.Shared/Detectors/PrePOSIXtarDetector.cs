using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	class POSIXtarDetector : IDetector
	{
		public string Precondition => null;
		public string Extension => "tar";

		public bool Detect ( Stream stream )
		{
			stream.Position = 100;
			byte [] mode = new byte [ 8 ];
			stream.Read ( mode, 0, 8 );
			if ( !Regex.IsMatch ( Encoding.GetEncoding ( "ascii" ).GetString ( mode, 0, 8 ), "[0-7][0-7][0-7][0-7][0-7][0-7][0-7][\0]" ) )
				return false;

			/*stream.Position = 156;
			stream.Read ( mode, 0, 1 );
			char linkIndicator = ( char ) mode [ 0 ];
			switch ( linkIndicator )
			{
				case '\0':
				case '0':
				case '1':
				case '2':
					return true;
			}

			return false;*/
			return true;
		}

		public override string ToString () => "pre-POSIX Tar(TAR) Detector";
	}
}
