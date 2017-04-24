﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class DMGDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] DMG_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x78, 0x01, 0x73, 0x0D, 0x62, 0x62, 0x60 } },
		};

		public override string Extension => "dmg";

		protected override SignatureInformation [] SignatureInformations => DMG_SignatureInfo;

		public override string ToString () => "Apple Disk Mount Image(DMG) Detector";
	}
}
