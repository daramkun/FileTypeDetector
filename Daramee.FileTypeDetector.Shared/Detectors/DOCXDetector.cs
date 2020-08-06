using System.Collections.Generic;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class DOCXDetector : AbstractZipDetailDetector
	{
		public override IEnumerable<string> Files { get { yield return "[Content_Types].xml"; yield return "_rels/.rels"; yield return "word/_rels/document.xml.rels"; } }

		public override string Precondition => "zip";
		public override string Extension => "docx";
		
		public override string ToString () => "Microsoft Word Open XML Document(DOCX) Detector";
	}
}
