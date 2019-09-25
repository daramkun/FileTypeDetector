using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Video )]
	[FormatCategory ( FormatCategories.Executable )]
	class ShockwaveFlashDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] SWF_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x43, 0x57, 0x53 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x46, 0x57, 0x53 } },
		};

		public override string Extension => "swf";

		protected override SignatureInformation [] SignatureInformations => SWF_SignatureInfo;

		public override string ToString () => "Shockwave Flash(SWF) Detector";
	}
}
