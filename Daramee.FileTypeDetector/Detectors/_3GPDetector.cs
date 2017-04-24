using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class _3GPDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] _3GP_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x00, 0x00, 0x00, 0x14, 0x66, 0x74, 0x79, 0x70 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70 } },
		};

		public override string Extension => "3gp";

		protected override SignatureInformation [] SignatureInformations => _3GP_SignatureInfo;

		public override string ToString () => "3GPP Detector";
	}
}
