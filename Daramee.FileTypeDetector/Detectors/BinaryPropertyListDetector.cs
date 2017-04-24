﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class BinaryPropertyListDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] BPLIST_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x62, 0x70, 0x6C, 0x69, 0x73, 0x74 } },
		};

		public override string Extension => "bplist";

		protected override SignatureInformation [] SignatureInformations => BPLIST_SignatureInfo;

		public override string ToString () => "Apple Binary Property List Detector";
	}
}
