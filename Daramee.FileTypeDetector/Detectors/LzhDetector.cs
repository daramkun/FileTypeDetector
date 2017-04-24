using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class LzhDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] LZH_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x2D, 0x6C, 0x68 } },
		};

		public override string Extension => "lzh";

		protected override SignatureInformation [] SignatureInformations => LZH_SignatureInfo;

		public override string ToString () => "LZH Detector";
	}
}
