using System.IO;
using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Video )]
	[FormatCategory ( FormatCategories.Audio )]
	class WebMDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] WEBM_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x1A, 0x45, 0xDF, 0xA3 } },
		};

		public override string Extension => "webm";

		protected override SignatureInformation [] SignatureInformations => WEBM_SignatureInfo;

		public override string ToString () => "WebM Video Detector";

		public override bool Detect ( Stream stream )
		{
			if ( base.Detect ( stream ) )
			{
				stream.Position = 0x1F;
				byte [] buffer = new byte [ 4 ];
				stream.Read ( buffer, 0, 4 );
				if ( Encoding.GetEncoding ( "ascii" ).GetString ( buffer, 0, 4 ) != "webm" )
					return false;

				return true;
			}
			return false;
		}
	}
}
