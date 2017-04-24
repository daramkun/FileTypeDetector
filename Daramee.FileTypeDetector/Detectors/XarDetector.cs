﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class XarDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] XAR_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x78, 0x61, 0x72, 0x21 } },
		};

		public override string Extension => "xar";

		protected override SignatureInformation [] SignatureInformations => XAR_SignatureInfo;

		public override string ToString () => "XAR Detector";
	}
}
