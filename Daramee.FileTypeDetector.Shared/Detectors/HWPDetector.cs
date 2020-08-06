using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class HWPDetector : AbstractRegexSignatureDetector
	{
		public override string Extension => "hwp";

		protected override Regex Signature => new Regex ( "^HWP Document File V[0-9]+.[0-9][0-9]" );

		public override string ToString () => "Hancom HWP Document Detector";
	}
}
