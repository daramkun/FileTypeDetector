using System.Collections.Generic;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class MicrosoftPowerPointPPTDetector : AbstractCompoundFileDetailDetector
	{
		public override IEnumerable<string> Chunks { get { yield return "PowerPoint Document"; } }

		public override string Extension => "ppt";

		protected override bool IsValidChunk ( string chunkName, byte [] chunkData )
		{
			if ( chunkName == "PowerPoint Document" )
				return true;
			return false;
		}

		public override string ToString () => "Microsoft Office PowerPoint Document(PPT) Detector";
	}
}
