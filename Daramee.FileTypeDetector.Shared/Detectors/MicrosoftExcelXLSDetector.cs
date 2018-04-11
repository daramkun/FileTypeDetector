using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
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
