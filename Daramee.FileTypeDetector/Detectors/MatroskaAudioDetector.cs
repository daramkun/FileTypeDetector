using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class MatroskaAudioDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] MKA_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x1A, 0x45, 0xDF, 0xA3 } },
		};

		public override string Extension => "mka";

		protected override SignatureInformation [] SignatureInformations => MKA_SignatureInfo;

		public override string ToString () => "Matroska Container Audio Only Detector";

		public override bool Detect ( Stream stream )
		{
			if ( base.Detect ( stream ) )
			{
				using ( var file = new TagLib.Matroska.File ( new TagLib.File.LocalStreamAbstraction ( stream ) ) )
				{
					if ( !file.Properties.MediaTypes.HasFlag ( TagLib.MediaTypes.Video ) )
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
