using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class JavaClassDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] CLASS_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xCA, 0xFE, 0xBA, 0xBE } },
		};

		public override string Extension => "class";

		protected override SignatureInformation [] SignatureInformations => CLASS_SignatureInfo;

		public override string ToString () => "Java Bytecode Detector";
	}
}
