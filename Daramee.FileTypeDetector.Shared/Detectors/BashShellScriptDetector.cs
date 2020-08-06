using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Executable )]
	class BashShellScriptDetector : AbstractRegexSignatureDetector
	{
		public override string Precondition => "txt";
		public override string Extension => "sh";

		protected override Regex Signature => new Regex ( "^#!\\/(.+)\n" );

		public override string ToString () => "Bash Shell Script Detector";
	}
}
