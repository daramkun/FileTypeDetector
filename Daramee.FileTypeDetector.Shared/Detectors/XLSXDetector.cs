using System.Collections.Generic;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class XLSXDetector : AbstractZipDetailDetector
	{
		public override IEnumerable<string> Files { get { yield return "[Content_Types].xml"; yield return "_rels/.rels"; yield return "xl/_rels/workbook.xml.rels"; } }

		public override string Precondition => "zip";
		public override string Extension => "xlsx";

		public override string ToString () => "Microsoft SpreadSheet Open XML Document(XLSX) Detector";
	}
}
