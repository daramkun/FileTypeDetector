using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class Bzip2Detector : AbstractSignatureDetector
	{
		static SignatureInformation [] BZ2_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x42, 0x5A, 0x68 } },
		};

		public override string Extension => "bz2";

		protected override SignatureInformation [] SignatureInformations => BZ2_SignatureInfo;

		public override string ToString () => "Bunzip2 Detector";
	}
}
