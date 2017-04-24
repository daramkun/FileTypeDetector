using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class MP4Detector : AbstractSignatureDetector
	{
		static SignatureInformation [] MP4_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70 } },
		};

		public override string Extension => "mp4";

		protected override SignatureInformation [] SignatureInformations => MP4_SignatureInfo;

		public override string ToString () => "MP4 Detector";
	}
}
