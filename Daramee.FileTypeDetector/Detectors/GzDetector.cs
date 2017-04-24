using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class GzDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] GZ_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x1F, 0x8B } },
		};

		public override string Extension => "gz";

		protected override SignatureInformation [] SignatureInformations => GZ_SignatureInfo;

		public override string ToString () => "GZ Detector";
	}
}
