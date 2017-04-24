using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class FlacDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] FLAC_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x66, 0x4C, 0x61, 0x43 } },
		};

		public override string Extension => "flac";

		protected override SignatureInformation [] SignatureInformations => FLAC_SignatureInfo;

		public override string ToString () => "FLAC Detector";
	}
}
