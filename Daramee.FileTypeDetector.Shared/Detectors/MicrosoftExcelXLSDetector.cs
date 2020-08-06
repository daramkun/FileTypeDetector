using System.Collections.Generic;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class MicrosoftExcelXLSDetector : AbstractCompoundFileDetailDetector
	{
		public override IEnumerable<string> Chunks { get { yield return "Workbook"; } }

		public override string Extension => "xls";

		protected override bool IsValidChunk ( string chunkName, byte [] chunkData )
		{
			if ( chunkName == "Workbook" )
				return true;
			return false;
		}

		public override string ToString () => "Microsoft Office Excel Document(XLS) Detector";
	}
}
