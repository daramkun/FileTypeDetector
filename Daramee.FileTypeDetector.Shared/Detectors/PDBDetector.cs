using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class PDBDetector : AbstractRegexSignatureDetector
	{
		public override string Extension => "pdb";

		protected override Regex Signature => new Regex ( "^Microsoft C\\/C\\+\\+ MSF [0-9].[0-9][0-9]\r\n" );

		public override string ToString () => "Microsoft Program Database Detector";
	}
}
