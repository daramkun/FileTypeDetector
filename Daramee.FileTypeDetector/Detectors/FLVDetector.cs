using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class FLVDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] FLV_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x46, 0x4C, 0x56 } },
		};

		public override string Extension => "flv";

		protected override SignatureInformation [] SignatureInformations => FLV_SignatureInfo;

		public override string ToString () => "Flash Video Detector";
	}
}
