using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class CabDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] CAB_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4D, 0x53, 0x43, 0x46 } },
		};

		public override string Extension => "cab";

		protected override SignatureInformation [] SignatureInformations => CAB_SignatureInfo;

		public override string ToString () => "Windows Cabinet Detector";
	}
}
