using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class MsiDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] MSI_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } },
		};

		public override string Extension => "msi";

		protected override SignatureInformation [] SignatureInformations => MSI_SignatureInfo;

		public override string ToString () => "Microsoft Installer Detector";
	}
}
