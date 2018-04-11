using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class RichTextFormatDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] RTF_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 } },
		};

		public override string Extension => "rtf";

		protected override SignatureInformation [] SignatureInformations => RTF_SignatureInfo;

		public override string ToString () => "Rich Text Format Detector";
	}
}
