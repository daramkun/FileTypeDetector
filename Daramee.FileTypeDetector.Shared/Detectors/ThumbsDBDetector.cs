using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class ThumbsDBDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] THUMBDB_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFD, 0xFF, 0xFF, 0xFF } },
		};

		public override string Extension => "db";

		protected override SignatureInformation [] SignatureInformations => THUMBDB_SignatureInfo;

		public override string ToString () => "Thumbs.db Detector";
	}
}
