using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class HWPDetector : AbstractRegexSignatureDetector
	{
		public override string Extension => "hwp";

		protected override Regex Signature => new Regex ( "^HWP Document File V[0-9]+.[0-9][0-9]" );

		public override string ToString () => "Hancom HWP Document Detector";
	}
}
