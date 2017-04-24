﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class CursorDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] CUR_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x00, 0x00, 0x02, 0x00 } },
		};

		public override string Extension => "cur";

		protected override SignatureInformation [] SignatureInformations => CUR_SignatureInfo;

		public override string ToString () => "Cursor Detector";
	}
}
