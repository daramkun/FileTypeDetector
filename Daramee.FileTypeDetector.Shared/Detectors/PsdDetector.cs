using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class PsdDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] PSD_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x38, 0x42, 0x50, 0x53 } },
		};

		public override string Extension => "psd";

		protected override SignatureInformation [] SignatureInformations => PSD_SignatureInfo;

		public override string ToString () => "Photoshop File(PSD) Detector";
	}
}
