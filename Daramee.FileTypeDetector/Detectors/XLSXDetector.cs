using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class XLSXDetector : AbstractZipDetailDetector
	{
		public override IEnumerable<string> Files { get { yield return "[Content_Types].xml"; yield return "_rels/.rels"; yield return "xl/_rels/workbook.xml.rels"; } }

		public override string Precondition => "zip";
		public override string Extension => "xlsx";

		public override string ToString () => "Microsoft SpreedSheet Open XML Document(XLSX) Detector";
	}
}
