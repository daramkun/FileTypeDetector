using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class IconDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] ICO_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x00, 0x00, 0x01, 0x00 } },
		};

		public override string Extension => "ico";

		protected override SignatureInformation [] SignatureInformations => ICO_SignatureInfo;

		public override string ToString () => "Icon Detector";
	}
}
