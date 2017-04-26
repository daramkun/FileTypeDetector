using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class PFXDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] PFX_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x30, 0x82, 0x06 } },
		};

		public override string Extension => "pfx";

		protected override SignatureInformation [] SignatureInformations => PFX_SignatureInfo;

		public override string ToString () => "Microsoft Personal inFormation eXchange Certificate Detector";
	}
}
