using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class HWPMLDetector : AbstractRegexSignatureDetector
	{
		public override string Extension => "hwp";

		protected override Regex Signature => new Regex ( "^HWP Document File V[0-9]+.[0-9][0-9] \\\\x1a\\\\1\\\\2\\\\3\\\\4\\\\5" );

		public override string ToString () => "Hancom HWPML Document Detector";
	}
}
