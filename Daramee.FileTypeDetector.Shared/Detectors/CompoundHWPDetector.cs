using System.Collections.Generic;
using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class CompoundHWPDetector : AbstractCompoundFileDetailDetector
	{
		public override IEnumerable<string> Chunks { get { yield return "FileHeader"; } }

		public override string Extension => "hwp";

		protected override bool IsValidChunk ( string chunkName, byte [] chunkData )
		{
			if ( chunkName == "FileHeader" && Encoding.UTF8.GetString ( chunkData, 0, 18 ) == "HWP Document File\0" )
				return true;
			return false;
		}

		public override string ToString () => "Hancom Compound File Binary Format Type HWP Document Detector";
	}
}
