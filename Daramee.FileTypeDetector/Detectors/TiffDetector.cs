using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class TiffDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] TIFF_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x49, 0x49, 0x2A, 0x00 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4D, 0x4D, 0x00, 0x2A } },
		};

		public override string Extension => "tif";

		protected override SignatureInformation [] SignatureInformations => TIFF_SignatureInfo;

		public override string ToString () => "TIFF Detector";
	}
}
