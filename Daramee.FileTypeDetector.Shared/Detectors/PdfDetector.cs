using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class PdfDetector : AbstractRegexSignatureDetector
	{
		public override string Extension => "pdf";

		protected override Regex Signature => new Regex ( "^%PDF-[0-9].[0-9]" );

		public override string ToString () => "Portable Document Format Detector";
	}
}
