using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class SRTDetector : AbstractRegexSignatureDetector
	{
		public override string Precondition => "txt";
		public override string Extension => "srt";

		protected override Regex Signature => new Regex ( "\\d+\r?\n(\\d\\d:\\d\\d:\\d\\d,\\d\\d\\d) --> (\\d\\d:\\d\\d:\\d\\d,\\d\\d\\d)\r?\n(.*)\r?\n\r?\n" );

		public override string ToString () => "SubRip Subtitle Detector";
	}
}
