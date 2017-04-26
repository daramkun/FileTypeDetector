using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class MatroskaVideoDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] MKV_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x1A, 0x45, 0xDF, 0xA3 } },
		};

		public override string Extension => "mkv";

		protected override SignatureInformation [] SignatureInformations => MKV_SignatureInfo;

		public override string ToString () => "Matroska Container Video Detector";

		public override bool Detect ( Stream stream )
		{
			if ( base.Detect ( stream ) )
			{
				stream.Position = 0x1F;
				byte [] buffer = new byte [ 8 ];
				stream.Read ( buffer, 0, 8 );
				if ( Encoding.GetEncoding ( "ascii" ).GetString ( buffer, 0, 8 ) != "matroska" )
					return false;
				
				using ( var file = new TagLib.Matroska.File ( new TagLib.File.LocalStreamAbstraction ( stream ) ) )
				{
					if ( file.Properties.MediaTypes.HasFlag ( TagLib.MediaTypes.Video ) )
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
