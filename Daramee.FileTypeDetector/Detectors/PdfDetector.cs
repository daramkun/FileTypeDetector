using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class PdfDetector : AbstractRegexSignatureDetector
	{
		public override string Extension => "pdf";

		protected override Regex Signature => new Regex ( "^%PDF-[0-9].[0-9]" );

		public override string ToString () => "Portable Document Format Detector";
	}
}
