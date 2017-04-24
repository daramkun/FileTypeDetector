﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class BitmapDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] BMP_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x42, 0x4D } },
			new SignatureInformation () { Position = 6, Signature = new byte [] { 0x00, 0x00, 0x00, 0x00 }, Presignature = new byte [] { 0x42, 0x4D } },
		};

		public override string Extension => "bmp";

		protected override SignatureInformation [] SignatureInformations => BMP_SignatureInfo;

		public override string ToString () => "Bitmap(BMP) Detector";
	}
}
