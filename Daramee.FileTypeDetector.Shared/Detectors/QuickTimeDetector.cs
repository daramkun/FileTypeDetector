using System.Collections.Generic;
using System.IO;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Video )]
	[FormatCategory ( FormatCategories.Audio )]
	class QuickTimeDetector : AbstractISOBaseMediaFileDetailDetector
	{
		public override string Extension => "mov";

		protected override IEnumerable<string> NextSignature { get { yield return "qt"; } }

		public override string ToString () => "QuickTime(MOV) Detector";

		public override bool Detect ( Stream stream )
		{
			if ( !base.Detect ( stream ) )
			{
				stream.Position = 4;
				byte [] buffer = new byte [ 4 ];
				stream.Read ( buffer, 0, 4 );
				if ( buffer [ 0 ] == 0x6D && buffer [ 1 ] == 0x6F && buffer [ 2 ] == 0x6F && buffer [ 3 ] == 0x76 )
					return true;
				return false;
			}
			return true;
		}
	}
}
