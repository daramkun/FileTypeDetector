using System.IO;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class DirectDrawSurfaceFormatDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] DDS_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x44, 0x44, 0x53, 0x20 } },
		};

		public override string Extension => "dds";

		protected override SignatureInformation [] SignatureInformations => DDS_SignatureInfo;

		public override string ToString () => "DirectDraw Surface(DDS) Detector";

		public override bool Detect ( Stream stream )
		{
			if ( base.Detect ( stream ) )
			{
				byte [] buffer = new byte [ 4 ];

				stream.Read ( buffer, 0, 4 );
				int dwSize = ( ( ( int ) buffer [ 3 ] ) << 24 ) + ( ( ( int ) buffer [ 2 ] ) << 16 ) + ( ( ( int ) buffer [ 1 ] ) << 8 ) + buffer [ 0 ];
				if ( dwSize != 0x7C )
					return false;

				stream.Read ( buffer, 0, 4 );
				int dwFlags = ( ( ( int ) buffer [ 3 ] ) << 24 ) + ( ( ( int ) buffer [ 2 ] ) << 16 ) + ( ( ( int ) buffer [ 1 ] ) << 8 ) + buffer [ 0 ];

				if ( ( ( dwFlags & 0x1 ) != 0 ) && ( ( dwFlags & 0x2 ) != 0 ) && ( ( dwFlags & 0x4 ) != 0 ) && ( ( dwFlags & 0x1000 ) != 0 ) )
					return true;
			}
			return false;
		}
	}
}
