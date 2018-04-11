using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class WebAssemblyDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] WASM_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x6D, 0x73, 0x61, 0x00 } },
		};

		public override string Extension => "wasm";

		protected override SignatureInformation [] SignatureInformations => WASM_SignatureInfo;

		public override string ToString () => "WebAssembly Binary Detector";
	}
}
