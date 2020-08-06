using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	[FormatCategory ( FormatCategories.System )]
	class REGDetector : AbstractRegexSignatureDetector
	{
		public override string Precondition => "txt";
		public override string Extension => "reg";

		protected override Regex Signature => new Regex ( "^Windows Registry Editor Version [0-9]+.[0-9][0-9]\r?\n" );

		public override string ToString () => "Windows Registry Detector";
	}
}
